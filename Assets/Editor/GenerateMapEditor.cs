using System;
using System.Collections.Generic;
using System.IO;
using Entities.Tile;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateMap))]
public class GenerateMapEditor : Editor {
    private string mapName;
    public Texture2D source;

    public override void OnInspectorGUI() {
        var myScript = (GenerateMap) target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Map") && EditorUtility.DisplayDialog("Delete current",
                "This will delete ALL current tiles, make sure current map is saved",
                "Delete all and generate new",
                "No, go back, go back!")) {
            myScript.Generate();
        }

        source = EditorGUILayout.ObjectField("heightmap:", source, typeof(Texture2D), false) as Texture2D;
        if (GUILayout.Button("Load from heightmap:")) {
            myScript.GenerateFromHeightMap(source);
        }
    }

    private void LoadMap() {
        if (!File.Exists(GetMapPath())) {
            EditorUtility.DisplayDialog("Not found", "Map with such name was not found", "Ok");
            return;
        }
        var dataAsJson = File.ReadAllText(GetMapPath());
        var tiles = JsonUtility.FromJson<TileScript[]>(dataAsJson);
        foreach (var ts in FindObjectsOfType<TileScript>()) {
            DestroyImmediate(ts.gameObject);
        }
        Debug.Log(tiles);
        foreach (var ts in tiles) {
            Debug.Log(ts.gameObject);
            Instantiate(ts.gameObject);
            ts.AlignToGrid();
        }
    }

    private void SaveMap() {
        if (File.Exists(GetMapPath()) && !EditorUtility.DisplayDialog("Map exists",
                "Are you sure you want to overwrite it?",
                "Yes, do it!",
                "No, go back, go back!")) {
            return;
        }

        var dataAsJson = "";
        foreach (var ts in FindObjectsOfType<TileScript>()) {
            dataAsJson += JsonUtility.ToJson(ts);
        }
        Debug.Log(dataAsJson);
        File.WriteAllText(GetMapPath(), dataAsJson);
    }

    private string GetMapPath() => Path.Combine(Application.dataPath, "Maps", mapName + ".txt");

    [Serializable]
    public class GameData {
        public List<TileScript> tiles;
    }
}
