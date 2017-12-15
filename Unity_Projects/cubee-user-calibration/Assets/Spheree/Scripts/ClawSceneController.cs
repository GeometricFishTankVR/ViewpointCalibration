using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// This class controls which scenario is running by keyboard.
/// Update of each scenario is handled by its own Update()
/// </summary>
public class ClawSceneController : MonoBehaviour {

    //private FlyingClawController flyClawController;
    private TargetPickupController targetPickupController; //target pickup scenario
    private PrizeSceneController prizeDropController; // prize drop off scenario
    private FlyingClawController clawController;
    public LogSystem logSystem;

    void Start () {
        targetPickupController = this.transform.Find("target-pickup-scenario").gameObject.GetComponent<TargetPickupController>();
        prizeDropController = this.transform.Find("ending-scenario").gameObject.GetComponent<PrizeSceneController>();
        clawController = this.transform.Find("flying-claw-rigidbody").gameObject.GetComponent<FlyingClawController>();
	}
	
	// Update is called once per frame
	void Update () {
        handleInput();
	}

    private void handleInput()
    {
        // default scene
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            logSystem.IsRecording = false;

            prizeDropController.gameObject.SetActive(false);

            targetPickupController.gameObject.SetActive(true);
            targetPickupController.cleanUp();

            // claw object setup
            clawController.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            clawController.gameObject.transform.position = new Vector3(0.0f, 0.3f, 0.3f);
        }
        
        // practice scene
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Log system setup
            logSystem.IsRecording = true;
            
            // Prize scene setup
            prizeDropController.gameObject.SetActive(false);

            // Target pickup scene setup
            targetPickupController.cleanUp();
            targetPickupController.gameObject.SetActive(true);
            targetPickupController.run(TargetPickup.Mode.Practice, 4);
            
            // claw object setup
            clawController.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            clawController.gameObject.transform.position = new Vector3(0.0f, 0.3f, 0.3f);
        }

        // task scene
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            logSystem.IsRecording = true;

            prizeDropController.gameObject.SetActive(false);

            targetPickupController.cleanUp();
            targetPickupController.gameObject.SetActive(true);
            targetPickupController.run(TargetPickup.Mode.Task, 8);

            clawController.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            clawController.gameObject.transform.position = new Vector3(0.0f, 0.3f, 0.3f);
        }

        // prize scene
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            logSystem.IsRecording = false;

            prizeDropController.gameObject.SetActive(true);
            prizeDropController.IsRunning = true;

            targetPickupController.cleanUp();
            targetPickupController.gameObject.SetActive(false);

            clawController.gameObject.transform.position = new Vector3(0.0f, 0.25f, 0.0f);
            clawController.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    /// <summary>
    /// Clean up all scenarios
    /// </summary>
    private void cleanUp()
    {
        
    }
}
