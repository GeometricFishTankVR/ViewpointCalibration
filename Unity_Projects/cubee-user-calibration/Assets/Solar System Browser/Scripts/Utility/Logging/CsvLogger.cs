using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Unity object describing a CSV file that can be written to.
/// </summary>
[CreateAssetMenu( menuName = "Runtime Logging/Comma Separated Values" )]
public class CsvLogger : ScriptableObject
// Author: Christopher Chamberlain - 2017
{
    [Header( "File Configuration" )]

    [Tooltip( "Enables prefixing the file name with a date string." )]
    public bool EnableDatePrefix = true;

    [Tooltip( "Configures how the date is prefixed using the C# DateTime.ToString string format." )]
    public string DatePrefixFormat = "yyyy.M.dd - HH.mm";

    [Tooltip( "The file name used when writing to disk." )]
    public string FileName = "record.csv";

    [Header( "Field Configuration" )]

    [Tooltip( "Enable automatic recording of time ( in milliseconds ) since object start as a field in the CSV file." )]
    public bool EnableElapsedTimeField = true;

    [Tooltip( "The allowed field names in the CSV file." )]
    public string[] FieldNames;

    /// <summary>
    /// The path to the file on disk.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// CSV Writer instance for writing a formatted CSV file.
    /// </summary>
    private CsvFileWriter CsvFileWriter;

    /// <summary>
    /// Flag determining if the <see cref="SetField(string, object)"/> method was called this frame.
    /// </summary>
    private bool WasModified = false;

    public const string ELAPSED_TIME_FIELD_NAME = "ElapsedTime";

    private int id = 0;

    /// <summary>
    /// The number of lines awaiting to be written to the disk.
    /// </summary>
    public int PendingLineCount { get { return CsvFileWriter.PendingLines; } }

    private void OnEnable()
    {
        // Ensure file has the CSV extension.
        FileName = Path.ChangeExtension( FileName, "csv" );

        // Construct path, prefixing date if enabled.
        FilePath = FileName;
        if( EnableDatePrefix )
            FilePath = PrefixDate( FilePath, DatePrefixFormat );

        // List of field names to populate the CSV writer.
        var fields = new List<string>();
        if( FieldNames != null )
            fields.AddRange( FieldNames );

        // If time recording is enabled, add to field list.
        if( EnableElapsedTimeField )
            fields.Add( ELAPSED_TIME_FIELD_NAME );

        fields.Add( "id" );

        // Construct the CSV writer object
        CsvFileWriter = new CsvFileWriter( FilePath, fields.ToArray() );
    }

    private void OnDisable()
    {
        // 
    }

    /// <summary>
    /// Writes the current field set / line to memory.
    /// </summary>
    public void CommitLine()
    {
        // 
        if( WasModified )
        {
            // If recording time, populate time field.
            if( EnableElapsedTimeField )
                CsvFileWriter[ELAPSED_TIME_FIELD_NAME] = GetTime();

            CsvFileWriter["id"] = id++;

            // Commit line, and clear write flag
            CsvFileWriter.CommitLine();
            WasModified = false;
        }
    }

    /// <summary>
    /// Writes the accumulated lines to disk.
    /// </summary>
    public void CommitChanges()
    {
        // 
        CsvFileWriter.CommitToFile();
    }

    /// <summary>
    /// Records information in a field to be written at the end of the current frame.
    /// </summary>
    /// <param name="name"> Valid field name. </param>
    /// <param name="data"> Some data to store in the given field. </param>
    public void SetField( string name, object data )
    {
        CsvFileWriter.SetField( name, data );
        WasModified = true;
    }

    /// <summary>
    /// Records information in a field to be written at the end of the current frame.
    /// </summary>
    /// <param name="name"> Valid field name. </param> 
    public object GetField( string name )
    {
        return CsvFileWriter.GetField( name );
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

    /// <summary>
    /// Gets the time since this object was started in milliseconds.
    /// </summary>
    private string GetTime()
    {
        //var time = (float) ( ( Sw.ElapsedTicks / (double) Stopwatch.Frequency ) * 1000.0 );
        var time = Time.time * 1000.0F;
        return time.ToString( "0.000" );
    }
}