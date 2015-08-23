using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class SelectedBuyer : MonoBehaviour {
    public InputManager InputManager;
    public List<Text> TextInputs;
    public AvailableBuyers Buyers;
    public DealerManager DealerManager;
    public City City;
    // Use this for initialization
    void Start () {
        TextInputs = GetComponentsInChildren<Text>().ToList();
	}
	
	// Update is called once per frame
	void Update () {
	    if(InputManager.SelectedNeighborhood != null)
        {
            TextInputs[0].text = InputManager.SelectedNeighborhood.Name;
            if (InputManager.SelectedNeighborhood.FriendlyDealer != null)
            {
                TextInputs[1].text = Mathf.RoundToInt(InputManager.SelectedNeighborhood.Respect[InputManager.SelectedNeighborhood.FriendlyDealer.ID] * 100f).ToString() + "%";
                TextInputs[2].text = InputManager.SelectedNeighborhood.FriendlyDealer.Name;
                TextInputs[3].text = "x" + InputManager.SelectedNeighborhood.FriendlyDealer.SellTaxRate.ToString("f1");
                if(InputManager.SelectedNeighborhood.FriendlyDealer == DealerManager.Dealers[0])
                {
                    TextInputs[4].gameObject.SetActive(false);
                }
                else
                {
                    TextInputs[4].gameObject.SetActive(true);
                }
            }
            else
            {
                TextInputs[1].text = "??%";
                TextInputs[2].text = "Unoccupied";
                TextInputs[3].text = "x1";
                TextInputs[4].gameObject.SetActive(true);
            }
            TextInputs[4].text = "me : " + Mathf.RoundToInt(InputManager.SelectedNeighborhood.Respect[DealerManager.Dealers[0].ID] * 100f).ToString() + "%";
        }
	}

    public void GetBuyer(int i)
    {
        var buyer = Buyers.BuyerManager.AvailableBuyers[i];
        if(DealerManager.Dealers[0].SellToDealer(buyer, InputManager.SelectedNeighborhood))
            Buyers.BuyerManager.AvailableBuyers.RemoveAt(i);
    }
}
