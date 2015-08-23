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

    public Buyer GenerateBuyer()
    {
        var typeQuantities = new Dictionary<WeaponType, int>();
        var typeValues = Enum.GetNames(typeof(WeaponType)).ToList();
        var selling = (WeaponType)Enum.Parse(typeof(WeaponType), typeValues[UnityEngine.Random.Range(0, typeValues.Count)]);
        typeValues.Remove(selling.ToString());
        typeQuantities.Add(selling, UnityEngine.Random.Range(1, 3));
        //var selling2 = (WeaponType)Enum.Parse(typeof(WeaponType), typeValues[UnityEngine.Random.Range(0, typeValues.Count)]);
        //typeQuantities.Add(selling2, UnityEngine.Random.Range(1, 3));
        return new Buyer
        {
            Name = "Buyer",
            SellRate = (neighborhood.FriendlyDealer == null) ? 1 : neighborhood.FriendlyDealer.SellTaxRate,
            TypesBuying = typeQuantities,
            Duration = UnityEngine.Random.Range(1, 5)
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
            yield return new WaitForSeconds(Constants.DayDuration);
        }
    }
}

public class Buyer
{

    public Guid SupplierID = Guid.NewGuid();
    public string Name = "Supplier";
    public Dictionary<WeaponType, int> TypesBuying { get; set; }
    public float SellRate { get; set; }
    public int Duration { get; set; }
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
                            money += (int)(40 / SellRate);
                            break;
                        case WeaponType.SMG:
                            money += (int)(80 / SellRate);
                            break;
                        case WeaponType.MachineGun:
                            money += (int)(160 / SellRate);
                            break;
                        case WeaponType.Explosives:
                            money += (int)(320 / SellRate);
                            break;
                    }
                }
            }
            return money;
        }
    }
}
