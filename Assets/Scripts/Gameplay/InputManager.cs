using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class InputManager : MonoBehaviour
{

    public GameObject InfoPanel;
    public GameObject SupplierPanel;
    public GameObject BuyerPanel;


    public Neighborhood SelectedNeighborhood;

    private bool wasClicking;

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
        var hasHit = false;
        if (hasHit = Physics.Raycast(ray, out hit, 100.0f))
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
                                c.text = Mathf.RoundToInt(block.Respect[block.FriendlyDealer.ID] * 100f).ToString() + "%";
                            else
                                c.text = "??%";
                            break;
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    SelectedNeighborhood = block;
                    BuyerPanel.SetActive(true);
                    //Camera.main.GetComponent<CameraInteraction>().CenterPoint = new Vector3(block.transform.position.x, 0, block.transform.position.z);
                }
            }
        }
        if (!hasHit && Input.GetMouseButtonDown(0))
        {
            SelectedNeighborhood = null;
            BuyerPanel.SetActive(false);
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
