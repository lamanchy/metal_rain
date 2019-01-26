using System;
using UnityEditor;
using UnityEngine;

namespace DigitalRuby.LightningBolt {
    [CustomEditor(typeof(LightningBoltScript))]
    public class LightningBoltEditor : UnityEditor.Editor {
        private Texture2D logo;

        public override void OnInspectorGUI() {
            if (logo == null) {
                var guids = AssetDatabase.FindAssets("LightningBoltLogo");
                foreach (var guid in guids) {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    logo = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
                    if (logo != null) {
                        break;
                    }
                }
            }
            if (logo != null) {
                const float maxLogoWidth = 430.0f;
                EditorGUILayout.Separator();
                var w = EditorGUIUtility.currentViewWidth;
                var r = new Rect();
                r.width = Math.Min(w - 40.0f, maxLogoWidth);
                r.height = r.width / 2.7f;
                var r2 = GUILayoutUtility.GetRect(r.width, r.height);
                r.x = (EditorGUIUtility.currentViewWidth - r.width) * 0.5f - 4.0f;
                r.y = r2.y;
                GUI.DrawTexture(r, logo, ScaleMode.StretchToFill);
                if (GUI.Button(r, "", new GUIStyle())) {
                    Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/34217?aid=1011lGnL");
                }
                EditorGUILayout.Separator();
            }

            DrawDefaultInspector();
        }
    }
}
