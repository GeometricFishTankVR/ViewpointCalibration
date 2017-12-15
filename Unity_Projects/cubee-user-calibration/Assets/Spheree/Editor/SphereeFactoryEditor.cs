using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SphereeFactory))]
public class SphereeFactoryEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SphereeFactory myScript = (SphereeFactory)target;
        if (GUILayout.Button("Create Projectors"))
        {
            Undo.RecordObject(target, "Loading from file.");
            myScript.LoadConfiguration();
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
    }
}
