using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StageActivator : MonoBehaviour {
    public GameObject Stage1Animator;
    public GameObject Stage2Animator;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(Stage1Animator, "Stage 1 animator cannot be null.");
        Assert.IsNotNull(Stage2Animator, "Stage 2 animator cannot be null.");
    }

    // Use this for initialization
    void Start () {
        Stage1Animator.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Callback for stage 1 completion
    void OnStage1Completed() 
    {
        Stage1Animator.SetActive(false);
    }
    #endregion
}
