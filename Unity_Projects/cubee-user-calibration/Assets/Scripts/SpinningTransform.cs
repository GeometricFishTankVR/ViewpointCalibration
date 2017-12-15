using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningTransform : MonoBehaviour {

    public float speed;
    private Transform thisTransform;
	// Use this for initialization
	void OnEnable () {
        thisTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        thisTransform.Rotate(Vector3.up, speed*Time.deltaTime*360);
	}
}
