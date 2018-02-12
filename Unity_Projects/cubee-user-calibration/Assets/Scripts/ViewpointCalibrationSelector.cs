using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ViewpointCalibrationSelector : MonoBehaviour {
    public Dropdown CalibrationDropdown;
    public string CalibrationDirectory;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsFalse(CalibrationDirectory == "", "Calibration directory cannot be empty. Please specify in the Editor.");
        Assert.IsTrue(Directory.Exists(CalibrationDirectory));
    }

    void Start () {
        CalibrationDropdown.ClearOptions();

        List<string> calibrationFilenames = GetListOfCalibrationFiles();
        CalibrationDropdown.AddOptions(new List<string>(calibrationFilenames));
    }
    #endregion

    public void OnButtonPress()
    {
        ChangeCalibration();
    }

    public void ChangeCalibration()
    {
        string filename = CalibrationDropdown.options[CalibrationDropdown.value].text;
        string filepath = Path.GetFullPath(Path.Combine(CalibrationDirectory, filename));
        PersistentProjectStorage.Instance.CurrentViewpointCalibration = ViewpointCalibration.LoadFromFile(filepath);
    }

    public List<string> GetListOfCalibrationFiles()
    {
        List<string> fileNames = new List<string>();
        DirectoryInfo di = new DirectoryInfo(CalibrationDirectory);
        FileSystemInfo[] files = di.GetFileSystemInfos();
        List<FileSystemInfo> finfos = files.Where(f => f.Name.Contains("_vc_"))
                                .OrderBy(f => f.LastWriteTime).Reverse()
                                .ToList();
        foreach (FileSystemInfo finfo in finfos)
        {
            fileNames.Add(finfo.Name);
        }
        return fileNames;
    }
}
