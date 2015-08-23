using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Neighborhood : MonoBehaviour {

    public Dealer FriendlyDealer;
    public Dictionary<Guid, float> Respect;
    public string Name;

    private MeshRenderer blockRenderer;
    private InputManager inputManager;

    public DealerManager DealerManager;
    // Use this for initialization
    void Start () {
        blockRenderer = GetComponent<MeshRenderer>();
        inputManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputManager>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(FriendlyDealer != null)
        {
            blockRenderer.material.color = Color.Lerp(Color.white, FriendlyDealer.DealerColor, Respect[FriendlyDealer.ID]);
        }
        else
        {
            if (blockRenderer.material.color != Color.white)
                blockRenderer.material.color = Color.white;
        }
        if ((FriendlyDealer != null && Respect.Max(r => r.Value) > Respect[FriendlyDealer.ID]) || (Respect.Any(r => r.Value > 0)))
        {
            FriendlyDealer = DealerManager.Dealers.First(d => d.ID == Respect.First(re => re.Value == Respect.Max(r => r.Value)).Key);
        }

        if (inputManager.SelectedNeighborhood == this)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
	}
}
