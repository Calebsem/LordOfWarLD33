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

    // Use this for initialization
    void Start () {
	    if(IsPlayer)
        {
            MoneyLabel = GameObject.Find("MoneyLabel").GetComponent<Text>();
            WeaponsLabel = GameObject.Find("WeaponsLabel").GetComponent<Text>();
            DayLabel = GameObject.Find("DayLabel").GetComponent<Text>();
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