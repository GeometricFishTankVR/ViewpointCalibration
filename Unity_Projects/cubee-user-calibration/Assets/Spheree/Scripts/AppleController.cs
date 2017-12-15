using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour {

    private AudioSource Audio;
    private Rigidbody rigidBody;
    private Renderer appleRenderer;
    void Start () {
        Audio = this.GetComponent<AudioSource>();
        rigidBody = this.GetComponent<Rigidbody>();
        appleRenderer = transform.Find("objapple").gameObject.GetComponent<Renderer>();
			}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("environment") && !rigidBody.isKinematic)
        {
            Audio.Play();
            Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>Target dropped, collided on " + other.gameObject.name + "</event></t>", 
                                            Time.time.ToString("F3")));
            //rigidBody.isKinematic = true;
            if (other.gameObject.name != "WoodenCrate")
                appleRenderer.material.color = Color.gray;      
        }

    }
}
