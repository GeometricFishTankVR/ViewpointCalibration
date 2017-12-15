using UnityEngine;

public class ManualHeadTracker : MonoBehaviour {
    public Transform hT; // Head in Tracker space
    public Vector3 oV = Vector3.zero; // Offset in View space
    public Quaternion qHV = Quaternion.identity;
    #region monobehaviour

    void Update()
    {
        hT.localPosition = transform.parent.InverseTransformPoint(transform.TransformPoint(oV));
        hT.rotation = qHV * transform.rotation;
    }

    void Start()
    {
        ViewpointCalibration identityCalibration = new ViewpointCalibration();
        identityCalibration.XTD = Matrix4x4.identity;
        identityCalibration.oV = Vector3.zero;
        identityCalibration.qVH = Quaternion.identity;
        identityCalibration.Error = 0;
        identityCalibration.WasTrackedWith6DoF = true;
        PersistentProjectStorage.Instance.CurrentViewpointCalibration = identityCalibration;
    }
    #endregion
}
