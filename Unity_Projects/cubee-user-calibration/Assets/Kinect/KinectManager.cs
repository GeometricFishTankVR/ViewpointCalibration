using UnityEngine;
using Windows.Kinect;

public class KinectManager : MonoBehaviour {
    private KinectSensor _Sensor;
    private BodySourceManager _BodySourceManager;
    public GameObject MultiSourceManager;
    public GameObject BodyManager;
    public Transform TrackerObject;

    // Use this for initialization
    void Start () {
        _Sensor = KinectSensor.GetDefault();
        if(_Sensor != null)
        {
            MultiSourceManager.SetActive(true);
            BodyManager.SetActive(true);
            _BodySourceManager = BodyManager.GetComponent<BodySourceManager>();
        }
	}

    void Update()
    {
        TrackerObject.position = GetTrackedHeadPosition();
    }

    void OnDisable()
    {
        if (_Sensor != null)
        {
            MultiSourceManager.SetActive(false);
            BodyManager.SetActive(false);
            _Sensor.Close();

        }
    }

    public Vector3 GetTrackedHeadPosition ()
    {
        if (isKinectActive())
        {
            CameraSpacePoint? tmp = _BodySourceManager.GetClosestHeadPosition();
            CameraSpacePoint kinectPoint;
            if (tmp == null) return Vector3.zero;
            else kinectPoint = (CameraSpacePoint)tmp;
            return new Vector3(kinectPoint.X, kinectPoint.Y, -kinectPoint.Z);
            // Kinect tracks in meters and the Cubee is modeled in centimeters
        }
        else return Vector3.zero;
    }

    public bool isKinectActive()
    {
        return _Sensor != null && _Sensor.IsAvailable && _Sensor.IsOpen;
    }
}
