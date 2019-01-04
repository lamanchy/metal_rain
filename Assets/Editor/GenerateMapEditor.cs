using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(GenerateMap))]
public class GenerateMapEditor : Editor
{
    public Texture2D source;
    public override void OnInspectorGUI()
    {
        GenerateMap myScript = (GenerateMap)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Map") && EditorUtility.DisplayDialog(
            "Delete current",
            "This will delete ALL current tiles, make sure current map is saved",
            "Delete all and generate new",
            "No, go back, go back!"))
        {
            myScript.Generate();
        }

        source = EditorGUILayout.ObjectField("heightmap:", source, typeof(Texture2D), false) as Texture2D;
        if (GUILayout.Button("Load from heightmap:"))
        {
            myScript.GenerateFromHeightMap(source);
        }
    }




    string mapName;
    private void LoadMap()
    {
        if (File.Exists(GetMapPath()))
        {
            string dataAsJson = File.ReadAllText(GetMapPath());
            TileScript[] tiles = JsonUtility.FromJson<TileScript[]>(dataAsJson);
            foreach (TileScript ts in FindObjectsOfType<TileScript>())
            {
                DestroyImmediate(ts.gameObject);
            }
            Debug.Log(tiles);
            foreach (TileScript ts in tiles)
            {
                Debug.Log(ts.gameObject);
                Instantiate(ts.gameObject);
                ts.MoveToPosition();
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Not found", "Map with such name was not found", "Ok");
        }

        
    }

    private void SaveMap()
    {
        if (File.Exists(GetMapPath()) && !EditorUtility.DisplayDialog(
            "Map exists",
            "Are you sure you want to overwrite it?",
            "Yes, do it!",
            "No, go back, go back!"))
        {
            return;
        }


        string dataAsJson = ""; 
        foreach (TileScript ts in FindObjectsOfType<TileScript>())
        {
            dataAsJson += JsonUtility.ToJson(ts) ;
        }
        Debug.Log(dataAsJson);
        File.WriteAllText(GetMapPath(), dataAsJson);
    }

    private string GetMapPath()
    {
        return Path.Combine(Application.dataPath, "Maps", mapName + ".txt");
    }

    [System.Serializable]
    public class GameData
    {
        public List<TileScript> tiles;
    }
}