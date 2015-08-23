using UnityEngine;
using System.Collections;

public class CargoBehavior : MonoBehaviour {
    public Neighborhood Block;
    public Dealer Dealer;
    public Buyer Buyer;

    public float Speed = 4f;
    public float RandomWait;
    private MeshRenderer cargoRenderer;
	// Use this for initialization
	void Start () {
        cargoRenderer = GetComponent<MeshRenderer>();
        RandomWait = Random.Range(0.5f, 5);
	}
	
	// Update is called once per frame
	void Update ()
    {
        cargoRenderer.material.color = Dealer.DealerColor;
        if (RandomWait > 0)
        {
            RandomWait -= Time.deltaTime;
            return;
        }
        var threshold = 2f;
        if(Block.transform.position.x + threshold < transform.position.x || Block.transform.position.x - threshold > transform.position.x)
        {
            var dir = (Block.transform.position.x - transform.position.x > 0) ? 1 : -1;
            transform.position += new Vector3(Speed * Time.deltaTime * dir, 0);
        }
        else if (Block.transform.position.z + threshold < transform.position.z || Block.transform.position.z - threshold > transform.position.z)
        {
            var dir = (Block.transform.position.z - transform.position.z > 0) ? 1 : -1;
            transform.position += new Vector3(0, 0, Speed * Time.deltaTime * dir);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
