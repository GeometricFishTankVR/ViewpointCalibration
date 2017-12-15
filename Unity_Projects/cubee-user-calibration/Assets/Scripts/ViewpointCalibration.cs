using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ViewpointCalibration {
    public static string CalibrationFilesDirectory = "CalibrationFiles";
    public Matrix4x4 XTD;
    public Vector3 oV;
    public Quaternion qVH;
    public Quaternion qTD;
    public float Error;
    public bool WasTrackedWith6DoF;
    public List<Vector3> cD;
    public List<Vector3> hT;
    public List<Quaternion> RHTn;


    public ViewpointCalibration Copy()
    {
        ViewpointCalibration viewPointCalib = new ViewpointCalibration();
        viewPointCalib.cD = new List<Vector3>(cD);
        viewPointCalib.hT = new List<Vector3>(hT);
        viewPointCalib.RHTn = new List<Quaternion>(RHTn);
        viewPointCalib.XTD = XTD;
        viewPointCalib.oV = oV;
        viewPointCalib.qVH = qVH;
        viewPointCalib.qTD = qTD;
        viewPointCalib.Error = Error;
        viewPointCalib.WasTrackedWith6DoF = WasTrackedWith6DoF;
        return viewPointCalib;
    }


    public void TransformPointXTD3DoF(Transform hT, ref Transform hD)
    {
        hD.localPosition = XTD.MultiplyPoint3x4(hT.localPosition);
    }

    public void TransformPointXTD6DoF(Transform hT, ref Transform hD)
    {
        hD.localPosition = XTD.MultiplyPoint3x4(hT.localPosition);
        // Transform the rotation
        Quaternion qHT = hT.rotation;
        Quaternion qHD = qTD * qHT;
        hD.localRotation = qHD;
    }

    public void TransformPointXTD(Transform hT, ref Transform vD)
    {
        if (WasTrackedWith6DoF) TransformPointXTD6DoF(hT, ref vD);
        else TransformPointXTD3DoF(hT, ref vD);
    }

    private void TransformPointXTV3DoF(Transform hT, ref Transform vD)
    {
        TransformPointXTD(hT, ref vD);
        float dist;
        Vector3 hD = vD.localPosition;
        // offset.x fix
        dist = new Vector2(hD.x, hD.z).magnitude;
        float gamma = Mathf.Asin(oV.x / dist);
        float theta = Mathf.Atan2(hD.z, hD.x) - gamma;

        // offset.y fix
        dist = new Vector3(hD.x * Mathf.Cos(gamma), hD.y, hD.z * Mathf.Cos(gamma)).magnitude;
        float psi = Mathf.Asin(oV.y / dist);
        float phi = Mathf.Acos(hD.y / dist) + psi;

        // offset.z fix
        dist = new Vector3(hD.x * Mathf.Cos(gamma), hD.y, hD.z * Mathf.Cos(gamma)).magnitude * Mathf.Cos(psi);
        float r = dist - oV.z;

        // convert from spherical to cartesian
        vD.localPosition = new Vector3(Mathf.Sin(phi) * Mathf.Cos(theta), Mathf.Cos(phi), Mathf.Sin(theta) * Mathf.Sin(phi)) * r;
        vD.localRotation = Quaternion.LookRotation(-vD.localPosition.normalized, Vector3.up); // Assume they are looking at the display
    }

    private void TransformPointXTV6DoF(Transform hT, ref Transform vD)
    {
        Quaternion qHT = hT.rotation;
        // Transform the position
        Vector3 oH = qVH * oV;
        Vector3 oT = qHT * oH;
        Vector3 oD = qTD * oT;
        Vector3 hD = XTD.MultiplyPoint3x4(hT.localPosition);
        vD.localPosition = hD - oD;

        // Transform the rotation
        Quaternion qVT = qHT * qVH;
        Quaternion qVD = qTD * qVT;
        vD.localRotation = qVD;
    }

    public void TransformPointXTV(Transform hT, ref Transform vD, bool use6DoFoVFixIfAvailable)
    {
        if (WasTrackedWith6DoF && use6DoFoVFixIfAvailable) TransformPointXTV6DoF(hT, ref vD);
        else TransformPointXTV3DoF(hT, ref vD);
    }

    public static ViewpointCalibration LoadFromFile(string filepath)
    {
        return Serializer.DeSerializeObject<ViewpointCalibration>(filepath) as ViewpointCalibration;
    }

    public static void SaveToFile(ViewpointCalibration calibration, string filepath)
    {
        Serializer.SerializeObject(calibration, filepath);
    }

    public void ApplyTranslationToXTD(Vector3 translation)
    {
        XTD.m03 += translation.x;
        XTD.m13 += translation.y;
        XTD.m23 += translation.z;
    }

    public void ApplyRotationToXTD(Quaternion q2)
    {
        float scale = CubeeMath.ExtractScaleFromMatrix(ref XTD).x;
        Quaternion q = qTD * q2;
        XTD.m00 = (1 - 2 * q.y * q.y - 2 * q.z * q.z) * scale;
        XTD.m01 = (2 * q.x * q.y - 2 * q.z * q.w) * scale;
        XTD.m02 = (2 * q.x * q.z + 2 * q.y * q.w) * scale;
        XTD.m10 = (2 * q.x * q.y + 2 * q.z * q.w) * scale;
        XTD.m11 = (1 - 2 * q.x * q.x - 2 * q.z * q.z) * scale;
        XTD.m12 = (2 * q.y * q.z - 2 * q.x * q.w) * scale;
        XTD.m20 = (2 * q.x * q.z - 2 * q.y * q.w) * scale;
        XTD.m21 = (2 * q.y * q.z + 2 * q.x * q.w) * scale;
        XTD.m22 = (1 - 2 * q.x * q.x - 2 * q.y * q.y) * scale;
    }
}
