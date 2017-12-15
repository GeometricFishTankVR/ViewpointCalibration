using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereeUpCorrector : MonoBehaviour {

    private Matrix4x4 correctedXTD;
    private Matrix4x4 originalXTD;
    private Quaternion originalQTD;
    private Quaternion correctedQTD;
    private Vector3 upDirection;
    // Use this for initialization
    void Start()
    {
        correctedXTD = Matrix4x4.identity;
        originalXTD = Matrix4x4.identity;
        correctedQTD = Quaternion.identity;
        originalQTD = Quaternion.identity;
        upDirection = Vector3.up;
    }        

    private void Update()
    {
        //ViewpointCalibration calibration = PersistentProjectStorage.Instance.CurrentViewpointCalibration;
        //if (calibration != null)
        //{
        //    Quaternion rot = Quaternion.FromToRotation(calibration.XTD.GetColumn(1), upDirection);
        //    Matrix4x4 alignedXTD = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
        //    Shader.SetGlobalMatrix("sphere_transform", alignedXTD);
        //}
    }


    public void CalculateCorrectedTransform()
    {
        Quaternion rot = Quaternion.FromToRotation(originalXTD.GetColumn(1), upDirection);
        Matrix4x4 alignedXTD = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
        Shader.SetGlobalMatrix("sphere_transform", alignedXTD);
        correctedXTD = alignedXTD * originalXTD;
        correctedQTD = CubeeMath.ExtractRotationFromMatrix(ref correctedXTD);
    }

    public void SetOriginalXTD(Matrix4x4 newXTD)
    {
        originalXTD = newXTD;
    }
    public void SetOriginalQTD(Quaternion newQTD)
    {
        originalQTD = newQTD;
    }

    public void SetUpDirection(Vector3 newUp)
    {
        upDirection = newUp;
    }

    public Matrix4x4 GetCorrectedXTD()
    {
        return correctedXTD;
    }

    public Quaternion GetCorrectedQTD()
    {
        return correctedQTD;
    }
}
