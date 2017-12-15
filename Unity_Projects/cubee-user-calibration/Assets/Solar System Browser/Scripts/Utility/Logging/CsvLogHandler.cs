using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Component for Unity that automatically watches loaded CsvLogFiles and writes changes to disk.
/// </summary>
public class CsvLogHandler : MonoBehaviour
// Author: Christopher Chamberlain - 2017
{
    private CsvLogger[] CsvFiles;

    private void Start()
    {
        // Discover all loaded csv files
        CsvFiles = Resources.FindObjectsOfTypeAll<CsvLogger>();
    }

    private void LateUpdate()
    {
        // Updates changes made this frame 
        foreach( var file in CsvFiles )
        {
            // Commit current line/row
            file.CommitLine();

            // Flush every 1000 lines
            if( file.PendingLineCount > 1000 )
                file.CommitChanges();
        }
    }

    private void OnDestroy()
    {
        // Writes the in-memory representation to disk
        foreach( var file in CsvFiles )
        {
            // 
            if( file.PendingLineCount > 0 )
                file.CommitChanges();
        }
    }

#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor( typeof( CsvLogHandler ) )]
    class CsvLogHandlerEditor : Editor
    {
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
            var writer = target as CsvLogHandler;
            if( writer == null ) return;

            var script = MonoScript.FromMonoBehaviour( writer );

            GUI.enabled = false;
            EditorGUILayout.ObjectField( "Script", script, typeof( MonoScript ), false );
            GUI.enabled = true;

            //
            if( Application.isPlaying )
            {
                EditorGUILayout.LabelField( string.Format( "Watching {0} CSV Files", writer.CsvFiles.Length ) );

                GUI.enabled = false;
                foreach( var file in writer.CsvFiles )
                {
                    EditorGUILayout.HelpBox( string.Format( "{0}: {1} Writes", file.FilePath, file.PendingLineCount ), MessageType.None );
                }
                GUI.enabled = true;
            }
            else EditorGUILayout.HelpBox( "Automatically watches loaded CSV Files", MessageType.Info );

            // 
            EditorUtility.SetDirty( target );
        }
    }

#endif
}