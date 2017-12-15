using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Camera))]
public class SphereeProjector : MonoBehaviour {
    public int ScreenPixelWidth;
    public int ScreenPixelHeight;
    public int ScreenRefreshRate;
    public Transform Spheree;
    public Transform Screen;

    private Camera thisCamera;

    void OnEnable()
    {
        
    }
	// Use this for initialization
	void Start () {
        Assert.IsNotNull(Spheree, "Spheree cannot be null. Please specify an object in the Editor.");
        Assert.IsNotNull(Screen, "Screen cannot be null. Please specify an object in the Editor.");
        Assert.IsTrue(ScreenRefreshRate > 0, "Screen refresh rate must be greater than 0. Please fix in the Editor.");
        Assert.IsTrue(ScreenPixelWidth > 0, "Screen pixel width must be greater than 0. Please fix in the Editor.");
        Assert.IsTrue(ScreenPixelHeight > 0, " Screen pixel height must be greater than 0. Please fix in the Editor.");
        thisCamera = GetComponent<Camera>(); // cache the component

        int displayIdx = thisCamera.targetDisplay;

        if (displayIdx < Display.displays.Length)
        {
            //Display.displays[displayIdx].Activate(ScreenPixelWidth, ScreenPixelHeight, ScreenRefreshRate);
            //Debug.logger.Log(Time.time.ToString("F3"), "Activated display: " + displayIdx);
            Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ActivatedDisplay{1}</event></t>", 
                Time.time.ToString("F3"),
                displayIdx));
        }
        else
        {
            //Debug.logger.Log(Time.time.ToString("F3"), "Not enough displays to activate this projector: " + gameObject.name);
            Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>NotEnoughDisplays{1}</event></t>",
                Time.time.ToString("F3"),
                gameObject.name));
        }
    }
	
	// Update is called once per frame
	void LateUpdate () {
        float scale = Spheree.localScale.x / 2;
        thisCamera.projectionMatrix = Matrix4x4.Ortho(-scale, scale, -scale, scale, 0.1f, 10000);
        thisCamera.nearClipPlane = scale / 2;
        thisCamera.farClipPlane = 1.5f*scale;
    }
}
