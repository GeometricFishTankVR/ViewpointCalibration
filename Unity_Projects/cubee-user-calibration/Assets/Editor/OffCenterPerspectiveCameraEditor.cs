using UnityEditor;

[CustomEditor(typeof(OffCenterPerspectiveCamera))]
public class OffCenterPerspectiveCameraEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawPropertiesExcluding(serializedObject, new string[] { "StaticNearPlane", "StaticFarPlane", "HasStaticNearPlane", "HasStaticFarPlane" });
        OffCenterPerspectiveCamera myScript = (OffCenterPerspectiveCamera)target;

        myScript.HasStaticNearPlane = EditorGUILayout.Toggle("Has static near plane", myScript.HasStaticNearPlane);

        EditorGUI.BeginDisabledGroup(!myScript.HasStaticNearPlane);
        myScript.StaticNearPlane = EditorGUILayout.FloatField("Static near plane:", myScript.StaticNearPlane);
        EditorGUI.EndDisabledGroup();

        

        myScript.HasStaticFarPlane = EditorGUILayout.Toggle("Has static far plane", myScript.HasStaticFarPlane);

        EditorGUI.BeginDisabledGroup(!myScript.HasStaticFarPlane);
        myScript.StaticFarPlane = EditorGUILayout.FloatField("Static far plane:", myScript.StaticFarPlane);
        EditorGUI.EndDisabledGroup();
    }
}
