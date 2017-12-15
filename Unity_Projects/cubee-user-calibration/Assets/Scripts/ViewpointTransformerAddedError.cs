using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class ViewpointTransformerAddedError : ViewpointTransformer
{
    public Quaternion qVHerror = Quaternion.identity;
    public bool shouldAddqVHerror = false;
    protected Quaternion qVHcalibration;

    public Vector3 oVerror = Vector3.zero;
    public bool shouldAddoVerror = false;
    protected Vector3 oVcalibration;

    public Quaternion qTDerror = Quaternion.identity;
    public bool shouldAddqTDerror = false;
    protected Quaternion qTDcalibration;
    protected Matrix4x4 XTDcalibration;

    public Vector3 tTDerror = Vector3.zero;
    public bool shouldAddtTDerror = false;

    
    #region monobehaviour
    void Update()
    {
        ViewpointCalibration calibration = PersistentProjectStorage.Instance.CurrentViewpointCalibration;
        if (calibration != null)
        {
            if (headUpdatesArePaused)
            {
                HeadInTracker.position = hTPositionLocked;
                HeadInTracker.rotation = hTRotationLocked;
            }
            ApplyErrors(ref calibration);
            calibration.TransformPointXTD(HeadInTracker, ref HeadInDisplay);
            calibration.TransformPointXTV(HeadInTracker, ref ViewpointInDisplay, use6dofFixIfAvailable);
            UnapplyErrors(ref calibration);
        }
    }
    #endregion

    protected void ApplyErrors(ref ViewpointCalibration calibration)
    {
        if (shouldAddqVHerror)
        {
            qVHcalibration = calibration.qVH; // Save the value
            calibration.qVH = qVHcalibration * qVHerror;
        }
        if (shouldAddoVerror)
        {
            oVcalibration = calibration.oV; // Save the value
            calibration.oV = oVcalibration + oVerror;
        }
        if (shouldAddqTDerror)
        {
            XTDcalibration = calibration.XTD;
            qTDcalibration = calibration.qTD;
            calibration.qTD = qTDcalibration * qTDerror;
            calibration.ApplyRotationToXTD(qTDerror);
        }
        if (shouldAddtTDerror)
        {
            XTDcalibration = calibration.XTD;
            calibration.ApplyTranslationToXTD(tTDerror);
        }
    }

    protected void UnapplyErrors(ref ViewpointCalibration calibration)
    {
        if (shouldAddqVHerror)
        {
            calibration.qVH = qVHcalibration; // Reset the value
        }
        if (shouldAddoVerror)
        {
            calibration.oV = oVcalibration;
        }
        if (shouldAddqTDerror)
        {
            calibration.qTD = qTDcalibration;
            calibration.XTD = XTDcalibration;
        }
        if (shouldAddtTDerror)
        {
            calibration.XTD = XTDcalibration;
        }
    }

}
