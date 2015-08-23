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
        var canSell = true;
        foreach (var t in buyer.TypesBuying)
        {
            if(DealerManager.Dealers[0].WeaponStock.Where(w => w.Type == t.Key).Count() < t.Value)
            {
                canSell = false;
                break;
            }
        }
        if (!canSell)
            return;

        foreach (var t in buyer.TypesBuying)
        {
            for (int u = 0; u < t.Value; u++)
            {
                var weapon = DealerManager.Dealers[0].WeaponStock.Where(w => w.Type == t.Key).First();
                DealerManager.Dealers[0].WeaponStock.Remove(weapon);
            }
        }
        var neighborBlocks = City.Neighborhoods.Where(n => Vector3.Distance(n.transform.position, InputManager.SelectedNeighborhood.transform.position) < 20).ToList();
        neighborBlocks.Remove(InputManager.SelectedNeighborhood);
        InputManager.SelectedNeighborhood.Respect[DealerManager.Dealers[0].ID] += 0.05f;
        InputManager.SelectedNeighborhood.Respect[DealerManager.Dealers[0].ID] = Mathf.Clamp(InputManager.SelectedNeighborhood.Respect[DealerManager.Dealers[0].ID], 0, 1);


        for (int u = 1; u < DealerManager.Dealers.Count; u++)
        {
            InputManager.SelectedNeighborhood.Respect[DealerManager.Dealers[u].ID] -= 0.025f;
            InputManager.SelectedNeighborhood.Respect[DealerManager.Dealers[u].ID] = Mathf.Clamp(InputManager.SelectedNeighborhood.Respect[DealerManager.Dealers[u].ID], 0, 1);
        }
        foreach (var n in neighborBlocks)
        {
            n.Respect[DealerManager.Dealers[0].ID] += 0.025f;
            n.Respect[DealerManager.Dealers[0].ID] = Mathf.Clamp(n.Respect[DealerManager.Dealers[0].ID], 0, 1);
            for (int u = 1; u < DealerManager.Dealers.Count; u++)
            {
                n.Respect[DealerManager.Dealers[u].ID] -= 0.0125f;
                n.Respect[DealerManager.Dealers[u].ID] = Mathf.Clamp(n.Respect[DealerManager.Dealers[u].ID], 0, 1);
            }
        }
        Buyers.BuyerManager.AvailableBuyers.RemoveAt(i);
        DealerManager.Dealers[0].Money += buyer.Price;
    }
}
