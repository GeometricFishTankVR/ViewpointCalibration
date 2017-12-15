using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminCameraController : MonoBehaviour {
    private List<System.Type> AdminCameraScripts;
    public GameObject AdminCamera;
    public Transform DisplayFocus;
    public Transform Viewpoint;
    public Transform Head;
    public bool LinkRotationIfAvailable = true;
    public Toggle RotationLinkToggle;

	// Use this for initialization
	void Start () {
        AdminCameraScripts = new List<System.Type>();
        AdminCameraScripts.Add(typeof(CameraLink));
        AdminCameraScripts.Add(typeof(CameraTrackball));
	}

    private void RemoveAllCameraScripts()
    {
        foreach(var type in AdminCameraScripts)
        {
            var script = AdminCamera.GetComponent(type);
            if(script != null)
            {
                Destroy(script);
            }
        }
    }

    public void OnButtonViewpoint()
    {
        RemoveAllCameraScripts();
        CameraLink script = AdminCamera.AddComponent<CameraLink>();
        script.Target = Viewpoint;
        script.TargetFocus = DisplayFocus;
        script.ShouldLinkRotation = RotationLinkToggle.isOn && PersistentProjectStorage.Instance.IsHeadTracker6DoF;

        
    }

    public void OnButtonHead()
    {
        RemoveAllCameraScripts();
        CameraLink script = AdminCamera.AddComponent<CameraLink>();
        script.Target = Head;
        script.TargetFocus = DisplayFocus;
        script.ShouldLinkRotation = RotationLinkToggle.isOn && PersistentProjectStorage.Instance.IsHeadTracker6DoF;
    }

    public void OnButtonTrackball()
    {
        RemoveAllCameraScripts();
        CameraTrackball script = AdminCamera.AddComponent<CameraTrackball>();
        script.Focus = DisplayFocus;
        script.MinimumZoomDistance = 0.3f;
        script.MaximumZoomDistance = 3;
        script.Sensitivity = 1;
        script.MouseButton = 0;
    }
}
