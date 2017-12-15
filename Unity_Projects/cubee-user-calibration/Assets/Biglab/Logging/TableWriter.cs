using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// An abstract class for writing to tables.
/// </summary>
public abstract class TableWriter : MonoBehaviour
// Author: Christopher Chamberlain - 2017
{
    private Dictionary<string, object> _Fields;
    private HashSet<string> _Names;

    /// <summary>
    /// The 
    /// </summary>
    public TableDescription TableDescription;

    protected IEnumerable<string> FieldNames { get { return _Names; } }

    void OnEnable()
    {
        // TODO: Somehow avoid this check when enabled in the editor...?
        // Application.isPlaying seems to be false.

        if( TableDescription == null )
            throw new ArgumentNullException( "Table writer must have a description" );

        // Validate each field name
        foreach( var name in TableDescription.Fields.Select( f => f.Name ) )
        {
            if( string.IsNullOrEmpty( name ) )
                throw new ArgumentException( "Field name must be non-null and non-empty." );
        }

        // 
        _Fields = new Dictionary<string, object>();
        _Names = new HashSet<string>( TableDescription.Fields.Select( x => x.Name ) );

        //
        EnableWriter();
    }

    /// <summary>
    /// Called when the object is enabled.
    /// Use this to create the lists, buffers, maps, etc.
    /// </summary>
    protected abstract void EnableWriter();

    /// <summary>
    /// Commits a row of information.
    /// </summary>
    /// <param name="row">The row of information.</param>
    /// <param name="force">Force flag, to commit all potentially buffered data to disk.</param>
    protected abstract void Commit( ICollection<KeyValuePair<string, object>> row, bool force );

    /// <summary>
    /// Commits the values in the field set to memory, and clears the field set.
    /// </summary>
    public void Commit( bool force = false )
    {
        if( _Fields.Count > 0 || force )
        {
            //Debug.Log( "Commit Writer" );
            //foreach( var name in _Names )
            //    Debug.LogFormat( "Field: '{0}'", name );

            Commit( _Fields, force );
            _Fields.Clear();
        }
    }

    /// <summary>
    /// Sets a field in the current line/row being written.
    /// </summary>
    /// <param name="name"> Some field name specified in the constructor. </param>
    /// <param name="value"> Some value to write. </param>
    public void SetField( string name, object value )
    {
        // 
        if( string.IsNullOrEmpty( name ) )
            throw new ArgumentException( "Field name must be non-null and non-empty." );

        // Record value
        if( _Names.Contains( name ) ) _Fields[name] = value;
        // Throw exception
        else throw new InvalidOperationException( string.Format( "Unable to set field '{0}', field name unknown.", name ) );
    }

    /// <summary>
    /// Sets a field in the current line/row being written.
    /// </summary>
    /// <param name="name"> Some field name specified in the constructor. </param>
    /// <returns> The value in the given field. </returns>
    public object GetField( string name )
    {
        // 
        if( string.IsNullOrEmpty( name ) )
            throw new ArgumentException( "Field name must be non-null and non-empty." );

        // Return value
        if( _Names.Contains( name ) )
        {
            if( _Fields.ContainsKey( name ) ) return _Fields[name];
            else return null;
        }
        // Throw exception
        else throw new InvalidOperationException( string.Format( "Unable to get field '{0}', field name unknown.", name ) );
    }

    private void LateUpdate()
    {
        // Updates changes made this frame.
        Commit();
    }

    private void OnDestroy()
    {
        // Forces the commit of content
        Commit( true );
    }

#if UNITY_EDITOR

    [CustomEditor( typeof( TableWriter ), true )]
    class TableWriterEditor : Editor
    {
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
            var writer = target as TableWriter;
            if( writer == null ) return;
            DrawDefaultInspector();

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField( "Table Description", EditorStyles.boldLabel );
            if( writer.TableDescription != null )
            {
                foreach( var f in writer.TableDescription.Fields )
                    EditorGUILayout.HelpBox( string.Format( "{0} ( {1} )", f.Name, f.Type ), MessageType.None );
            }
            else
            {
                EditorGUILayout.HelpBox( "Select a table description", MessageType.Error );
            }
        }
    }

#endif
}