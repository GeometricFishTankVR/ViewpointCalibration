using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot]
public class trackerCalibraitonData
{
    [XmlArray]
    public float[] data_points_PCS { get; set; }

    [XmlArray]
    public int[] data_points_SCS { get; set; }
}

public class TrackerCalibrationController : MonoBehaviour {

    public Transform viewPointTransform;
    public Transform virtualCameraTransform;
    public string SphereeTrackerCalibration;

    private string calibraitonDataFile = "data_points_PCS.xml";
    
    private TextMesh calibrationText;
    private TextMesh dataText;

    private Transform cubeTransform;
    
    private List<int> visiblePointIndex = new List<int>();
    private List<float> trackedCalibrationPositions = new List<float>();
    private int currentPointIndex;
    private bool isCompleted;
    private static float[] generatedCalibrationPositions = { 
		0.0f, 1.0f,  0.0f,
		1.0f, 0.0f,  0.0f,
		0.0f, 0.0f,  -1.0f,
		-1.0f, 0.0f,  0.0f,

		0.0f, 0.0f,  1.0f,

		-1.0f/Mathf.Sqrt(3.0f), 1.0f / Mathf.Sqrt(3.0f), -1.0f / Mathf.Sqrt(3.0f),
		-1.0f / Mathf.Sqrt(3.0f), 1.0f / Mathf.Sqrt(3.0f), 1.0f / Mathf.Sqrt(3.0f),
		1.0f / Mathf.Sqrt(3.0f), 1.0f / Mathf.Sqrt(3.0f), -1.0f / Mathf.Sqrt(3.0f),
		1.0f / Mathf.Sqrt(3.0f), 1.0f / Mathf.Sqrt(3.0f), 1.0f / Mathf.Sqrt(3.0f),

		1.0f / Mathf.Sqrt(2.0f),  0.0f, 1.0f / Mathf.Sqrt(2.0f),
		-1.0f / Mathf.Sqrt(2.0f),  0.0f, -1.0f / Mathf.Sqrt(2.0f),
		-1.0f / Mathf.Sqrt(2.0f), 0.0f, 1.0f / Mathf.Sqrt(2.0f), 
		1.0f / Mathf.Sqrt(2.0f), 0.0f, -1.0f / Mathf.Sqrt(2.0f), 

		1.0f / Mathf.Sqrt(2.0f), 0.0f, 1.0f / Mathf.Sqrt(2.0f),
		0.0f, 1.0f / Mathf.Sqrt(2.0f), 1.0f / Mathf.Sqrt(2.0f),
		-1.0f / Mathf.Sqrt(2.0f), 0.0f, 1.0f / Mathf.Sqrt(2.0f),
		0.0f, 1.0f / Mathf.Sqrt(2.0f), -1.0f / Mathf.Sqrt(2.0f),

		0.0f, Mathf.Sqrt(1.0f / 3.0f), Mathf.Sqrt(2.0f/3.0f),
		0.0f, Mathf.Sqrt(1.0f / 3.0f), -Mathf.Sqrt(2.0f / 3.0f),
		Mathf.Sqrt(1.0f / 3.0f), 0.0f, Mathf.Sqrt(2.0f / 3.0f),
		Mathf.Sqrt(1.0f / 3.0f), 0.0f, -Mathf.Sqrt(2.0f / 3.0f),

		0.0f, Mathf.Sqrt(2.0f / 3.0f), Mathf.Sqrt(1.0f / 3.0f),
		0.0f, Mathf.Sqrt(2.0f / 3.0f), -Mathf.Sqrt(1.0f / 3.0f),
		Mathf.Sqrt(2.0f / 3.0f), 0.0f, Mathf.Sqrt(1.0f / 3.0f),
		-Mathf.Sqrt(2.0f / 3.0f), 0.0f, Mathf.Sqrt(1.0f / 3.0f)};
    private int totalPoints;

	void Start () {
        cubeTransform = transform.Find("Cube");
        calibrationText = transform.Find("Text").gameObject.GetComponent<TextMesh>();
        dataText = cubeTransform.Find("DataText").gameObject.GetComponent<TextMesh>();

        calibrationText.text = "Press A to record";
        dataText.text = viewPointTransform.position.ToString();
        totalPoints = generatedCalibrationPositions.Length / 3;
        isCompleted = false;

        trackedCalibrationPositions.Clear();
        visiblePointIndex.Clear();
        
        setCubeTransform();
	}
	
	void Update () {
        if (viewPointTransform != null)
        {
            handleInput();
            if (!isCompleted)
                dataText.text = currentPointIndex.ToString() + "/" + totalPoints.ToString() + ": " + viewPointTransform.position.ToString();
            else
                dataText.text = "Finish. Data saved!";
        }
            
	}

    private void handleInput()
    {
        if (Input.GetButtonDown("XboxOneA"))
        {
            recordCurrentPosition();
            goNextPosition();
        }

        //if (Input.GetButtonDown("XboxOneB"))
        //{
        //    goNextPosition();
        //}
    }

    private void recordCurrentPosition()
    {
        trackedCalibrationPositions.Add(viewPointTransform.position.x);
        trackedCalibrationPositions.Add(viewPointTransform.position.y);
        trackedCalibrationPositions.Add(viewPointTransform.position.z);

        visiblePointIndex.Add(currentPointIndex);

        calibrationText.text = "Record tracked point No. " + currentPointIndex.ToString() + " :" + viewPointTransform.position.ToString();
    }

    private void goNextPosition()
    {
        if (currentPointIndex < totalPoints - 1)
        {
            currentPointIndex++;
            setCubeTransform();
        }
        else 
        {
            calibrationText.text = "Finish calibraiton!";
            saveCalibraitonXml();
            isCompleted = true;
        }
    }

    private void setCubeTransform()
    {
        cubeTransform.position = new Vector3(generatedCalibrationPositions[currentPointIndex * 3],
                                             generatedCalibrationPositions[currentPointIndex * 3 + 1],
                                             generatedCalibrationPositions[currentPointIndex * 3 + 2]);
        cubeTransform.LookAt(Vector3.zero, Vector3.up);

        virtualCameraTransform.position = cubeTransform.position * 3;
    }

    private void saveCalibraitonXml()
    {

        trackerCalibraitonData calibdata = new trackerCalibraitonData { data_points_PCS = trackedCalibrationPositions.ToArray(), 
                                                                              data_points_SCS = visiblePointIndex.ToArray() };

        XmlSerializer xmls = new XmlSerializer(typeof(trackerCalibraitonData));

        string trackerCalibrationPath = Path.GetFullPath(SphereeTrackerCalibration);

        trackerCalibrationPath = Path.Combine(trackerCalibrationPath, calibraitonDataFile);

        using (FileStream stream = new FileStream(trackerCalibrationPath, FileMode.Create))
        {
            xmls.Serialize(stream, calibdata);
        }
    }
}
