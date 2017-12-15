using UnityEngine;
using UnityEngine.Assertions;

public class ViewpointTransformer : MonoBehaviour {
    public Transform HeadInTracker;
    public Transform HeadInDisplay;
    public Transform ViewpointInDisplay;
    public bool use6dofFixIfAvailable = true;

    protected Vector3 hTPositionLocked = Vector3.zero;
    protected Quaternion hTRotationLocked = Quaternion.identity;
    protected bool headUpdatesArePaused = false;
    protected ViewpointCalibration calibration;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(HeadInTracker, "Head in tracker cannot be null. Please specify an object in the Editor.");
        Assert.IsNotNull(HeadInDisplay, "Head in display cannot be null. Please specify an object in the Editor.");
        Assert.IsNotNull(ViewpointInDisplay, "Viewpoint in display cannot be null. Please specify an object in the Editor.");
        calibration = null;
    }

    public void CalibrationHaChanged()
    {
        calibration = null;
    }

	void Update () {

        if (calibration == null)
        {
            calibration = PersistentProjectStorage.Instance.CurrentViewpointCalibration;
            if (calibration != null)
            {
                SphereeUpCorrector upCorrector = FindObjectOfType<SphereeUpCorrector>();
                if (upCorrector != null)
                {
                    //calibration = calibration.Copy();
                    upCorrector.SetOriginalXTD(calibration.XTD);
                    upCorrector.SetOriginalQTD(calibration.qTD);
                    upCorrector.CalculateCorrectedTransform();
                    calibration.XTD = upCorrector.GetCorrectedXTD();
                    calibration.qTD = upCorrector.GetCorrectedQTD();

                }
            }
        }
        
        if (calibration != null)
        {
            if(headUpdatesArePaused)
            {
                HeadInTracker.position = hTPositionLocked;
                HeadInTracker.rotation = hTRotationLocked;
            }

            

            calibration.TransformPointXTD(HeadInTracker, ref HeadInDisplay);
            calibration.TransformPointXTV(HeadInTracker, ref ViewpointInDisplay, use6dofFixIfAvailable);
        }
	}
    #endregion

    public void PauseHeadUpdates()
    {
        hTPositionLocked = HeadInTracker.position;
        hTRotationLocked = HeadInTracker.rotation;
        headUpdatesArePaused = true;
    }

    public void ContinueHeadUpdates()
    {
        hTPositionLocked = Vector3.zero;
        hTRotationLocked = Quaternion.identity;
        headUpdatesArePaused = false;
    }
}
