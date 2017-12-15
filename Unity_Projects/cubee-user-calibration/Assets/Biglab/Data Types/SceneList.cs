using Malee;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Biglab
{
    /// <summary>
    /// Provides a list of scenes populated from the inspector.
    /// </summary>
    [ExecuteInEditMode]
    public class SceneList : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        [SerializeField, Reorderable]
        private SceneAssetList m_SceneInfo;

        [System.Serializable]
        private class SceneAssetList : ReorderableArray<SceneInfo> { }

        /// <summary>
        /// The collection of scenes.
        /// </summary>
        public IEnumerable<SceneInfo> Scenes { get { return m_SceneInfo; } }

        private void OnValidate()
        {
#if UNITY_EDITOR
            var original = EditorBuildSettings.scenes;

            // Filter already added scenes
            var newScenes = new HashSet<string>( m_SceneInfo.Where( x => !string.IsNullOrEmpty( x.Scene.FilePath ) ).Select( x => x.Scene.FilePath ) );
            foreach( var editorScene in original )
                newScenes.Remove( editorScene.path );

            // Create an array to hold the existing scenes
            var newSettings = new List<EditorBuildSettingsScene>( original );

            // Append the new scenes
            foreach( var scene in newScenes )
            {
                var sceneToAdd = new EditorBuildSettingsScene( scene, true );
                newSettings.Add( sceneToAdd );
            }

            EditorBuildSettings.scenes = newSettings.ToArray();
#endif
        }

        [System.Serializable]
        public class SceneInfo
        {
            /// <summary>
            /// The alternate name given to the scene.
            /// </summary>
            public string Title
            {
                get
                {
                    if( string.IsNullOrEmpty( m_Title ) ) return Scene.Name;
                    else return m_Title;
                }
            }

            /// <summary>
            /// The scene asset.
            /// </summary>
            public SceneField Scene { get { return m_Scene; } }

            [SerializeField]
            private string m_Title;

            [SerializeField]
            private SceneField m_Scene;
        }
    }
}