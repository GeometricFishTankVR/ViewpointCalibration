using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer( typeof( TableDescription.Field ) )]
public class TableDescription_FieldDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI( Rect r, SerializedProperty property, GUIContent label )
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty( r, label, property );

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var nameRect = new Rect( r.x, r.y, r.width / 2 - 2, EditorGUIUtility.singleLineHeight );
        var typeRect = new Rect( r.x + r.width / 2 + 2, r.y, r.width / 2 - 2, EditorGUIUtility.singleLineHeight );

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField( nameRect, property.FindPropertyRelative( "Name" ), GUIContent.none );
        EditorGUI.PropertyField( typeRect, property.FindPropertyRelative( "Type" ), GUIContent.none );

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
