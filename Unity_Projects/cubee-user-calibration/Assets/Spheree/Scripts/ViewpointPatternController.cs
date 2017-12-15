using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewpointPatternController : MonoBehaviour {

    public Transform viewCamera;
    public Transform displayCenter;

    private Transform thisTransform;

    // Use this for initialization
    void Start () {
        thisTransform = transform;
        thisTransform.position = displayCenter.position;
	}
	
	// Update is called once per frame
	void Update () {
        thisTransform.LookAt(viewCamera);

	}
}
