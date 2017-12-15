using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Component for Unity that automatically watches loaded CsvLogFiles and writes changes to disk.
/// </summary>
public class TableWriterHandler : MonoBehaviour
// Author: Christopher Chamberlain - 2017
{
    private TableWriter[] Writers;

    private void Start()
    {
        // Discover all loaded table writers
        Writers = Resources.FindObjectsOfTypeAll<TableWriter>();
    }

    private void LateUpdate()
    {
        // Updates changes made this frame 
        foreach( var writer in Writers )
            writer.Commit();
    }

    private void OnDestroy()
    {
        // Forces the commit of all writers
        foreach( var writer in Writers )
            writer.Commit( true );
    }

#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor( typeof( TableWriterHandler ) )]
    class TableWriterHandlerEditor : Editor
    {
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
            var writer = target as TableWriterHandler;
            if( writer == null ) return;

            var script = MonoScript.FromMonoBehaviour( writer );

            GUI.enabled = false;
            EditorGUILayout.ObjectField( "Script", script, typeof( MonoScript ), false );
            GUI.enabled = true;

            //
            if( Application.isPlaying )
            {
                EditorGUILayout.LabelField( string.Format( "Watching {0} Table Writers", writer.Writers.Length ) );

                //GUI.enabled = false;
                //foreach( var file in writer.Writers )
                //{
                //    EditorGUILayout.HelpBox( string.Format( "{0}: {1} Writes", file.FilePath, file.PendingLineCount ), MessageType.None );
                //}
                //GUI.enabled = true;
            }
            else EditorGUILayout.HelpBox( "Automatically watches loaded Table Writers", MessageType.Info );

            // 
            EditorUtility.SetDirty( target );
        }
    }

#endif
}