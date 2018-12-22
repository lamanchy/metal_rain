using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public GameObject tile;
    public int size = 10;
    public void Generate()
    {
        foreach (TileScript ts in FindObjectsOfType<TileScript>())
        { 
            DestroyImmediate(ts.gameObject);
        }

        for (int x = -size; x < size; x++)
        {
            for (int y = -size; y < size; ++y)
            {
                GameObject g = Instantiate(tile, this.gameObject.transform);
                TileScript tileScript = g.GetComponent<TileScript>();
                float elevation = (Mathf.Pow(x - size/2, 2) + Mathf.Pow(y - size/2, 2)) / 100f;
                //elevation = 1;

                tileScript.pos = new Vector3Int(x, y, 0);
                tileScript.elevation = elevation;
                tileScript.height = elevation + 1;
                tileScript.MoveToPosition();
            }
        }
    }
}
