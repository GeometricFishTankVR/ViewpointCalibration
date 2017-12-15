using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Xml;
using System.IO;

public class SphereeTrackerCalibrationTransformer : MonoBehaviour {

    public string SphereeTrackerCalibration;
    public string calibraitonDataFile;
    public Transform headInTracker;
    public HeadInDisplay headInDisplay;
 //   public ViewerOptionSelector viewerOption;
   
    private Vector3 offSet;
//    private bool isFlipped;

    private Matrix4x4 transformTrackerToDisplay= new Matrix4x4();

    void OnEnable() {
        Assert.IsNotNull(SphereeTrackerCalibration, "string SphereeTrackerCalibration can't be null in SphereeTrackerCalibrationTransformer.");
        Assert.IsNotNull(calibraitonDataFile, "string calibraitonDataFile can't be null in SphereeTrackerCalibrationTransformer.");
        Assert.IsNotNull(headInTracker, "Transform headInTracker can't be null in SphereeTrackerCalibrationTransformer.");
        Assert.IsNotNull(headInDisplay, "Script headInDisplay can't be null in SphereeTrackerCalibrationTransformer.");
 //       Assert.IsNotNull(viewerOption, "ViewerOptionSelector viewerOption can't be null in SphereeTrackerCalibrationTransformer.");
    }    

	void Start () {
        readTransformXmlFile();
        Assert.IsTrue(transformTrackerToDisplay != Matrix4x4.zero, "SphereeTrackerCalibrationTransformer: transformTrackerToDisplay is not correctly loaded. ");
        offSet = Vector3.zero;
 //       isFlipped = false;
	}
	
	void FixedUpdate () {
        if (headInDisplay!=null)
            transformHeadInTracker();
	}

    void Update() {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    isFlipped = !isFlipped;
        //}
        //handleInput();
    }

    public void setOffSet(ViewerOption.EyeOption option)
    {
        switch (option)
        {
            case ViewerOption.EyeOption.Left:
                offSet = new Vector3(0.0f, -2.0f, -0.3f);
                break;
            case ViewerOption.EyeOption.Right:
                offSet = new Vector3( -0.5f,-4.5f, -1.0f);
                break;
            case ViewerOption.EyeOption.Both:
                offSet = new Vector3( -0.25f, -3.25f, -0.65f);
                break;
        }
    }

    private void handleInput()
    {
        float sensitivity = 0.02f;
        Vector3 movement = new Vector3(Input.GetAxis("CameraHorizontal"), Input.GetAxis("CameraVertical"), (Input.GetAxis("CameraDepthUp") + Input.GetAxis("CameraDepthDown")));
        if (Vector3.Magnitude(movement) > 0.001f)
        {
            offSet += movement * sensitivity;
            //Debug.logger.Log(offSet.ToString("F3"));
        }

        if (Input.GetButtonDown("XboxOneA"))
            offSet = new Vector3(0.0f, -4.5f, -1.0f);

    }

    private void transformHeadInTracker()
    {
		Matrix4x4 tranformT2T = computeMatrixFromHeadInTracker();
        
        Vector3 unity_position = transformTrackerToDisplay.MultiplyPoint (tranformT2T.MultiplyPoint (offSet));

        headInDisplay.AxisX = transformTrackerToDisplay.MultiplyVector(tranformT2T.GetColumn(0));
        headInDisplay.AxisY = transformTrackerToDisplay.MultiplyVector(tranformT2T.GetColumn(1));
        headInDisplay.AxisZ = transformTrackerToDisplay.MultiplyVector(tranformT2T.GetColumn(2));

        headInDisplay.Position = unity_position;

    }
    
    private Matrix4x4 computeMatrixFromHeadInTracker()
	{
		Vector3 plposition = headInTracker.position;
        Quaternion plrotation = headInTracker.rotation;

        //float q0 = plrotation [0];
        //float q1 = plrotation [1];
        //float q2 = plrotation [2];
        //float q3 = plrotation [3];

        Matrix4x4 rigidmat = new Matrix4x4();
        //Vector4 temp_vec = new Vector4(q0*q0 + q1*q1 - q2*q2 - q3*q3, 2*(q3*q0 + q1*q2), 2*(q1*q3 - q0*q2), 0);
        //rigidmat.SetColumn (0, temp_vec);
        //temp_vec = new Vector4(2*(q1*q2 - q0*q3), q0*q0 - q1*q1 + q2*q2 - q3*q3, 2*(q1*q0 + q3*q2), 0);
        //rigidmat.SetColumn (1, temp_vec);
        //temp_vec = new Vector4(2*(q1*q3 + q0*q2), 2*(q2*q3 - q0*q1), q0*q0 - q1*q1 - q2*q2 + q3*q3, 0);
        //rigidmat.SetColumn (2, temp_vec);
        //temp_vec = new Vector4(plposition.x, plposition.y, plposition.z,1);
        //rigidmat.SetColumn (3, temp_vec);

        //if (isFlipped)
        //{ 
        //    Matrix4x4 flipMat = new Matrix4x4();
        //    flipMat.SetColumn(0, new Vector4(-1, 0, 0, 0));
        //    flipMat.SetColumn(1, new Vector4(0, -1, 0, 0));
        //    flipMat.SetColumn(2, new Vector4(0, 0, -1, 0));
        //    flipMat.SetColumn(3, new Vector4(0, 0, 0, 1));

        //    rigidmat = flipMat * rigidmat;
        //}

        rigidmat.SetColumn(0, new Vector4(headInTracker.transform.right.x, headInTracker.transform.right.y, headInTracker.transform.right.z, 0));
        rigidmat.SetColumn(1, new Vector4(headInTracker.transform.up.x, headInTracker.transform.up.y, headInTracker.transform.up.z, 0));
        rigidmat.SetColumn(2, new Vector4(headInTracker.transform.forward.x, headInTracker.transform.forward.y, headInTracker.transform.forward.z, 0));
        rigidmat.SetColumn(3, new Vector4(headInTracker.transform.position.x, headInTracker.transform.position.y, headInTracker.transform.position.z, 1));

		return rigidmat;
	}

    private void readTransformXmlFile()
    {
        string trackerCalibrationPath = Path.GetFullPath(SphereeTrackerCalibration);
        trackerCalibrationPath = Path.Combine(trackerCalibrationPath, calibraitonDataFile);

        XmlReaderSettings settings = new XmlReaderSettings();
        XmlReader reader = XmlReader.Create(trackerCalibrationPath, settings);
        reader.ReadToFollowing("transform_T2S");
        XmlReader subReader = reader.ReadSubtree();
        subReader.ReadToFollowing("data");
        string dataString = subReader.ReadElementContentAsString().Trim().Replace("\n", "").Replace("\r", "");
        int temp_len = 0;
        while (temp_len != dataString.Length)
        {
            temp_len = dataString.Length;
            dataString = dataString.Replace("  ", " ");
        }
        string[] stringArray = dataString.Split(' ');

        for (int i = 0; i < 4; i++)
            transformTrackerToDisplay.SetRow(i, new Vector4(float.Parse(stringArray[4 * i]), 
                                                            float.Parse(stringArray[4 * i + 1]), 
                                                            float.Parse(stringArray[4 * i + 2]), 
                                                            float.Parse(stringArray[4 * i + 3])));
    }
}
