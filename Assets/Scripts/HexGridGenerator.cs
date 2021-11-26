using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{

    [SerializeField] private int mapLength;
    [SerializeField] private int mapWidth;

    [SerializeField] private float heightTreshold = 0;

    [SerializeField] private List<GameObject> tiles = new List<GameObject>();

    private float tileOffset = 0;

    private void Awake()
    {
        if (tiles.Count <= 0) return; 
        
        for (int i = 0; i < mapLength; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                tileOffset = i % 2;

                float xNoise = 1 / ((float)j + 1);
                float yNoise = 1 / ((float)i + 1);
                float noiseSample = Mathf.PerlinNoise(xNoise * 50, yNoise * 50);

                float roundedHeight = Mathf.Ceil(noiseSample * 100) - 10;

                int tileIndex = 0;

                if (noiseSample > heightTreshold) tileIndex = 1;
                else tileIndex = 0;

                Instantiate(tiles[tileIndex], new Vector3(i * 1.5f, roundedHeight, j * 2 + tileOffset), Quaternion.identity);
            }
        }
    }
}
