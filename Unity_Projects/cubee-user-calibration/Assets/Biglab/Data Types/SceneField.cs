using System;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Biglab
{
    /// <summary>
    /// A special data type that represents a scene that can be used in builds. 
    /// The default SceneAsset only works within the editor.
    /// </summary>
    [System.Serializable]
    public class SceneField
    {
        [SerializeField]
        private Object sceneAsset;

        [SerializeField]
        private string sceneName = "";

        [SerializeField]
        private string sceneFile = "";

        /// <summary>
        /// The full path of the scene ( to be used with <see cref="UnityEngine.SceneManagement.SceneManager.LoadScene"/> ).
        /// </summary>
        public string Path
        {
            get { return sceneName; }
        }

        /// <summary>
        /// The full path of the scene ( to be used with <see cref="UnityEngine.SceneManagement.SceneManager.LoadScene"/> ).
        /// </summary>
        public string FilePath
        {
            get { return sceneFile; }
        }

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string Name
        {
            get
            {
                var idx = sceneName.LastIndexOf( "/" );
                if( idx >= 0 ) return sceneName.Substring( idx + 1 );
                else return sceneName;
            }
        }

        // Makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string( SceneField sceneField )
        {
            return sceneField.Path;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer( typeof( SceneField ) )]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginProperty( position, GUIContent.none, property );

            position.height = EditorGUIUtility.singleLineHeight;

            var sceneAsset = property.FindPropertyRelative( "sceneAsset" );
            var sceneName = property.FindPropertyRelative( "sceneName" );
            var sceneFile = property.FindPropertyRelative( "sceneFile" );

            position = EditorGUI.PrefixLabel( position, GUIUtility.GetControlID( FocusType.Passive ), label );

            if( sceneAsset != null )
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUI.ObjectField( position, sceneAsset.objectReferenceValue, typeof( SceneAsset ), false );

                if( EditorGUI.EndChangeCheck() )
                {
                    sceneAsset.objectReferenceValue = value;
                    if( sceneAsset.objectReferenceValue != null )
                    {
                        var scenePath = sceneFile.stringValue = AssetDatabase.GetAssetPath( sceneAsset.objectReferenceValue );
                        var assetsIndex = scenePath.IndexOf( "Assets", StringComparison.Ordinal ) + 7;
                        var extensionIndex = scenePath.LastIndexOf( ".unity", StringComparison.Ordinal );
                        scenePath = scenePath.Substring( assetsIndex, extensionIndex - assetsIndex );
                        sceneName.stringValue = scenePath;
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}