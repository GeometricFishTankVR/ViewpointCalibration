using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
public class ClawAnimationController : MonoBehaviour {

    public float propRotationSpeed;

    private GameObject flyClawParent;
    private Transform propTranform;
    private Animator clawAnimator;
    private FlyingClawController clawController;
    private AudioSource[] audioSources;
    #region monobehaviour
    void onEnable()
    {
        Assert.IsTrue(propRotationSpeed > 0, "Invalid propeller rotation speed value.");
    }
    void Start () {
        flyClawParent = this.transform.parent.gameObject;
        propTranform = this.transform.FindChild("propeller");
        clawAnimator = this.GetComponent<Animator>();
        audioSources = this.GetComponents<AudioSource>();
        clawController = flyClawParent.GetComponent<FlyingClawController>();
    }
	
	void Update () {
        propellerRotation();
	}
    #endregion

    /// <summary>
    /// public functions called by FlyingClawController
    /// </summary>
    public void grabObject()
    {
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<keyevent>ClawWillReachTarget</keyevent></t>", Time.time.ToString("F3")));
        clawAnimator.SetTrigger("isGrabbed");
    }
    
    public void missObject()
    {
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<keyevent>ClawWillUnderReachTarget</keyevent></t>", Time.time.ToString("F3")));
        clawAnimator.SetTrigger("isMissed");
    }

    public void overReachObject()
    {
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<keyevent>ClawWillOverReachTarget</keyevent></t>", Time.time.ToString("F3")));
        clawAnimator.SetTrigger("isOverReached");
    }

    public void triggerClaw()
    {
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawWillOpen</event></t>", Time.time.ToString("F3")));
        clawAnimator.SetTrigger("isTriggered");
    }

    public void releaseObject()
    {
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawWillRelease</event></t>", Time.time.ToString("F3")));
        clawAnimator.SetTrigger("isReleased");
    }

    public bool isGrabbed()
    {
        return clawAnimator.GetCurrentAnimatorStateInfo(0).IsName("ClawGrabGet");
    }

    public bool isIdle()
    {
        return clawAnimator.GetCurrentAnimatorStateInfo(0).IsName("ClawIdle");
    }

    /// <summary>
    /// Auto-called inside ClawClose animation clip
    /// </summary>
    public void clawReset()
    {
        clawAnimator.SetTrigger("isReset");
        //clawAnimator.ResetTrigger("isTriggered");
        //clawAnimator.ResetTrigger("isMissed");
        //clawAnimator.ResetTrigger("isGrabbed");
        //clawAnimator.ResetTrigger("isOverReached");
        //clawAnimator.ResetTrigger("isReleased");
    }

    /// <summary>
    /// Auto-called inside ClawIdle animation clip
    /// </summary>
    public void clawInit()
    {
        clawAnimator.ResetTrigger("isReset");
        clawAnimator.ResetTrigger("isTriggered");
        clawAnimator.ResetTrigger("isMissed");
        clawAnimator.ResetTrigger("isGrabbed");
        clawAnimator.ResetTrigger("isOverReached");
        clawAnimator.ResetTrigger("isReleased");
    }

    /// <summary>
    /// Auto-called inside ClawRelease animation clip
    /// </summary>
    public void clawRelease()
    {
        clawAnimator.ResetTrigger("isReset");

        clawController.detachTargetFromClaw();
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawReleased</event></t>", Time.time.ToString("F3")));
    }

    /// <summary>
    /// Auto-called inside ClawGrabGet animation clip
    /// </summary>
    public void clawGrabGet()
    {
        clawAnimator.ResetTrigger("isReset");
        clawAnimator.ResetTrigger("isReleased");
        
        clawController.attachTargetToClaw();
        audioSources[1].Play();
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawGotTarget</event></t>", Time.time.ToString("F3")));
    }

    /// <summary>
    /// Auto-called inside ClawBounceBack animation clip
    /// </summary>
    private void clawOverReach()
    {
        audioSources[2].Play();
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawOverReachedTarget</event></t>", Time.time.ToString("F3")));
    }

    /// <summary>
    /// Auto-called inside ClawGrabGet animation clip
    /// </summary>
    private void clawGrab()
    {
        audioSources[0].Play();
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawUnderReachedTarget</event></t>", Time.time.ToString("F3")));
    }

    /// <summary>
    /// Auto-called inside ClawRelease animation clip
    /// </summary>
    private void clawComplete()
    {
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawCompetedTarget</event></t>", Time.time.ToString("F3")));
        clawController.nextActionOfScenario();
    }

    /// <summary>
    /// Propeller self-rotation
    /// </summary>
    private void propellerRotation()
    {
        float angle = propRotationSpeed * Time.deltaTime;
        propTranform.Rotate(new Vector3(0, 1, 0), angle, 0);
    }

}
