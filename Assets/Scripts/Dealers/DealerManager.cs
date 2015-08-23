using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DealerManager : MonoBehaviour {
    public List<Dealer> Dealers;

    public List<Color> Colors;

    public int DealerCount = 2;

    public City City;
    public SupplierManager SupplierManager;
    public InputManager InputManager;

    // Use this for initialization
    void Start () {
        Dealers = new List<Dealer>();
	    for(int i = 0; i < DealerCount; i++)
        {
            var dealer = new GameObject("dealer", typeof(Dealer));
            var props = dealer.GetComponent<Dealer>();
            if (i == 0)
            {
                props.DealerColor = Color.red;
                props.Money = 20000;
                props.Reputation = 0;
                props.SellTaxRate = 0.9f;
                props.BuyTaxRate = 1;
                props.Name = "player";
                props.IsPlayer = true;
            }
            else
            {
                props.DealerColor = Colors[i - 1];
                props.Money = Random.Range(100000, 1000000);
                props.Reputation = Random.Range(0.7f, 1f);
                props.SellTaxRate = Random.Range(1.1f, 1.3f);
                props.BuyTaxRate = Random.Range(1.1f, 1.3f);
                props.Name = "NPC";
                props.IsPlayer = false;

            }
            props.Manager = this;
            Dealers.Add(props);
            dealer.transform.SetParent(transform);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
