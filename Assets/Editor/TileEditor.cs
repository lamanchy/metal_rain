using Entities.Tile;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileScript))]
public class TileEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var myScript = (TileScript) target;
        if (GUILayout.Button("Move to position")) {
            myScript.MoveToPosition();
        }
    }
}
