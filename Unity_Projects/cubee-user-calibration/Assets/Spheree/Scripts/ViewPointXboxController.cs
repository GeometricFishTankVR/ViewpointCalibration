using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPointXboxController : MonoBehaviour {

    public Transform viewPointTransform;

 //   private float sensitivity;
	void Start () {
 //       sensitivity = 0.1f;
	}
	
	void Update () {
        handleInput();
	}

    private void handleInput()
    {
        //Vector3 move = new Vector3(Input.GetAxis("CameraHorizontal"), Input.GetAxis("CameraVertical"), Input.GetAxis("CameraDepth"));
        viewPointTransform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("CameraHorizontal"));
        viewPointTransform.RotateAround(Vector3.zero, Vector3.right, Input.GetAxis("CameraVertical"));
        //float new_z = Input.GetAxis("CameraDepthUp") + Input.GetAxis("CameraDepthDown");
        //new_z = new_z * sensitivity;
        //Vector3 z_dir = viewPointTransform.position;
        //z_dir = Vector3.Normalize(z_dir) * new_z;
        //viewPointTransform.Translate(z_dir);

    }
}
