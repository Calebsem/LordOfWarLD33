using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class AvailableSupplierControl : MonoBehaviour {
    List<Text[]> Labels;
    List<Button> Buttons;
    public SupplierManager SupplierManager;
    public bool Available = true;

    // Use this for initialization
    void Start ()
    {
        Buttons = GetComponentsInChildren<Button>().ToList();
        Labels = Buttons.Select(t => t.GetComponentsInChildren<Text>()).ToList();
    }
	
	// Update is called once per frame
	void Update ()
    {
        var list = (Available) ? SupplierManager.AvailableSuppliers : SupplierManager.ContractSuppliers;
        var availableCount = list.Count;
        for(int i = availableCount; i < Buttons.Count; i++)
        {
            Buttons[i].interactable = false;
            foreach(var t in Labels[i])
            {
                t.text = "";
            }
        }
        for(int i = 0; i < list.Count; i++)
        {
            var type = "";
            foreach(var t in list[i].TypesSelling)
            {
                type += t.Key.ToString() + " " + t.Value.ToString() + " - ";
            }
            Labels[i][0].text = list[i].Name;
            Labels[i][1].text = type;
            Labels[i][2].text = list[i].QualitySelling.ToString();
            Labels[i][3].text = list[i].SellRate.ToString("F2") + "x ($" + list[i].Price.ToString() + ")";
            Buttons[i].interactable = true;
        }
	}
}
