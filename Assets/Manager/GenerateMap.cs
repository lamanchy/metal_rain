using System;
using System.Collections.Generic;
using Entities.Tile;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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

                var levels = new Vector3();
                var sum = new Vector3Int();
                for (var i = 0; i < pixelsPerTile; ++i) {
                    for (var j = 0; j < pixelsPerTile; ++j) {
                        if (x * pixelsPerTile + i >= source.width || y * pixelsPerTile + j >= source.height) {
                            continue;
                        }

                        var pixel = source.GetPixel(x * pixelsPerTile + i, y * pixelsPerTile + j);
                        levels.x += pixel.r;
                        levels.y += pixel.g;
                        levels.z += pixel.b;

                        if (Math.Abs(pixel.r) > 0.01f) {
                            ++sum.x;
                        }

                        if (Math.Abs(pixel.g) > 0.01f) {
                            ++sum.y;
                        }

                        if (Math.Abs(pixel.b) > 0.01f) {
                            ++sum.z;
                        }
                    }
                }
                
                // Use max to avoid division by zero
                levels.x /= Math.Max(sum.x, 1);
                levels.y /= Math.Max(sum.y, 1);
                levels.z /= Math.Max(sum.z, 1);
                levels *= steepness;

                // If upper and lower levels are colliding, merge them
                if (levels.y - levels.x < 0.5f && levels.z > 0.5f) {
                    levels.x = levels.z;
                    levels.z = 0f;
                    levels.y = 0f;
                }

                // Create lower level
                if (Math.Abs(levels.x) > 0.5f) {
                    var firstLayer = PrefabUtility.InstantiatePrefab(tile) as GameObject;
                    firstLayer.transform.parent = parent.transform;
                    SetTileScriptData(firstLayer.GetComponent<TileScript>(), new Vector3Int(x, y + x / 2, 0), levels.x, levels.x);
                }
                
                // Create upper level
                if (Math.Abs(levels.y - levels.z) > 0.5f && Math.Abs(levels.y) > 0.5f && Math.Abs(levels.z) > 0.5f) {
                    var secondLayer = PrefabUtility.InstantiatePrefab(tile) as GameObject;
                    secondLayer.transform.parent = parent.transform;
                    SetTileScriptData(secondLayer.GetComponent<TileScript>(), new Vector3Int(x, y + x / 2, 1), levels.z, levels.z - levels.y);
                }
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
