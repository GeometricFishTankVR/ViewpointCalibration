using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor( typeof( TableDescription ) )]
public class TableDescriptionEditor : Editor
{
    private ReorderableList ReorderableList;

    private TableDescription Table
    {
        get { return target as TableDescription; }
    }

    private SerializedProperty Fields;

    private void OnEnable()
    {
        // 
        Fields = serializedObject.FindProperty( "Fields" );
        ReorderableList = new ReorderableList( serializedObject, Fields, true, true, true, true );

        // Add listeners to draw events
        ReorderableList.drawHeaderCallback += DrawHeader;
        ReorderableList.drawElementCallback += DrawElement;

        ReorderableList.onAddCallback += AddItem;
        ReorderableList.onRemoveCallback += RemoveItem;
    }

    private void OnDisable()
    {
        // Make sure we don't get memory leaks etc.
        ReorderableList.drawHeaderCallback -= DrawHeader;
        ReorderableList.drawElementCallback -= DrawElement;

        ReorderableList.onAddCallback -= AddItem;
        ReorderableList.onRemoveCallback -= RemoveItem;
    }

    private void DrawHeader( Rect rect )
    {
        GUI.Label( rect, "Columns" );
    }

    private void DrawElement( Rect rect, int index, bool active, bool focused )
    {
        var item = Table.Fields[index];

        EditorGUI.BeginChangeCheck();

        // 
        //rect.y += EditorGUIUtility.standardVerticalSpacing;

        // If you are using a custom PropertyDrawer, this is probably better
        EditorGUI.PropertyField( rect, Fields.GetArrayElementAtIndex( index ), true );

        if( EditorGUI.EndChangeCheck() )
            EditorUtility.SetDirty( target );
    }

    private void AddItem( ReorderableList list )
    {
        Table.Fields.Add( new TableDescription.Field() );
        EditorUtility.SetDirty( target );
    }

    private void RemoveItem( ReorderableList list )
    {
        Table.Fields.RemoveAt( list.index );
        EditorUtility.SetDirty( target );
    }

    public override void OnInspectorGUI()
    {
        // 
        serializedObject.UpdateIfDirtyOrScript();

        // DrawDefaultInspector();
        ReorderableList.DoLayoutList();

        // 
        serializedObject.ApplyModifiedProperties();
    }
}

