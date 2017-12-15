using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour {

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 forward = transform.forward;
        //Shader.SetGlobalVector("_LightPos", new Vector4(forward.x, forward.y, forward.z, 1));
        Shader.SetGlobalVector("_LightPos", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));
    }
}
