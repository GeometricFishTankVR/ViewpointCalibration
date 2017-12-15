using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformToTextBox : MonoBehaviour {

    private Transform myTransform;
    public Text textBox;

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (textBox != null)
        {
            textBox.text = myTransform.position.ToString();
        }
	}
}
