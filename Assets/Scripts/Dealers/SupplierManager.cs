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

    Supplier GenerateSupplier()
    {
        var qualityValues = Enum.GetNames(typeof(WeaponQuality)).ToList();
        var typeQuantities = new Dictionary<WeaponType, int>();
        typeQuantities.Add(WeaponType.Handgun, 3);
        typeQuantities.Add(WeaponType.SMG, 2);
        typeQuantities.Add(WeaponType.MachineGun, 1);
        typeQuantities.Add(WeaponType.Explosives, 1);

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
            yield return new WaitForSeconds(2);
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
