using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Dealer : MonoBehaviour {
    public string Name;
    public int Money;
    public float Reputation;
    public Guid ID = Guid.NewGuid();
    public float BuyTaxRate;
    public float SellTaxRate;

    public Color DealerColor;
    public List<Supplier> ContractSuppliers = new List<Supplier>();

    public bool IsPlayer;
    public Text MoneyLabel;
    public Text WeaponsLabel;
    public Text DayLabel;

    private float timeOfDay;
    private const float dayDuration = 10f;

    public List<Weapon> WeaponStock = new List<Weapon>();

    public DealerManager Manager;

    // Use this for initialization
    void Start () {
	    if(IsPlayer)
        {
            MoneyLabel = GameObject.Find("MoneyLabel").GetComponent<Text>();
            WeaponsLabel = GameObject.Find("WeaponsLabel").GetComponent<Text>();
            DayLabel = GameObject.Find("DayLabel").GetComponent<Text>();
        }
        else
        {
            StartCoroutine("DealerAI");
        }
        StartCoroutine("DayCycle");
    }
	
	// Update is called once per frame
	void Update () {
        timeOfDay += Time.deltaTime;
        if (IsPlayer)
        {
            MoneyLabel.text = "$" + Money.ToString();
            var weapons = "";
            var WeaponTypes = Enum.GetNames(typeof(WeaponType));
            for(int i = 0; i < WeaponTypes.Length; i++)
            {
                var count = WeaponStock.Where(w => w.Type == (WeaponType)Enum.Parse(typeof(WeaponType), WeaponTypes[i])).Count();
                weapons += WeaponTypes[i] + " : " + count.ToString() + "  ";
            }
            WeaponsLabel.text = weapons;
            DayLabel.text = (timeOfDay * 100f / dayDuration).ToString("f0") + "%";
        }
    }

    IEnumerator DayCycle()
    {
        for(; ;)
        {
            yield return new WaitForSeconds(dayDuration);
            timeOfDay = 0;
            foreach (var c in ContractSuppliers)
            {
                foreach(var t in c.TypesSelling)
                {
                    for(int i = 0; i < t.Value; i++)
                    {
                        WeaponStock.AddRange(WeaponProvider.CreateWeapon(t.Key, c.QualitySelling).MakeStock(6));
                        switch(t.Key)
                        {
                            case WeaponType.Handgun:
                                Money -= (int)(100 * c.SellRate);
                                break;
                            case WeaponType.SMG:
                                Money -= (int)(200 * c.SellRate);
                                break;
                            case WeaponType.MachineGun:
                                Money -= (int)(400 * c.SellRate);
                                break;
                            case WeaponType.Explosives:
                                Money -= (int)(800 * c.SellRate);
                                break;
                        }
                    }
                }
            }
        }
    }

    IEnumerator DealerAI()
    {
        var addSupplier = 0f;
        for(; ;)
        {
            yield return new WaitForSeconds(0.5f);
            
            if (addSupplier % 5 == 0 &&ContractSuppliers.Count < 5 && Manager.SupplierManager.AvailableSuppliers.Count > 0)
            {
                var contract = Manager.SupplierManager.AvailableSuppliers[0];
                ContractSuppliers.Add(contract);
                Manager.SupplierManager.AvailableSuppliers.Remove(contract);
                addSupplier = 0;
            }
            addSupplier += 0.5f;

            var blockId = UnityEngine.Random.Range(0, Manager.City.Neighborhoods.Count);
            var block = Manager.City.Neighborhoods[blockId].GetComponent<BuyerManager>();
            var buyer = block.AvailableBuyers[UnityEngine.Random.Range(0, block.AvailableBuyers.Count)];

            var canSell = true;
            foreach (var t in buyer.TypesBuying)
            {
                if (WeaponStock.Where(w => w.Type == t.Key).Count() < t.Value)
                {
                    canSell = false;
                    break;
                }
            }
            if (canSell)
            {
                foreach (var t in buyer.TypesBuying)
                {
                    for (int u = 0; u < t.Value; u++)
                    {
                        var weapon = WeaponStock.Where(w => w.Type == t.Key).First();
                        WeaponStock.Remove(weapon);
                    }
                }
                var neighborBlocks = Manager.City.Neighborhoods.Where(n => Vector3.Distance(n.transform.position, block.transform.position) < 20).ToList();
                neighborBlocks.Remove(Manager.City.Neighborhoods[blockId]);
                Manager.City.Neighborhoods[blockId].Respect[ID] += 0.05f;
                Manager.City.Neighborhoods[blockId].Respect[ID] = Mathf.Clamp(Manager.City.Neighborhoods[blockId].Respect[ID], 0, 1);


                for (int u = 1; u < Manager.Dealers.Where(d => d != this).ToList().Count; u++)
                {
                    Manager.City.Neighborhoods[blockId].Respect[Manager.Dealers[u].ID] -= 0.025f;
                    Manager.City.Neighborhoods[blockId].Respect[Manager.Dealers[u].ID] = Mathf.Clamp(Manager.City.Neighborhoods[blockId].Respect[Manager.Dealers[u].ID], 0, 1);
                }
                foreach (var n in neighborBlocks)
                {
                    n.Respect[ID] += 0.025f;
                    n.Respect[ID] = Mathf.Clamp(n.Respect[ID], 0, 1);
                    for (int u = 1; u < Manager.Dealers.Where(d => d != this).ToList().Count; u++)
                    {
                        n.Respect[Manager.Dealers[u].ID] -= 0.0125f;
                        n.Respect[Manager.Dealers[u].ID] = Mathf.Clamp(n.Respect[Manager.Dealers[u].ID], 0, 1);
                    }
                }
                block.AvailableBuyers.Remove(buyer);
                Money += buyer.Price;
                Debug.Log("buy");
            }
        }
    }
}

public class Supplier
{
    public Guid SupplierID = Guid.NewGuid();
    public string Name = "Supplier";
    public Dictionary<WeaponType, int> TypesSelling { get; set; }
    public WeaponQuality QualitySelling { get; set; }
    public float SellRate { get; set; }
    public Dictionary<Guid, float> Respect { get; set; }
    public int Price
    {
        get
        {
            var money = 0;
            foreach (var t in TypesSelling)
            {
                for (int i = 0; i < t.Value; i++)
                {
                    switch (t.Key)
                    {
                        case WeaponType.Handgun:
                            money += (int)(100 * SellRate);
                            break;
                        case WeaponType.SMG:
                            money += (int)(200 * SellRate);
                            break;
                        case WeaponType.MachineGun:
                            money += (int)(400 * SellRate);
                            break;
                        case WeaponType.Explosives:
                            money += (int)(800 * SellRate);
                            break;
                    }
                }
            }
            return money;
        }
    }
}