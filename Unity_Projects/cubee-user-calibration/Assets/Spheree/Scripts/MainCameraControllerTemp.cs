using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraControllerTemp : MonoBehaviour {
    public Transform viewPointTransform;
	void Start () {
        this.transform.position = viewPointTransform.position;
        this.transform.rotation = viewPointTransform.rotation;
        this.transform.parent = viewPointTransform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
