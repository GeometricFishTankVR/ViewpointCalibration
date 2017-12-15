using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnamorphosisScatterer))]
public class AnamorphosisScattererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AnamorphosisScatterer myScript = (AnamorphosisScatterer)target;
        if (GUILayout.Button("Scatter Children"))
        {
            myScript.ScatterChildren();
        }
        else if(GUILayout.Button("Reset Children"))
        {
            myScript.ResetChildren();
        }
    }
}
