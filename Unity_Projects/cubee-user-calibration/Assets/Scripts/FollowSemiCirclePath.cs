using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSemiCirclePath : MonoBehaviour {
    public Transform Target;
    public float Speed;

    private Vector3 rotationAxis;
    private Vector3 rotationPoint;

    #region monobehaviour
    void OnEnable () {
        if(Target != null)
        {
            rotationAxis = Target.right;
            rotationPoint = (transform.position + Target.position) / 2;
        }
        else
        {
            enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(rotationPoint, rotationAxis, Speed * Time.deltaTime);
	}
    #endregion
}
