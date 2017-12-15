using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeSceneController : MonoBehaviour {

    public Transform viewerTransform;

    private Rigidbody prizeBoxRigidBody;
    private Transform textTransform;
    private AudioSource applauseAudio;
    private bool isrunning;
    // Use this for initialization
	void Start () {
        this.gameObject.SetActive(false);
        isrunning = false;
        
        GameObject prizeBox = this.transform.Find("prize-box").gameObject;
        prizeBoxRigidBody = prizeBox.GetComponent<Rigidbody>();
        prizeBoxRigidBody.isKinematic = true;

        textTransform = this.transform.Find("prize-text");

        applauseAudio = this.GetComponent<AudioSource>();
	}

    public bool IsRunning
    {
        get
        {
            return isrunning;
        }
        set
        {
            isrunning = value;
            this.gameObject.SetActive(isrunning);
            if (isrunning)
            {
                applauseAudio.Play();
                Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>RunPrizeScene</event></t>", Time.time.ToString("F3")));
            }
        }

    }

	// Update is called once per frame
	void Update () {
        if (isrunning)
        {
            textTransform.rotation = viewerTransform.rotation;
            if (Input.GetButtonDown("Release") )
                prizeBoxRigidBody.isKinematic = false;	    
        }
	
	}

}
