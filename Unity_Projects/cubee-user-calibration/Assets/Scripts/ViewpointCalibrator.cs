using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ViewpointCalibrator : MonoBehaviour {
    public int NumberOfSamples;
    public float MinimumCalibrationDistance;
    public float MaximumCalibrationDistance;
    public Transform DisplayCenter;
    public Transform Display;
    public GameObject Pattern;
    public GameObject IdleAnimation;
    public Text StatusText;
    public Transform HeadInTracker;
    public Transform ViewInDisplay;
    public int RandomSeed = 4;

    public string DisplayName;
    public string CalibrationDirectory = "CalibrationFiles";
    public string ViewpointSolverFilePath;


    private List<Vector3> GeneratedCalibrationPositions;
    private List<Vector3> calibrationPositions;
    private List<Vector3> trackedPositions;
    private List<Quaternion> trackedRotations; // Quaternion
    private Vector3 currentCalibrationPosition;
    private ViewpointCalibration resultingCalibration;

    private const string calibrationPositionsFileName = "CalibrationPositions.txt";
    private const string trackerPositionsFileName = "TrackerPositions.txt";
    private const string trackerRotationsFileName = "TrackerRotations.txt";
    private const string resultsFileName = "Results.txt";
    private string resultsFilePath;

    private bool xboxRBDepressed = false;
    private bool xboxLBDepressed = false;
    private bool isSolverRunning = false;
    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(DisplayCenter, "Display origin cannot be null. Please specify in the Editor.");
        Assert.IsNotNull(Pattern, "Pattern cannot be null. Please specify in the Editor.");
        Assert.IsNotNull(HeadInTracker, "Head Position in Tracker cannot be null. Please specify in the Editor.");
        Assert.IsNotNull(ViewInDisplay, "View cannot be null. Please specify in the Editor.");
        Assert.IsFalse(DisplayName == "", "Display name cannot be empty. Please specify in the Editor.");
        Assert.IsTrue(NumberOfSamples > 3, "The number of samples must be greater than 3.");
        Assert.IsTrue(MinimumCalibrationDistance > 0, "The minimum calibration distance must be greater than 0.");
        Assert.IsTrue(MaximumCalibrationDistance > 0, "The maximum calibration distance must be greater than 0.");
        Assert.IsTrue(MaximumCalibrationDistance > MinimumCalibrationDistance, "The maximum calibration distance must be greater than the minimum calibration distance.");
    }

    void Start() {
        resultingCalibration = null;
        GeneratedCalibrationPositions = new List<Vector3> { Vector3.down };
        currentCalibrationPosition = GeneratedCalibrationPositions[0];
        Pattern.SetActive(false);
        IdleAnimation.SetActive(true);
        StatusText.text = "Waiting for user input.\nCalibration can be started by pressing\nthe Menu (Start) button on the Xbox One controller.\nUse the A button to record a position and go forward.\n Use the B button to go back.";
    }

    void Update()
    {
        if(Input.GetButtonDown("XboxOneLB"))
        {
            xboxLBDepressed = true;
        }
        if(Input.GetButtonDown("XboxOneRB"))
        {
            xboxRBDepressed = true;
        }
        if(Input.GetButtonUp("XboxOneLB"))
        {
            xboxLBDepressed = false;
        }
        if(Input.GetButtonUp("XboxOneRB"))
        {
            xboxRBDepressed = false;
        }
        if (Input.GetButtonUp("XboxOneMenu(Start)"))
        {
            StartCalibration();
        }
        else if (Input.GetButtonDown("XboxOneLB") )
        {
            FinishCalibration();
        }
        if (Input.GetButtonUp("XboxOneA"))
        {
            RecordCurrentAndGoToNext();
        }
        else if (Input.GetButtonUp("XboxOneB"))
        {
            DeleteCurrentAndGoToPrev();
        }
        if (Pattern.activeSelf)
        {
            Pattern.transform.rotation = Quaternion.LookRotation(-currentCalibrationPosition);
            Pattern.transform.position = DisplayCenter.position;
            ViewInDisplay.localPosition = Vector3.Lerp(ViewInDisplay.localPosition, currentCalibrationPosition, Time.deltaTime * 20);
        }
        if (StatusText.text == "" &&
            trackedPositions.Count == NumberOfSamples &&
            trackedRotations.Count == NumberOfSamples &&
            calibrationPositions.Count == NumberOfSamples)
        {
            StatusText.text = "Depress LB and RB to start the calibration solver.\nIt will take the solver a few moments to complete.";
        }
        if(resultingCalibration != null)
        {
            StatusText.text = "Solver has completed the calibration.\nError: " + resultingCalibration.Error + "\nYou may start a new calibration by pressing the Menu (Start) button.";
        }
    }
    #endregion

    public void StartCalibration()
    {
        Pattern.SetActive(true);
        IdleAnimation.SetActive(false);
        StatusText.text = "";
        resultingCalibration = null;
        GeneratedCalibrationPositions = GenerateCalibrationPositions(RandomSeed);
        GeneratedCalibrationPositions.Add(Vector3.down);
        currentCalibrationPosition = GeneratedCalibrationPositions[0];
        trackedPositions = new List<Vector3>(NumberOfSamples);
        trackedRotations = new List<Quaternion>(NumberOfSamples);
        calibrationPositions = new List<Vector3>(NumberOfSamples);

    }

    public Vector3 NextRay(Vector3 current, out bool moved)
    {
        int index = GeneratedCalibrationPositions.IndexOf(current);

        if(index < GeneratedCalibrationPositions.Count - 1)
        {
            moved = true;
            return GeneratedCalibrationPositions[index + 1];
        }
        else
        {
            moved = false;
            return current;
        }
    }

    public Vector3 PrevRay(Vector3 current, out bool moved)
    {
        int index = GeneratedCalibrationPositions.IndexOf(current);

        if(index >= 1)
        {
            moved = true;
            return GeneratedCalibrationPositions[index - 1];
        }
        else
        {
            moved = false;
            return current;
        }
    }

    public void RecordCurrentAndGoToNext()
    {
        // Record data
        Vector3 headPosition = HeadInTracker.position;
        Quaternion headRotation = HeadInTracker.rotation;
        Vector3 calibrationPosition = currentCalibrationPosition;

        bool moved;
        Vector3 nextCalibrationRayDirection = NextRay(currentCalibrationPosition, out moved);
        if(moved)
        {
            trackedPositions.Add(headPosition);
            trackedRotations.Add(headRotation);
            calibrationPositions.Add(calibrationPosition);
            currentCalibrationPosition = nextCalibrationRayDirection;
        }
    }

    public void DeleteCurrentAndGoToPrev()
    {
        bool moved;
        Vector3 prevCalibrationRayDirection = PrevRay(currentCalibrationPosition, out moved);
        if (moved)
        {
            trackedPositions.RemoveAt(trackedPositions.Count - 1);
            trackedRotations.RemoveAt(trackedRotations.Count - 1);
            calibrationPositions.RemoveAt(calibrationPositions.Count - 1);
            currentCalibrationPosition = prevCalibrationRayDirection;
        }
    }

    public void FinishCalibration()
    {
        // Solve the problem
        if (trackedPositions.Count == NumberOfSamples && 
            trackedRotations.Count == NumberOfSamples &&
            calibrationPositions.Count == NumberOfSamples)
        {
            Pattern.SetActive(false);
            IdleAnimation.SetActive(true);
            StatusText.text = "Please wait while the solver crunches some numbers...";
            RunCalibrationSolution();
        }
    }

    public void WriteVector3ListToFile(string filepath, List<Vector3> data)
    {
        StreamWriter writer = new StreamWriter(filepath, false);
        foreach(Vector3 element in data)
        {
            writer.WriteLine(element.ToString("G4"));
        }
        writer.Close();
    }

    public void WriteQuaternionListToFile(string filepath, List<Quaternion> data)
    {
        StreamWriter writer = new StreamWriter(filepath, false);
        foreach (Quaternion element in data)
        {
            writer.WriteLine(element.ToString("G4"));
        }
        writer.Close();
    }

    private void matlabProcess_Exited(object sender, System.EventArgs e)
    {
        Matrix4x4 XTD;
        Vector3 oV;
        float oVe;
        Quaternion qHV;

        readResultsFile(out XTD, out oV, out oVe, out qHV);


        resultingCalibration = new ViewpointCalibration();
        resultingCalibration.cD = calibrationPositions;
        resultingCalibration.hT = trackedPositions;
        resultingCalibration.RHTn = trackedRotations;
        resultingCalibration.XTD = XTD;
        resultingCalibration.oV = oV;
        resultingCalibration.qVH = Quaternion.Inverse(qHV);
        resultingCalibration.qTD = CubeeMath.ExtractRotationFromMatrix(ref XTD);
        resultingCalibration.Error = oVe;
        resultingCalibration.WasTrackedWith6DoF = PersistentProjectStorage.Instance.IsHeadTracker6DoF;
        
        System.DateTime timeCompleted = System.DateTime.Now;
        string filename = string.Format(DisplayName + "_vc_{0:yyyy-MM-dd_hh-mm-ss-tt}.xml", timeCompleted);
        string filePath = Path.GetFullPath(Path.Combine(CalibrationDirectory, filename));
        ViewpointCalibration.SaveToFile(resultingCalibration, filePath);
        isSolverRunning = false;
    }

    public void RunCalibrationSolution()
    {
        string tempPath = CalibrationDirectory;//Path.GetTempPath();
        UnityEngine.Debug.Log("Using temporary directory: " + tempPath);

        // Delete the old results file if it exists
        resultsFilePath = Path.GetFullPath(Path.Combine(tempPath, resultsFileName));
        File.Delete(resultsFilePath); // Delete the results file if it exists

        string calibrationPositionsFilePath = Path.GetFullPath(Path.Combine(tempPath, calibrationPositionsFileName));
        string trackerPositionsFilePath = Path.GetFullPath(Path.Combine(tempPath, trackerPositionsFileName));
        string trackerRotationsFilePath = Path.GetFullPath(Path.Combine(tempPath, trackerRotationsFileName));

        // Write the files
        WriteVector3ListToFile(calibrationPositionsFilePath, calibrationPositions);
        WriteVector3ListToFile(trackerPositionsFilePath, trackedPositions);
        WriteQuaternionListToFile(trackerRotationsFilePath, trackedRotations);
        
        float scaleFactor = PersistentProjectStorage.Instance.HeadTrackerToDisplayScaleFactor;
        bool isSixDoF = PersistentProjectStorage.Instance.IsHeadTracker6DoF;
        isSixDoF = false;

        string arguments = isSixDoF.ToString() + ' ' +
                           scaleFactor.ToString() + ' ' +
                           resultsFilePath + ' ' +
                           calibrationPositionsFilePath + ' ' +
                           trackerPositionsFilePath + ' ' +
                           trackerRotationsFilePath;
        isSolverRunning = true;
        Process matlabProcess = new Process();
        matlabProcess.StartInfo.CreateNoWindow = true;
        matlabProcess.StartInfo.WorkingDirectory = tempPath;
        matlabProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        matlabProcess.StartInfo.FileName = ViewpointSolverFilePath.Replace('\\', Path.DirectorySeparatorChar);
        matlabProcess.StartInfo.Arguments = arguments;
        matlabProcess.EnableRaisingEvents = true;
        matlabProcess.Exited += new System.EventHandler(matlabProcess_Exited);
        matlabProcess.Start();
    }

    private void readResultsFile(out Matrix4x4 transformation, out Vector3 offset, out float error, out Quaternion rotation)
    {
        Matrix4x4 t = Matrix4x4.identity;
        Vector3 o = Vector3.zero;
        float e = 0;
        Quaternion q = Quaternion.identity;

        StreamReader reader = new StreamReader(resultsFilePath);
        string results = reader.ReadToEnd();
        
        string[] lines = results.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        // First 4 lines are the rows of the transformation matrix
        for(int i = 0; i < 4; i++)
        {
            string[] elements = lines[i].Split(' ');
            float elm1 = float.Parse(elements[0]);
            float elm2 = float.Parse(elements[1]);
            float elm3 = float.Parse(elements[2]);
            float elm4 = float.Parse(elements[3]);
            t.SetRow(i, new Vector4(elm1, elm2, elm3, elm4));
        }

        // 5th line is the offset (3 floats: Vector3)
        string[] off = lines[4].Split(' ');
        o.x = float.Parse(off[0]);
        o.y = float.Parse(off[1]);
        o.z = float.Parse(off[2]);

        // 6th line is the rotation (4 floats: Quaternion) 
        string[] quat = lines[5].Split(' ');
        if (quat[0] == "NaN" || quat[1] == "NaN" || quat[2] == "NaN" || quat[3] == "NaN" )
        {
            q.x = 1;
            q.y = 0;
            q.z = 0;
            q.w = 0;
        } else
        {

            q.x = float.Parse(quat[0]);
            q.y = float.Parse(quat[1]);
            q.z = float.Parse(quat[2]);
            q.w = float.Parse(quat[3]);
        }

        // last line is the error
        e = float.Parse(lines[6]);

        transformation = t;
        offset = o;
        error = e;
        rotation = q;
    }

    public List<Vector3> GenerateCalibrationPositions(int seed)
    {
        Random.InitState(seed);

        List<float> theta = new List<float>(NumberOfSamples);
        List<float> phi = new List<float>(NumberOfSamples);
        List<float> radius = new List<float>(NumberOfSamples);

        int samplesPerQuadrant = NumberOfSamples / 4;
        for (int i = 0; i < NumberOfSamples; i++)
        {
            int quadrant = i / samplesPerQuadrant;
            // Generate restricted random spherical coordinates
            theta.Add((((i % samplesPerQuadrant) + 1.0f) / samplesPerQuadrant) * 0.4f + 0.4f + (quadrant * Mathf.PI / 2));
            phi.Add(Random.Range(Mathf.PI / 2.5f, Mathf.PI / 6));
            radius.Add(Random.Range(MinimumCalibrationDistance, MaximumCalibrationDistance));
        }

        // Sort theta coordinate
        theta.Sort();

        // Convert to cartesian and add to list
        List<Vector3> positions = new List<Vector3>(NumberOfSamples);
        for (int i = 0; i < NumberOfSamples; i++)
        {
            Vector3 position = new Vector3(Mathf.Cos(theta[i]) * Mathf.Sin(phi[i]), Mathf.Cos(phi[i]), Mathf.Sin(theta[i]) * Mathf.Sin(phi[i])) * radius[i];
            positions.Add(position);
        }
        return positions;
    }
}
