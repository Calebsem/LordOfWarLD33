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

        if(inputManager.SelectedNeighborhood == this)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
	}
}
