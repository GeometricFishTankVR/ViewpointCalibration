using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraLink : MonoBehaviour {
    public Transform Target;
    public Transform TargetFocus;
    public bool ShouldLinkRotation = false;

    #region monobehaviour
    void Update () {
		if (Target != null)
        {
            transform.position = Target.position;
            if(ShouldLinkRotation)
            {
                transform.rotation = Target.rotation;
            }
            else if(TargetFocus != null)
            {
                transform.rotation = Quaternion.LookRotation(TargetFocus.position - transform.position, Vector3.up);
            }
        }
	}
    #endregion
}
