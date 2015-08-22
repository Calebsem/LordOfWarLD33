using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Dealer : MonoBehaviour {
    public string Name;
    public int Money;
    public float Reputation;
    public Guid MafiaID = Guid.NewGuid();
    public float BuyTaxRate;
    public float SellTaxRate;

    public Color DealerColor;

    public bool IsPlayer;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class Supplier
{
    public Guid SupplierID = Guid.NewGuid();
    public string Name = "Supplier";
    public List<WeaponType> TypesSelling { get; set; }
    public WeaponQuality QualitySelling { get; set; }
    public float SellRate { get; set; }
    public Dictionary<Guid, float> Respect { get; set; }
}