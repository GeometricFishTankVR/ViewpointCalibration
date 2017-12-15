using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;

public class BodySourceManager : MonoBehaviour 
{
    public Transform Head;
    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;
    private bool frameReady;
    private CameraSpacePoint? closestPoint;
    public Body[] GetData()
    {
        return _Data;
    }
    
    public CameraSpacePoint? GetClosestHeadPosition()
    {
        if (frameReady)
        {
            float closestZ = float.MaxValue;
            closestPoint = null;
            if (_Data != null)
            {
                for (int i = 0; i < _Data.Length; i++)
                {
                    if (_Data[i].IsTracked)
                    {
                        Dictionary<JointType, Windows.Kinect.Joint> Joints = _Data[i].Joints;
                        Windows.Kinect.CameraSpacePoint tmp = Joints[JointType.Head].Position;
                        if (tmp.Z < closestZ)
                        {
                            closestZ = tmp.Z;
                            closestPoint = tmp;
                        }
                    }
                }
            }
            frameReady = false;
        }
        return closestPoint;
    }

    void Start () 
    {
        Assert.IsNotNull(Head, "Head cannot be null. Please specify in the Editor.");
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();
            _Reader.FrameArrived += _Reader_FrameArrived;
            frameReady = false;
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }   
    }

    void Update()
    {
        CameraSpacePoint? head = GetClosestHeadPosition();
        if(head != null)
        {
            Head.position = new Vector3(-head.Value.X, head.Value.Y, head.Value.Z);
        }
    }

    private void _Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(_Data);
                frameReady = true;
                frame.Dispose();
                frame = null;
            }
        }
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
