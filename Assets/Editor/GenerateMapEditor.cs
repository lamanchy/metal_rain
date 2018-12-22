using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GenerateMap))]
public class GenerateMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateMap myScript = (GenerateMap)target;
        if (GUILayout.Button("Generate Map"))
        {
            myScript.Generate();
        }
    }
}