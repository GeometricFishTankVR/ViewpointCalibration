using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour {

    public Transform fishTank;
    public float force;
    private Transform thisTransform;
    private Rigidbody thisRigidBody;

	// Use this for initialization
	void Start () {
        thisTransform = GetComponent<Transform>();
        thisRigidBody = GetComponent<Rigidbody>();
        
        thisRigidBody.velocity = 0.1f*Random.onUnitSphere;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        thisRigidBody.AddForce(Vector3.up*force);

        if (thisTransform.position.y > fishTank.position.y + fishTank.localScale.y / 2)
        {
            GameObject.DestroyImmediate(transform.gameObject);
        }
	}
}
