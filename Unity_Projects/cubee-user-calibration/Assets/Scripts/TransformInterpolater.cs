using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformInterpolater : MonoBehaviour {
    public Transform Target;
    public float Speed;
    public bool ChangeScale = false;

    private Vector3 targetScaleFactor;
	// Use this for initialization
	void Start () {
        targetScaleFactor = Target.lossyScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(Target != null)
        {
            transform.position = Vector3.Lerp(transform.position, Target.position, Time.deltaTime * Speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, Time.deltaTime * Speed);
            if (ChangeScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScaleFactor, Time.deltaTime * Speed);
            }
            if (Vector3.Distance(transform.position, Target.position) < 0.00001f && Quaternion.Angle(transform.rotation, Target.rotation) < 0.00001f)
            {
                Destroy(this);
            }
        }
	}
}
