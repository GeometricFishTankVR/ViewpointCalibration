using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSourceController : MonoBehaviour {

    public Object bubbleObject;
    public Transform fishTank;
    public float frequency;
    private Transform thisTransform;
    private float ellapsedTime;


	// Use this for initialization
	void Start () {
        thisTransform = GetComponent<Transform>();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (ellapsedTime > 1 / frequency)
        {
            makeBubble();
            ellapsedTime = 0;
        }
        ellapsedTime += Time.deltaTime;
		
	}

    void makeBubble()
    {
        GameObject bubble = (GameObject) GameObject.Instantiate(bubbleObject, thisTransform.position, Quaternion.identity, thisTransform);
        bubble.GetComponent<BubbleController>().fishTank = fishTank;
        bubble.GetComponent<MaterialManagerColor>().fishTank = fishTank;
    }
}
