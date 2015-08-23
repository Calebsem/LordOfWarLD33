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

    public List<Weapon> WeaponStock = new List<Weapon>();
    public Dictionary<Neighborhood, List<Buyer>> ContractedBuyers = new Dictionary<Neighborhood, List<Buyer>>();

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
            DayLabel.text = (timeOfDay * 100f / Constants.DayDuration).ToString("f0") + "%";
        }
    }

    IEnumerator DayCycle()
    {
        for(; ;)
        {
            yield return new WaitForSeconds(Constants.DayDuration);
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



            foreach (var n in ContractedBuyers)
            {
                var toRemove = new List<Buyer>();
                foreach(var b in n.Value)
                {
                    var canSell = true;
                    foreach (var t in b.TypesBuying)
                    {
                        if (WeaponStock.Where(w => w.Type == t.Key).Count() < t.Value)
                        {
                            canSell = false;
                            break;
                        }
                    }
                    if(!canSell)
                    {
                        AffectRespect(-0.025f, n.Key);
                        continue;
                    }
                    var cargo = Instantiate(Manager.CargoPrefab, new Vector3(-22.5f, 5, 7.5f), Quaternion.Euler(0, 0, 0)) as GameObject;
                    var behavior = cargo.GetComponent<CargoBehavior>();
                    behavior.Dealer = this;
                    behavior.Buyer = b;
                    behavior.Block = n.Key;
                    b.Duration--;
                    if (b.Duration == 0)
                        toRemove.Add(b);
                    foreach (var t in b.TypesBuying)
                    {
                        for (int u = 0; u < t.Value; u++)
                        {
                            var weapon = WeaponStock.Where(w => w.Type == t.Key).First();
                            WeaponStock.Remove(weapon);
                        }
                    }
                    AffectRespect(0.05f, n.Key);
                    Money += b.Price;
                }
                foreach(var r in toRemove)
                {
                    n.Value.Remove(r);
                }

            }
        }
    }

    void AffectRespect(float Amount, Neighborhood block)
    {
        var neighborBlocks = Manager.City.Neighborhoods.Where(ne => Vector3.Distance(ne.transform.position, block.transform.position) < 20).ToList();
        neighborBlocks.Remove(block);
        block.Respect[ID] += Amount;
        block.Respect[ID] = Mathf.Clamp(block.Respect[ID], 0, 1);


        for (int u = 1; u < Manager.Dealers.Where(d => d != this).ToList().Count; u++)
        {
            block.Respect[Manager.Dealers[u].ID] -= Amount / 2f;
            block.Respect[Manager.Dealers[u].ID] = Mathf.Clamp(block.Respect[Manager.Dealers[u].ID], 0, 1);
        }
        foreach (var n in neighborBlocks)
        {
            n.Respect[ID] += Amount / 2f;
            n.Respect[ID] = Mathf.Clamp(n.Respect[ID], 0, 1);
            for (int u = 1; u < Manager.Dealers.Where(d => d != this).ToList().Count; u++)
            {
                n.Respect[Manager.Dealers[u].ID] -= Amount / 4f;
                n.Respect[Manager.Dealers[u].ID] = Mathf.Clamp(n.Respect[Manager.Dealers[u].ID], 0, 1);
            }
        }
    }

    IEnumerator DealerAI()
    {
        var addSupplier = 0f;
        for (; ;)
        {
            yield return new WaitForSeconds(0.5f);

            if (addSupplier % 5 == 0 && ContractSuppliers.Count < 5 && Manager.SupplierManager.AvailableSuppliers.Count > 0)
            {
                var contract = Manager.SupplierManager.GenerateSupplier();
                ContractSuppliers.Add(contract);
                addSupplier = 0;
            }
            addSupplier += 0.5f;

            var blockId = UnityEngine.Random.Range(0, Manager.City.Neighborhoods.Count);
            var block = Manager.City.Neighborhoods[blockId].GetComponent<BuyerManager>();
            var buyer = block.GenerateBuyer();
            SellToDealer(buyer, Manager.City.Neighborhoods[blockId]);
        }
    }

    public bool SellToDealer(Buyer buyer, Neighborhood block)
    {
        var canSell = true;

        if (block.Respect[ID] < 0.2f)
            return false;
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
            if(!ContractedBuyers.ContainsKey(block))
            {
                ContractedBuyers.Add(block, new List<Buyer>());
            }
            ContractedBuyers[block].Add(buyer);

        }
        return canSell;
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