using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeBoxController : MonoBehaviour {

    private AudioSource Audio;

    void Start () {
        Audio = this.GetComponent<AudioSource>();
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("environment"))
            Audio.Play();
    }
}
