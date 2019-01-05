using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateMap : MonoBehaviour
{
    public GameObject tile;
    public int size = 10;

    [SerializeField]
    private List<HexVisualsData> hexVisualsData;

    public void Generate()
    {
        GameObject parent = GameObject.Find("/Tiles");
        if (parent == null) parent = new GameObject("Tiles");

        foreach (TileScript ts in FindObjectsOfType<TileScript>())
        {
            DestroyImmediate(ts.gameObject);
        }

        int sqrt_size = (int)Mathf.Sqrt(size);
        for (int x = -sqrt_size; x < sqrt_size; x++)
        {
            for (int y = -sqrt_size; y < sqrt_size; ++y)
            {
                GameObject g = PrefabUtility.InstantiatePrefab(tile) as GameObject;
                g.transform.parent = parent.transform;

                TileScript tileScript = g.GetComponent<TileScript>();
                float elevation = (Mathf.Pow(x - sqrt_size / 2, 2) + Mathf.Pow(y - sqrt_size / 2, 2)) / 100f;
                //elevation = 1;

                SetTileScriptData(tileScript, new Vector3Int(x, y, 0), elevation);
            }
        }
    }

    public void GenerateFromHeightMap(Texture2D source)
    {
        GameObject parent = GameObject.Find("/Tiles");
        if (parent == null) parent = new GameObject("Tiles");

        foreach (TileScript ts in FindObjectsOfType<TileScript>())
        {
            DestroyImmediate(ts.gameObject);
        }
        
        int pixels = source.width * source.height;
        int pixelsPerTile = Mathf.FloorToInt(Mathf.Sqrt(pixels / size));
        
        Debug.Log(source.width);
        for (int x = 0; x < source.width / pixelsPerTile; x++)
        {
            for (int y = 0; y < source.height / pixelsPerTile; y++)
            {
                GameObject g = PrefabUtility.InstantiatePrefab(tile) as GameObject;
                g.transform.parent = parent.transform;
                TileScript tileScript = g.GetComponent<TileScript>();

                float elevation = 0;
                float sum = 0;
                for (int i = 0; i < pixelsPerTile; ++i)
                {
                    for (int o = 0; o < pixelsPerTile; ++o)
                    {
                        if (x * pixelsPerTile + i < source.width && y * pixelsPerTile + o < source.height)
                        {
                            elevation += source.GetPixel(x * pixelsPerTile + i, y * pixelsPerTile + o).grayscale;
                            sum += 1;
                        }
                    }
                }
                
                elevation /= sum;
                elevation *= 100f;
                elevation -= 70f;

                SetTileScriptData(tileScript, new Vector3Int(x, y + x / 2, 0), elevation);
            }
        }
    }

    private void SetTileScriptData(TileScript tileScript, Vector3Int pos, float elevation)
    {
        tileScript.pos = pos;
        tileScript.elevation = elevation;
        tileScript.height = elevation + 1;

        for(int i = hexVisualsData.Count - 1; i >= 0; --i)
        {
            if(elevation >= hexVisualsData[i].startHeight)
            {
                var randomVisual = hexVisualsData[i].items[Random.Range(0, hexVisualsData[i].items.Count)];
                tileScript.SetSideMaterial(randomVisual.sidesMaterial);
                tileScript.SetTopMaterial(randomVisual.topMaterial);

                break;
            }
        }

        tileScript.MoveToPosition();
    }
}
