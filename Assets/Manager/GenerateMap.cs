using System.Collections.Generic;
using Entities.Tile;
using UnityEditor;
using UnityEngine;

public class GenerateMap : MonoBehaviour {
    [SerializeField]
    private List<HexVisualsData> hexVisualsData;

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

        var sqrt_size = (int) Mathf.Sqrt(size);
        for (var x = -sqrt_size; x < sqrt_size; x++) {
            for (var y = -sqrt_size; y < sqrt_size; ++y) {
                var g = PrefabUtility.InstantiatePrefab(tile) as GameObject;
                g.transform.parent = parent.transform;

                var tileScript = g.GetComponent<TileScript>();
                var elevation = (Mathf.Pow(x - sqrt_size / 2, 2) + Mathf.Pow(y - sqrt_size / 2, 2)) / 100f;
                //elevation = 1;

                SetTileScriptData(tileScript, new Vector3Int(x, y, 0), elevation);
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
        var pixelsPerTile = Mathf.FloorToInt(Mathf.Sqrt(pixels / size));

        Debug.Log(source.width);
        for (var x = 0; x < source.width / pixelsPerTile; x++) {
            for (var y = 0; y < source.height / pixelsPerTile; y++) {
                var g = PrefabUtility.InstantiatePrefab(tile) as GameObject;
                g.transform.parent = parent.transform;
                var tileScript = g.GetComponent<TileScript>();

                float elevation = 0;
                float sum = 0;
                for (var i = 0; i < pixelsPerTile; ++i) {
                    for (var o = 0; o < pixelsPerTile; ++o) {
                        if (x * pixelsPerTile + i < source.width && y * pixelsPerTile + o < source.height) {
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

    private void SetTileScriptData(TileScript tileScript, Vector3Int pos, float elevation) {
        tileScript.Position = pos;
        tileScript.elevation = elevation;
        tileScript.height = elevation + 1;

        for (var i = hexVisualsData.Count - 1; i >= 0; --i) {
            if (!(elevation >= hexVisualsData[i].startHeight)) {
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
