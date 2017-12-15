using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewrayTargetAligner : MonoBehaviour {
    public Transform Target;
    public Transform Reticle;
    public Transform Viewpoint;


    private Vector3 reticleTargetPosition;
    #region monobehaviour
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Target != null && Reticle != null && Viewpoint != null)
        {
            Target.rotation = Quaternion.LookRotation(Target.position - Viewpoint.position, Vector3.up);
            Reticle.rotation = Target.rotation;

            //
            Vector3 n = Target.forward;
            Vector3 p0 = Target.position;
            Vector3 l = Viewpoint.forward;
            Vector3 l0 = Viewpoint.position;

            float d = Vector3.Dot(p0 - l0, n) / Vector3.Dot(l, n);
            reticleTargetPosition = d * 0.99f * l + l0;
            Reticle.position = Vector3.Lerp(Reticle.position, reticleTargetPosition, Time.deltaTime * 5);
        }
	}
    #endregion
}
