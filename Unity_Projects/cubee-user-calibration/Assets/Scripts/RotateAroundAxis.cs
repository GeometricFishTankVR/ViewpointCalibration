using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundAxis : MonoBehaviour {
    public Vector3 RotationAxis = Vector3.up;
    public float RotationSpeed = 0;

	// Update is called once per frame
	void Update () {
        transform.Rotate(RotationAxis, RotationSpeed * Time.deltaTime, Space.World);
	}
}
