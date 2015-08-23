using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BuyerManager : MonoBehaviour
{
    public List<Buyer> AvailableBuyers;

    private Neighborhood neighborhood;

    // Use this for initialization
    void Start () {
        neighborhood = GetComponent<Neighborhood>();
        AvailableBuyers = new List<Buyer>();
        StartCoroutine("MakeBuyer");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    Buyer GenerateBuyer()
    {
        var typeQuantities = new Dictionary<WeaponType, int>();
        typeQuantities.Add(WeaponType.Handgun, UnityEngine.Random.Range(1, 8));
        typeQuantities.Add(WeaponType.SMG, UnityEngine.Random.Range(0, 6));
        typeQuantities.Add(WeaponType.MachineGun, UnityEngine.Random.Range(0, 4));
        typeQuantities.Add(WeaponType.Explosives, UnityEngine.Random.Range(0, 2));
        return new Buyer
        {
            Name = "Buyer",
            SellRate = (neighborhood.FriendlyDealer == null) ? 1 : neighborhood.FriendlyDealer.SellTaxRate,
            TypesBuying = typeQuantities
        };
    }

    IEnumerator MakeBuyer()
    {
        for (; ;)
        {
            if (AvailableBuyers.Count >= 6)
            {
                AvailableBuyers.RemoveAt(UnityEngine.Random.Range(0, AvailableBuyers.Count - 1));
            }
            AvailableBuyers.Add(GenerateBuyer());
            yield return new WaitForSeconds(2);
        }
    }
}

public class Buyer
{

    public Guid SupplierID = Guid.NewGuid();
    public string Name = "Supplier";
    public Dictionary<WeaponType, int> TypesBuying { get; set; }
    public float SellRate { get; set; }
    public int Price
    {
        get
        {
            var money = 0;
            foreach (var t in TypesBuying)
            {
                for (int i = 0; i < t.Value; i++)
                {
                    switch (t.Key)
                    {
                        case WeaponType.Handgun:
                            money += (int)(16 / SellRate);
                            break;
                        case WeaponType.SMG:
                            money += (int)(40 / SellRate);
                            break;
                        case WeaponType.MachineGun:
                            money += (int)(80 / SellRate);
                            break;
                        case WeaponType.Explosives:
                            money += (int)(200 / SellRate);
                            break;
                    }
                }
            }
            return money;
        }
    }
}
