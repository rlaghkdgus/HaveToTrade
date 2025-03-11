using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BackGroundScaler))]
public class BG_Scaler : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BackGroundScaler bgs = (BackGroundScaler)target;
        if (GUILayout.Button("ReSize"))
        {
            bgs.Resize();
        }
    }
}
