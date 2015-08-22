using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {
    public GameObject InfoPanel;
    public GameObject SupplierPanel;
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.collider.tag == "Neighborhood")
            {
                var block = hit.collider.GetComponent<Neighborhood>();
                foreach (var c in InfoPanel.GetComponentsInChildren<Text>())
                {
                    switch (c.gameObject.name)
                    {
                        case "Name":
                            c.text = block.Name;
                            break;
                        case "DealerName":
                            if (block.FriendlyDealer != null)
                                c.text = block.FriendlyDealer.Name;
                            else
                                c.text = "Unoccupied";
                            break;
                        case "Respect":
                            if (block.FriendlyDealer != null)
                                c.text = Mathf.RoundToInt(block.Respect * 100f).ToString() + "%";
                            else
                                c.text = "??%";
                            break;
                    }
                }
            }
        }
    }

    public void OpenSuppliers()
    {
        SupplierPanel.SetActive(true);
    }


    public void CloseSuppliers()
    {
        SupplierPanel.SetActive(false);
    }
}
