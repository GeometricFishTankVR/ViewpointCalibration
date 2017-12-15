using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using System;

public class MyFileLogHandler : ILogHandler
{
    private FileStream m_FileStream;
    private StreamWriter m_StreamWriter;
    private ILogHandler m_DefaultLogHandler = Debug.logger.logHandler;

    public MyFileLogHandler(string fileFolder, string fileName)
    {
        string fullFileName = fileName + System.DateTime.Now.ToString("MMdd_HHmmss") + ".xml";
        Debug.Log(System.DateTime.Now.ToString("MMdd"));
        string filePath = Path.GetFullPath(fileFolder);
        filePath = Path.Combine(filePath, fullFileName);

        m_FileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        m_StreamWriter = new StreamWriter(m_FileStream);
    }

    public void replaceDefaultDebugLogger()
    {
        Debug.logger.logHandler = this;
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        m_StreamWriter.WriteLine(String.Format(format, args));
        m_StreamWriter.Flush();
        m_DefaultLogHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        m_DefaultLogHandler.LogException(exception, context);
    }
}

public class LogSystem : MonoBehaviour {

    public string logFileFolder;
    public string logFileName;

    public Transform ViewpointTransform;
    public Transform ClawTipTransform;

    private static ILogger logger = Debug.logger;
    private MyFileLogHandler myFileLogHandler;

    private bool isRecording = false;

    public bool IsRecording
    {
        get {
            return isRecording;
        }
        set {
            isRecording = value;
            if (isRecording)
            {
                StopCoroutine("recordData");
                StartCoroutine("recordData");
            }
            else 
            {
                StopCoroutine("recordData");
            }
        }
    }

    #region monobehaviour

    void OnEnable() {
        Assert.IsNotNull(ViewpointTransform, "ViewpointTransform can not be null. Please specify an object in the Editor.");
        Assert.IsNotNull(ClawTipTransform, "ClawTipTransform can not be null. Please specify an object in the Editor.");
        Assert.IsNotNull(logFileFolder, "logFileFolder can not be null. Please specify an object in the Editor.");
        Assert.IsNotNull(logFileName, "logFileName can not be null. Please specify an object in the Editor.");

        // LogSystem is initialized before all other objects
        myFileLogHandler = new MyFileLogHandler(logFileFolder, logFileName);
        myFileLogHandler.replaceDefaultDebugLogger();
        logger.Log(System.DateTime.Now + " Claw Selection User Study: <condition>Spheree</condition> Log. \r\n");
        logger.Log(String.Format("<t><time>{0}</time><event>LogSystem Initialized.</event></t>", Time.time.ToString("F3")));
    }

    #endregion

    IEnumerator recordData()
    {
        while (isRecording)
        {
            logger.Log(String.Format("<t><time>{0}</time>\r\n<viewpoint> <position>{1}</position> <rotation>{2}</rotation></viewpoint>\r\n<clawtip><position>{3}</position></clawtip></t>", 
                Time.time.ToString("F3"), 
                ViewpointTransform.position.ToString("F4"), 
                ViewpointTransform.rotation.ToString("F4"),
                ClawTipTransform.position.ToString("F4")));
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    
    public void resetLogFile()
    {
 
    }
}
