using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class City : MonoBehaviour {

    public GameObject NeighborhoodPrefab;

    public DealerManager DealerManager;

    public Vector2 CitySize = new Vector2(5, 5);
    public Vector2 BlockSize = new Vector2(10, 10);

    public float[,] CityInfluenceMap;
    public List<Neighborhood> Neighborhoods;

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
        Neighborhoods = new List<Neighborhood>();
        for (int x = 0; x < CitySize.x; x++)
        {
            for (int y = 0; y < CitySize.y; y++)
            {
                CityInfluenceMap[x, y] = Mathf.PerlinNoise(x/10f + 0.1f + randomOffset, y/10f + 0.1f + randomOffset);
                var block = Instantiate(NeighborhoodPrefab, new Vector3(startX + BlockSize.x * x, 0, startY + BlockSize.y * y), Quaternion.Euler(0, 0, 0)) as GameObject;

                var neighborhood = block.GetComponent<Neighborhood>();
                neighborhood.FriendlyDealer = DealerManager.Dealers[1] ;
                neighborhood.DealerManager = DealerManager;
                var respects = new Dictionary<Guid, float>();
                foreach(var d in DealerManager.Dealers)
                {
                    respects.Add(d.ID, Random.Range(0.1f, 1f));
                }
                respects[DealerManager.Dealers[0].ID] = 0f;
                neighborhood.Respect = respects;
                neighborhood.Name = "Neighboorhood " + x.ToString() + " " + y.ToString();
                Neighborhoods.Add(neighborhood);
                block.transform.SetParent(transform);
            }
        }

        if (!Neighborhoods.Any(b => b.FriendlyDealer != null))
            Neighborhoods[Random.Range(0, Neighborhoods.Count)].FriendlyDealer = DealerManager.Dealers[1];
        var emptyBlocks = Neighborhoods.Where(n => n.FriendlyDealer == null).ToArray();
        if (emptyBlocks.Length == 0)
            emptyBlocks = Neighborhoods.ToArray();
        var startingBlock = emptyBlocks[Random.Range(0, emptyBlocks.Length)];
        startingBlock.FriendlyDealer = DealerManager.Dealers[0];
        startingBlock.Respect[DealerManager.Dealers[0].ID] = 0.5f;
        for(int i = 1; i < startingBlock.Respect.Count; i++)
        {
            startingBlock.Respect[DealerManager.Dealers[i].ID] = 0f;
        }
    }
}
