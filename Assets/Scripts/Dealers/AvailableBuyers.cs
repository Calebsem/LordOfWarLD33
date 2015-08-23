using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class AvailableBuyers : MonoBehaviour {
    List<Text[]> Labels;
    List<Button> Buttons;
    public BuyerManager BuyerManager;
    public SelectedBuyer SelectedBuyer;
    // Use this for initialization
    void Start ()
    {
        Buttons = GetComponentsInChildren<Button>().ToList();
        Labels = Buttons.Select(t => t.GetComponentsInChildren<Text>()).ToList();

    }
	
	// Update is called once per frame
	void Update ()
    {
        BuyerManager = SelectedBuyer.InputManager.SelectedNeighborhood.GetComponent<BuyerManager>();
        var list = BuyerManager.AvailableBuyers;
        var availableCount = list.Count;
        for (int i = availableCount; i < Buttons.Count; i++)
        {
            Buttons[i].interactable = false;
            foreach (var t in Labels[i])
            {
                t.text = "";
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            var type = "";
            foreach (var t in item.TypesBuying)
            {
                type += t.Key.ToString() + " " + t.Value.ToString() + " - ";
            }
            Labels[i][0].text = item.Name;
            Labels[i][1].text = type;
            Labels[i][2].text = "$" + item.Price.ToString() + " x " + item.Duration.ToString();
            Buttons[i].interactable = true;
        }

    }
}
