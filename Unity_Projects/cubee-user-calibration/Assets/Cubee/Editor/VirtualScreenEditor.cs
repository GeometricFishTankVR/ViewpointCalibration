using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VirtualScreen))]
public class VirtualScreenEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VirtualScreen myScript = (VirtualScreen)target;
        if (GUILayout.Button("Save Transformation"))
        {
            myScript.SaveTransformation();
        }
        else if (GUILayout.Button("Load Transformation"))
        {
            Undo.RecordObject(target, "Loading from file.");
            myScript.LoadTransformation();
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
        else if(GUILayout.Button("Load Identity"))
        {
            Undo.RecordObject(target, "Loading identity matrix.");
            myScript.LoadIdentity();
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
        else if(GUILayout.Button("Update Screen Orientation"))
        {
            Undo.RecordObject(target, "Updating screen orientations.");
            myScript.UpdateScreenOrientation();
            if (!Application.isEditor)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }
}
