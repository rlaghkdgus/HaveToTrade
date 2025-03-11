using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventoryContainer))]
public class sEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InventoryContainer inven = (InventoryContainer)target;
        if (GUILayout.Button("Print Weight"))
        {
            inven.PrintStatus();
        }
    }
}
