using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TileScript))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileScript myScript = (TileScript)target;
        if (GUILayout.Button("Move to position"))
        {
            myScript.MoveToPosition();
        }
    }
}