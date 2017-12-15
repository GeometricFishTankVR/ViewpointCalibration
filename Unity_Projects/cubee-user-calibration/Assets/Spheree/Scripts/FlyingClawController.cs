using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class FlyingClawController : MonoBehaviour {

    public TargetPickupController targetSceneController;
    public Transform viewPoint; // virtual camera viewpoint w/out rotation 
    public TimerController timeController;
    public HeadInDisplay headInDisplay; // actual viewpoint w/ rotation 

    private GameObject targetObject;
    private ClawAnimationController clawAnimController;
    private Rigidbody rigidBody;
    private GameObject clawObject;
    private GameObject clawTip;
    private Vector3 offSet;
    private GameObject[] clawFingers;
    private Rigidbody targetRigidBody;
    private string handednessString;
    private TargetPickup.targetInfo targetData;
    
    void OnEnable()
    {
        Assert.IsNotNull(targetSceneController, "targetSceneController can't be null in FlyingClawController.");
        Assert.IsNotNull(viewPoint, "viewPoint can't be null in FlyingClawController.");
        Assert.IsNotNull(timeController, "timeController can't be null in FlyingClawController.");
    }

	void Start () {
        rigidBody = this.GetComponent<Rigidbody>();
        
        clawAnimController = this.GetComponentInChildren<ClawAnimationController>();
        clawObject = this.transform.Find("flying-claw/claw").gameObject;
        clawTip = clawObject.transform.Find("Tip").gameObject;
        clawFingers = GameObject.FindGameObjectsWithTag("claw_finger");

        setCurrentTarget(null);
        computeOffset(); //offset caused by animationclip

        this.targetData = targetSceneController.targetData;

	}
	
	void Update () {
        handleInput();
	}

    void OnCollisionEnter(Collision other)
    {
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawCollidedOn</event>\r\n<name>{1}</name></t>", 
            Time.time.ToString("F3"),
            other.gameObject.name));

    }

    /// <summary>
    /// Xbox Controller will control the claw's position
    /// </summary>
    void FixedUpdate()
    {
        float movehorizontal;
        float movevertical;
        float moveheight;

        movehorizontal = Input.GetAxis("Horizontal" + handednessString);
        movevertical = Input.GetAxis("Vertical" + handednessString);
        moveheight = Input.GetAxis("Height" + handednessString);
        
        Vector3 move = new Vector3(movehorizontal, moveheight, movevertical);

        if (Vector3.Distance(move, Vector3.zero) > .001f)
        {
            float theta = Mathf.Atan(viewPoint.position.x / viewPoint.position.z);

            if (theta > 0)
            {
                if (viewPoint.position.x >= 0)
                    theta += Mathf.PI;
            }
            else
            {
                if (viewPoint.position.z >= 0)
                    theta += Mathf.PI;
            }

            if (Vector3.Dot(viewPoint.transform.up, headInDisplay.AxisX) < 0)
            {
                Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawControlFlipped</event></t>", Time.time.ToString("F3")));
                move.x = -move.x;
                move.z = -move.z;
            }           

            Vector3 new_move = Quaternion.Euler(0, Mathf.Rad2Deg * theta, 0) * move;

            float speed = 10.0f;
            rigidBody.AddForce(new_move * speed);

        }

    }

    public void setCurrentTarget(GameObject target)
    {
        targetObject = target;
        if (targetObject!=null)
            targetRigidBody = targetObject.GetComponent<Rigidbody>();
    }

    public void attachTargetToClaw()
    {
        targetObject.transform.SetParent(this.transform, true);
        targetObject.transform.position = new Vector3(clawTip.transform.position.x, targetObject.transform.position.y, clawTip.transform.position.z);
        if (timeController.TimerOn == true)
            timeController.Pause = true;
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawAttachedTarget</event></t>", Time.time.ToString("F3")));
    }

    public void detachTargetFromClaw()
    {
        targetObject.transform.SetParent(targetSceneController.gameObject.transform, true);
        targetRigidBody.isKinematic = false;
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawDetachedTarget</event></t>", Time.time.ToString("F3")));
    }

    public void nextActionOfScenario()
    {
        targetSceneController.nextAction();
        timeController.Pause = false;
        //turnOnClawCollider(true);// turn on claw mesh collider 
    }

    public void reset()
    {
        clawAnimController.clawReset();
        targetRigidBody.isKinematic = true;

        targetObject.transform.SetParent(targetSceneController.gameObject.transform, true);
        this.transform.position = Vector3.zero;
    }

    private void handleInput()
    {
        if (Input.GetButtonDown("Grab"))
        {
            Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>GrabButtonPressed</event></t>", Time.time.ToString("F3")));

            clawAnimController.triggerClaw();

            if (targetObject != null && clawAnimController.isIdle())
            {
                float THRESHOLD = targetObject.transform.lossyScale.x;
                float distance = computeClawObjectDistance();
                float direction = Mathf.Sign(distance);
                Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawGrabDistanceToTarget</event>\r\n<distance>{1}</distance></t>", 
                    Time.time.ToString("F3"),
                    distance.ToString("F4")));
                // determine states based on distance and direction
                if (Mathf.Abs(distance) < THRESHOLD) //grab successfully
                {
                    targetSceneController.turnOffHighlight();
                    clawAnimController.grabObject();

                    this.targetData = targetSceneController.targetData;
                    float deltatime = Time.time - targetData.setupTime;
                    Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<targetinfo><tid>{1}</tid><distance>{2}</distance><diameter>{3}</diameter><deltatime>{4}</deltatime><erate>{5}</erate><trate>{6}</trate></targetinfo></t>",
                                                     Time.time.ToString("F3"),
                                                     targetData.index,
                                                     targetData.distance,
                                                     targetData.diameter,
                                                     deltatime,
                                                     targetData.errorRate,
                                                     targetData.timeoutRate));                    
                }
                else if (direction < 0) //grab fail, overreach
                {
                    clawAnimController.overReachObject();
                    targetSceneController.targetData.errorRate++;
                }
                else //grab fail, underreach 
                {
                    clawAnimController.missObject();
                    targetSceneController.targetData.errorRate++;
                }
            }
            else
            {
                Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ClawNoTarget</event></t>", Time.time.ToString("F3")));
                clawAnimController.missObject(); // no target object
            }
        }

        if (Input.GetButtonDown("Release") && targetObject != null)
        {
            Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>ReleaseButtonPressed</event></t>", Time.time.ToString("F3")));

            if (clawAnimController.isGrabbed())
                clawAnimController.releaseObject();
            //else
                //Debug.logger.Log(Time.time.ToString("F3"), "Claw can not release, not in grab state.");
        }

    }

    /// <summary>
    /// callback in viewerOptionSelector
    /// </summary>
    /// <param name="hand"></param>
    public void setHandedOption(ViewerOption.HandOption hand)
    {
        if (hand == ViewerOption.HandOption.Left)
            handednessString = "LH";
        else
            handednessString = "RH";
    }

    private void computeOffset()
    {
        Transform flyingclawtransform = this.transform.FindChild("flying-claw");
        offSet = flyingclawtransform.TransformPoint(0.0f, 0.0f, -0.4f) - flyingclawtransform.TransformPoint(0.0f, 0.0f, -0.25f);
    }

    private float computeClawObjectDistance()
    {
        Vector3 clawTipPosition = clawTip.transform.position;
        clawTipPosition += offSet;
        float distance = Vector3.Distance(targetObject.transform.position, clawTipPosition);
        distance = distance * Mathf.Sign(clawTipPosition.y - targetObject.transform.position.y);
        return distance; 
    }

    private void turnOnClawCollider(bool isOn)
    {
        foreach(GameObject finger in clawFingers)
        {
            Collider f_collider = finger.GetComponent<Collider>();
            f_collider.enabled = isOn;
        }
    }
}
