using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shaderInitializer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Shader.SetGlobalMatrix("sphere_transform", Matrix4x4.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
