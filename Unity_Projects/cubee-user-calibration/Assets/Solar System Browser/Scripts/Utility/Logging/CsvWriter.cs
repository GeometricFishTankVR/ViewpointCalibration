using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Utility class for writing well-formatted CSV files.
/// </summary>
public class CsvFileWriter
// Author: Christopher Chamberlain - 2017
{
    /// <summary>
    /// Path to the file being written.
    /// </summary>
    public string Path { get; private set; }

    private List<object[]> LineBuffer;
    private Dictionary<string, object> CurrentFields;
    private HashSet<string> FieldNames;

    // If the headers have been written already?
    private bool HasWrittenOnceAlready = false;

    /// <summary>
    /// The number of lines awaiting to be written to the disk.
    /// </summary>
    public int PendingLines { get { return LineBuffer.Count; } }

    /// <summary>
    /// Constructs a new CSV writer object.
    /// </summary>
    /// <param name="path"> The name of the file to write to. </param>
    /// <param name="fieldNames"> The accepted field names. </param>
    public CsvFileWriter( string path, IEnumerable<string> fieldNames )
    {
        // 
        if( string.IsNullOrEmpty( path ) )
            throw new ArgumentException( "File path must be non-null and non-empty" );

        // TODO: Validate path?
        //if( !Uri.IsWellFormedUriString( path, UriKind.RelativeOrAbsolute ) )
        //    throw new ArgumentException( "File path was not a well-formed URI path" );

        // 
        Path = path;

        // Validate each field name
        foreach( var name in fieldNames )
        {
            if( string.IsNullOrEmpty( name ) )
                throw new ArgumentException( "Field name must be non-null and non-empty." );
        }

        // Create field name set
        FieldNames = new HashSet<string>( fieldNames );

        // 
        CurrentFields = new Dictionary<string, object>();
        LineBuffer = new List<object[]>();
    }

    /// <summary>
    /// Gets or Sets a named field.
    /// </summary>
    /// <param name="name"> Some field name specified in the constructor. </param>
    /// <returns> The value in the given field. </returns>
    public object this[string name]
    {
        set { SetField( name, value ); }
        get { return GetField( name ); }
    }

    /// <summary>
    /// Writes commited lines to the file ( First write will truncate the file, subsequent writes append the file ).
    /// </summary>
    public void CommitToFile()
    {
        WriteToFile( Path, "," );
    }

    /// <summary>
    /// Commits the values in the field set to memory, and clears the field set.
    /// </summary>
    public void CommitLine()
    {
        // 
        var slots = new List<object>();
        foreach( var key in FieldNames )
        {
            // Have an entry
            if( CurrentFields.ContainsKey( key ) ) slots.Add( CurrentFields[key] );
            // Do not have an entry
            else slots.Add( string.Empty );
        }

        // Submit line set
        LineBuffer.Add( slots.ToArray() );
        CurrentFields.Clear();
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
        if( FieldNames.Contains( name ) ) CurrentFields[name] = value;
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
        if( FieldNames.Contains( name ) )
        {
            if( CurrentFields.ContainsKey( name ) ) return CurrentFields[name];
            else return null;
        }
        // Throw exception
        else throw new InvalidOperationException( string.Format( "Unable to get field '{0}', field name unknown.", name ) );
    }

    /// <summary>
    /// Writes the current line buffer to a file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="delimiter"></param>
    private void WriteToFile( string path, string delimiter )
    {
        var sb = new StringBuilder();

        // Write field names
        if( HasWrittenOnceAlready == false )
            sb.AppendLine( CreateDelimitedString( FieldNames.ToArray(), delimiter ) );

        // Write each line
        foreach( var line in LineBuffer )
            sb.AppendLine( CreateDelimitedString( line, delimiter ) );

        // Write content to the file
        if( HasWrittenOnceAlready ) File.AppendAllText( path, sb.ToString() );
        else
        {
            File.WriteAllText( path, sb.ToString() );
            HasWrittenOnceAlready = true;
        }

        // 
        LineBuffer.Clear();
    }

    /// <summary>
    /// Creates a delimited string where each entry is quoted separated by a delimiter.
    /// </summary>
    private string CreateDelimitedString( object[] objects, string delimiter )
    {
        // 
        if( objects == null ) throw new ArgumentNullException();
        if( delimiter == null ) throw new ArgumentNullException();

        //
        var fields = objects.Select( o => Stringify( o ) );
        return string.Join( delimiter, fields.ToArray() );
    }

    /// <summary>
    /// Wraps the given object string representation in quotes and escapes existing quotes.
    /// </summary>
    private string Stringify( object obj )
    {
        // Null, return empty
        if( obj == null ) return string.Empty;
        // Wrap string representation
        else
        {
            var msg = obj.ToString();
            msg = msg.Replace( "\"", "\"\"" );
            return string.Format( "\"{0}\"", msg );
        }
    }
}