using UnityEngine;
using System.Collections;

public class Neighborhood : MonoBehaviour {

    public Dealer FriendlyDealer;
    public float Respect;

    private MeshRenderer blockRenderer;

	// Use this for initialization
	void Start () {
        blockRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(FriendlyDealer != null)
        {
            blockRenderer.material.color = Color.Lerp(Color.white, FriendlyDealer.DealerColor, Respect);
        }
        else
        {
            if (blockRenderer.material.color != Color.white)
                blockRenderer.material.color = Color.white;
        }
	}
}
