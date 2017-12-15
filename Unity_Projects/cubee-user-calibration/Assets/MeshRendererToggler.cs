using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererToggler : MonoBehaviour {

    public bool state;
    private MeshRenderer thisMeshRenderer;


	// Use this for initialization
	void Start () {
        thisMeshRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonUp("XboxOneView(Back)"))
        {
            thisMeshRenderer.enabled = !thisMeshRenderer.enabled;
            state = thisMeshRenderer.enabled;
        }
	}
}
