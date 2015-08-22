using UnityEngine;
using System.Collections;

public class City : MonoBehaviour {

    public GameObject NeighborhoodPrefab;

    public DealerManager DealerManager;

    public Vector2 CitySize = new Vector2(5, 5);
    public Vector2 BlockSize = new Vector2(10, 10);

    public float[,] CityInfluenceMap;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine("CreateCity");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator CreateCity()
    {
        yield return new WaitForEndOfFrame();

        CityInfluenceMap = new float[(int)CitySize.x, (int)CitySize.y];
        var startX = -CitySize.x / 2f * BlockSize.x / 2f;
        var startY = -CitySize.y / 2f * BlockSize.y / 2f;

        var randomOffset = Random.Range(-100f, 100f);

        for (int x = 0; x < CitySize.x; x++)
        {
            for (int y = 0; y < CitySize.y; y++)
            {
                CityInfluenceMap[x, y] = Mathf.PerlinNoise(x/10f + 0.1f + randomOffset, y/10f + 0.1f + randomOffset);
                var block = Instantiate(NeighborhoodPrefab, new Vector3(startX + BlockSize.x * x, 0, startY + BlockSize.y * y), Quaternion.Euler(0, 0, 0)) as GameObject;

                var neighborhood = block.GetComponent<Neighborhood>();
                neighborhood.FriendlyDealer = (CityInfluenceMap[x, y] > 0.5f) ? DealerManager.Dealers[1] : null ;
                neighborhood.Respect = Random.Range(0.5f, 1f);
                block.transform.SetParent(transform);
            }
        }
    }
}
