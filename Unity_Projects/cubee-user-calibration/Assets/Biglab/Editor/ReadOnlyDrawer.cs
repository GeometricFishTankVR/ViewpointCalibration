using UnityEditor;
using UnityEngine;

namespace Biglab
{
    [CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label, true );
        }

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            var readOnly = attribute as ReadOnlyAttribute;
            var oldState = GUI.enabled;

            var disable = ( readOnly.OnlyDisableWhenPlaying && Application.isPlaying ) || !readOnly.OnlyDisableWhenPlaying;

            GUI.enabled = !disable;
            EditorGUI.PropertyField( position, property, label, true );
            GUI.enabled = oldState;
        }
    }
}