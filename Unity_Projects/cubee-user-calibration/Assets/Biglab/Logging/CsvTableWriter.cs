using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Utility class for writing well-formatted CSV files.
/// </summary> 
public class CsvTableWriter : TableWriter
// Author: Christopher Chamberlain - 2017
{
    [Header( "File Configuration" )]

    [Tooltip( "Enables prefixing the file name with a date string." )]
    public bool EnableDatePrefix = true;

    [Tooltip( "Configures how the date is prefixed using the C# DateTime.ToString string format." )]
    public string DatePrefixFormat = "yyyy.M.dd - HH.mm";

    /// <summary>
    /// Path to the file being written.
    /// </summary>
    public string FilePath { get { return m_Path; } }

    [SerializeField]
    private string m_Path = "record.csv";

    private List<object[]> LineBuffer;

    // If the headers have been written already?
    private bool HasWrittenOnceAlready = false;

    /// <summary>
    /// The number of lines awaiting to be written to the disk.
    /// </summary>
    public int PendingLines { get { return LineBuffer.Count; } }

    private const int LINE_BUFFER_CAPACITY = 100;

    private const string DELIMITER = ",";

    protected override void EnableWriter()
    {
        HasWrittenOnceAlready = false;
        LineBuffer = new List<object[]>();
    }

    protected override void Commit( ICollection<KeyValuePair<string, object>> row, bool force )
    {
        var line = new List<object>();
        var dict = row.ToDictionary( x => x.Key );
        foreach( var key in dict.Keys )
        {
            // Have an entry
            if( dict.ContainsKey( key ) ) line.Add( dict[key].Value );
            // Do not have an entry
            else line.Add( string.Empty );
        }

        // Submit line set
        LineBuffer.Add( line.ToArray() );

        // If line buffer grows too large, append to file
        if( LineBuffer.Count > LINE_BUFFER_CAPACITY || force )
            WriteToFile();
    }

    /// <summary>
    /// Writes the current line buffer to a file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="delimiter"></param>
    private void WriteToFile()
    {
        // Ensure file has the CSV extension.
        var path = Path.ChangeExtension( FilePath, "csv" );

        // Construct path, prefixing date if enabled. 
        if( EnableDatePrefix )
            path = PrefixDate( path, DatePrefixFormat );

        // TODO: Validate path?
        if( string.IsNullOrEmpty( path ) )
            throw new ArgumentException( "Unale to write, file path must be non-null and non-empty" );

        var sb = new StringBuilder();

        Debug.LogFormat( "Appending to File '{0}'", path );
        //foreach( var name in FieldNames )
        //    Debug.LogFormat( "Field: '{0}'", name );

        // Write field names
        if( HasWrittenOnceAlready == false )
        {
            sb.AppendLine( CreateDelimitedString( FieldNames.ToArray(), DELIMITER ) );
        }

        // Write each line
        foreach( var line in LineBuffer )
            sb.AppendLine( CreateDelimitedString( line, DELIMITER ) );

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

    /// <summary>
    /// Prefixes the file name on the given path with the current date string.
    /// </summary>
    private static string PrefixDate( string path, string prefixFormat )
    {
        path = Path.GetFullPath( path );

        // Split path
        var dir = Path.GetDirectoryName( path );
        var name = Path.GetFileName( path );

        // 
        var prefix = string.Format( "{0} - ", DateTime.Now.ToString( prefixFormat ) );
        path = dir + Path.DirectorySeparatorChar + prefix + name;

        return path;
    }
}