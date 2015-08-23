using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class SupplierManager : MonoBehaviour {
    public List<Supplier> AvailableSuppliers;
    public List<Supplier> ContractSuppliers;
    public DealerManager DealerManager;
	// Use this for initialization
	void Start ()
    {
        AvailableSuppliers = new List<Supplier>();
        StartCoroutine("MakeSupplier");
    }
	
	// Update is called once per frame
	void Update ()
    {
        ContractSuppliers = DealerManager.Dealers[0].ContractSuppliers;
    }

    public Supplier GenerateSupplier()
    {
        var qualityValues = Enum.GetNames(typeof(WeaponQuality)).ToList();
        var typeValues = Enum.GetNames(typeof(WeaponType)).ToList();
        var selling = (WeaponType)Enum.Parse(typeof(WeaponType), typeValues[UnityEngine.Random.Range(0, typeValues.Count)]);
        typeValues.Remove(selling.ToString());
        var selling2 = (WeaponType)Enum.Parse(typeof(WeaponType), typeValues[UnityEngine.Random.Range(0, typeValues.Count)]);
        var typeQuantities = new Dictionary<WeaponType, int>();
        typeQuantities.Add(selling, UnityEngine.Random.Range(1,3));
        typeQuantities.Add(selling2, UnityEngine.Random.Range(1, 3));

        var respects = new Dictionary<Guid, float>();
        foreach(var d in DealerManager.Dealers)
        {
            respects.Add(d.ID, 0.5f);
        }
        return new Supplier
        {
            Name = "Supplier",
            Respect = respects,
            SellRate = UnityEngine.Random.Range(1.1f, 1.3f),
            TypesSelling = typeQuantities,
            QualitySelling = (WeaponQuality)Enum.Parse(typeof(WeaponQuality), qualityValues[UnityEngine.Random.Range(0, qualityValues.Count)])
        };
    }

    IEnumerator MakeSupplier()
    {
        for(; ;)
        {
            if (AvailableSuppliers.Count >= 5)
            {
                AvailableSuppliers.RemoveAt(UnityEngine.Random.Range(0, AvailableSuppliers.Count - 1));
            }
            AvailableSuppliers.Add(GenerateSupplier());
            if (AvailableSuppliers.Count < 5)
            {
                AvailableSuppliers.Add(GenerateSupplier());
            }
            yield return new WaitForSeconds(Constants.DayDuration);
        }
    }

    public void GetContract(int i)
    {
        if (ContractSuppliers.Count >= 5)
            return;
        var contract = AvailableSuppliers[i];
        AvailableSuppliers.RemoveAt(i);
        ContractSuppliers.Add(contract);
    }
    public void RemoveContract(int i)
    {
        ContractSuppliers.RemoveAt(i);
    }
}
