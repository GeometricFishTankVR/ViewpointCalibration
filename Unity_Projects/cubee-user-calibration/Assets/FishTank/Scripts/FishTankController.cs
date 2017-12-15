using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTankController : MonoBehaviour {

    public float indexOfRefraction;
	// Use this for initialization
	void Start () {
        Shader.SetGlobalFloat("indexOfRefraction", indexOfRefraction);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
