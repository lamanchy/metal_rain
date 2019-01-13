using System.Collections.Generic;
using Entities.Tile;
using UnityEditor;
using UnityEngine;

public class GenerateMap : MonoBehaviour {
    [SerializeField]
    private List<HexVisualsData> hexVisualsData;

    public int steepness = 100;
    public int size = 10;
    public GameObject tile;

    public void Generate() {
        var parent = GameObject.Find("/Tiles");
        if (parent == null) {
            parent = new GameObject("Tiles");
        }

        foreach (var ts in FindObjectsOfType<TileScript>()) {
            DestroyImmediate(ts.gameObject);
        }

        var sqrtSize = (int) Mathf.Sqrt(size);
        for (var x = -sqrtSize; x < sqrtSize; x++) {
            for (var y = -sqrtSize; y < sqrtSize; ++y) {
                var g = PrefabUtility.InstantiatePrefab(tile) as GameObject;
                g.transform.parent = parent.transform;

                var tileScript = g.GetComponent<TileScript>();
                var elevation = (Mathf.Pow(x - sqrtSize / 2, 2) + Mathf.Pow(y - sqrtSize / 2, 2)) / 100f;
                //elevation = 1;

                SetTileScriptData(tileScript, new Vector3Int(x, y, 0), elevation, elevation);
            }
        }
    }

    public void GenerateFromHeightMap(Texture2D source) {
        var parent = GameObject.Find("/Tiles");
        if (parent == null) {
            parent = new GameObject("Tiles");
        }

        foreach (var ts in FindObjectsOfType<TileScript>()) {
            DestroyImmediate(ts.gameObject);
        }

        var pixels = source.width * source.height;
        var pixelsPerTile = Mathf.FloorToInt(Mathf.Sqrt((float)pixels / size));

        Debug.Log(source.width);
        for (var x = 0; x < source.width / pixelsPerTile; x++) {
            for (var y = 0; y < source.height / pixelsPerTile; y++) {
                var newTile = PrefabUtility.InstantiatePrefab(tile) as GameObject;
                newTile.transform.parent = parent.transform;

                float elevation = 0;
                float sum = 0;
                for (var i = 0; i < pixelsPerTile; ++i) {
                    for (var o = 0; o < pixelsPerTile; ++o) {
                        if (x * pixelsPerTile + i >= source.width || y * pixelsPerTile + o >= source.height) {
                            continue;
                        }
                        // TODO Get second level from RGB channels
                        elevation += source.GetPixel(x * pixelsPerTile + i, y * pixelsPerTile + o).grayscale;
                        sum += 1;
                    }
                }

                elevation /= sum;
                elevation *= steepness;

                // TODO Calculate height for tiles on second layer read from GB channels
                SetTileScriptData(newTile.GetComponent<TileScript>(), new Vector3Int(x, y + x / 2, 0), elevation, elevation);
            }
        }
    }

    private void SetTileScriptData(TileScript tileScript, Vector3Int pos, float elevation, float height) {
        tileScript.Position = pos;
        tileScript.elevation = elevation;
        tileScript.height = height;
        
        for (var i = hexVisualsData.Count - 1; i >= 0; --i) {
            if (elevation < hexVisualsData[i].startHeight) {
                continue;
            }
            var randomVisual = hexVisualsData[i].items[Random.Range(0, hexVisualsData[i].items.Count)];
            tileScript.SetSideMaterial(randomVisual.sidesMaterial);
            tileScript.SetTopMaterial(randomVisual.topMaterial);

            break;
        }

        tileScript.MoveToPosition();
    }
}
