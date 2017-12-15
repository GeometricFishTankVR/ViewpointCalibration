using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

namespace TargetPickup
{
    public enum Mode { Practice, Task };
    public struct targetInfo 
    {
        public int index;
        public int errorRate;
        public int timeoutRate;
        public float setupTime;
        public float diameter;
        public float distance;
    }
}

[RequireComponent(typeof(ParticleSystem))]

public class TargetPickupController : MonoBehaviour {

    public GameObject flyClaw;
    public GameObject Timer;
    public UnityEngine.Object applePrefab;
    public Vector2 targetDiameterRange;
    public HightLightTargetController highLightController;
    public LogSystem logSystem;

    private TargetPickup.Mode mode;
    private bool isRunning;
    private int numOfTargets;
    private int currentTargetIdx;

    private TimerController timeController;
    private FlyingClawController flyClawController;
    private Transform flyClawTipTransformation;

//    private ParticleSystem targetHighLight;

    private GameObject[] targetSpots;
    private List<GameObject> targetGroup = new List<GameObject>();
    public TargetPickup.targetInfo targetData = new TargetPickup.targetInfo();


    void Start () {
        //logSystem = this.transform.parent.gameObject.GetComponent<LogSystem>();
        
        mode = TargetPickup.Mode.Practice;
        isRunning = false;
        numOfTargets = 0;
        currentTargetIdx = -1;

        timeController = Timer.GetComponent<TimerController>();
        flyClawController = flyClaw.GetComponent<FlyingClawController>();
        flyClawTipTransformation = flyClaw.transform.Find("flying-claw/claw/Tip");
//        targetHighLight = this.transform.Find("particle-highlight/particle").gameObject.GetComponent<ParticleSystem>();
        highLightController.HighLightOn = false;
	}
	
	void Update () {
        if (isRunning)
        {
            handleInput();
        }
	}

    /// <summary>
    /// setup the next target; this function is called by flying claw controller
    /// </summary>
    public void nextAction() {
        currentTargetIdx++;
        if (currentTargetIdx < numOfTargets)
        {
            if (mode == TargetPickup.Mode.Task)
                timeController.TimerOn = true;
            activateTarget(currentTargetIdx, true);
            flyClawController.setCurrentTarget(targetGroup[currentTargetIdx]);
        }
        else
        {
            timeController.TimerOn = false;
            timeController.showCompleteMsg(); // end of scenario
            //Debug.logger.Log("\r\n");
            //Debug.logger.Log(Time.time.ToString("F3"), "Complete claw scene.");
            Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<keyevent>ClawSceneCompleted</keyevent></t>", Time.time.ToString("F3")));

            logSystem.IsRecording = false;
        }
    }

    /// <summary>
    /// deal with time out case; this function is a broadcast call by timer
    /// </summary>
    void targetTimeOut()
    {
        //Debug.logger.Log(Time.time.ToString("F3"), string.Format("Target No.{0} times out. Reset this target.", currentTargetIdx));
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<keyevent>TimeOutTarget</keyevent><tid>{1}</tid></t>", 
            Time.time.ToString("F3"),
            currentTargetIdx));

        // reset claw position and animation state
        flyClawController.reset();

        // reset target object position and rotation
        targetGroup[currentTargetIdx].transform.position = targetSpots[currentTargetIdx].transform.position;
        targetGroup[currentTargetIdx].transform.rotation = targetSpots[currentTargetIdx].transform.rotation;

        // activate object again and start timer
        timeController.TimerOn = true;
        activateTarget(currentTargetIdx, true, true);
    }

    /// <summary>
    /// activate this scene and specify the mode and number of targets
    /// </summary>
    /// <param name="m"></param>
    /// <param name="num_target"></param>
    public void run(TargetPickup.Mode m, int num_target)
    {
        Debug.logger.Log("\r\n");
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<keyevent>ClawSceneStarted</keyevent> <name>{1}</name></t>", 
            Time.time.ToString("F3"),
            Enum.GetName(typeof(TargetPickup.Mode), m)));

        isRunning = true;
        createTargets(num_target);       
        mode = m;
    }

    /// <summary>
    /// Destroy previous targets
    /// </summary>
    public void cleanUp()
    {
        isRunning = false;
//        targetHighLight.Stop();
        highLightController.HighLightOn = false;
        timeController.TimerOn = false;

        if (targetGroup.Count > 0) 
        {
            //Debug.logger.Log(Time.time.ToString("F3"), string.Format("Clean up {0} previous pickups.", targetGroup.Count));
            Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>CleanUp{1}PreviouTargets</event></t>", 
                Time.time.ToString("F3"),
                targetGroup.Count));

            for (int i = targetGroup.Count-1; i >=0; i--)
                Destroy(targetGroup[i]);
            targetGroup.Clear(); // this only clears the list; objects are not destroyed
        }      
    }

    /// <summary>
    /// called in FlyingClawController
    /// </summary>
    /// <returns></returns>
    public TargetPickup.targetInfo getTargetInfo()
    {
        return targetData;
    }

    /// <summary>
    /// call in FlyClawController
    /// </summary>
    public void turnOffHighlight()
    {
 //       targetHighLight.Stop();
        highLightController.HighLightOn = false;
    }

    private void handleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //trigger the start of pickup scene
        {
            currentTargetIdx = 0;
            activateTarget(currentTargetIdx, true);
            if (mode == TargetPickup.Mode.Task)
                timeController.TimerOn = true;
            flyClawController.setCurrentTarget(targetGroup[currentTargetIdx]);
        }
    }

    private void activateTarget(int idx, bool isActive, bool isReactivated = false)
    {
        targetGroup[idx].SetActive(isActive);
        highLightController.attachParticleToObject(targetGroup[idx].transform);
        highLightController.HighLightOn = true;

        float distance = Vector3.Distance(targetGroup[idx].transform.position, flyClawTipTransformation.position);
        
        // record target id, size, pos, and distance to current cursor position
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<keyevent>ActivatedTargetNum</keyevent>\r\n<tid>{1}</tid>\r\n<position>{2}</position>\r\n<diameter>{3}</diameter>\r\n<distance>{4}</distance>\r\n</t>",
            Time.time.ToString("F3"),
            idx,
            targetGroup[idx].transform.position.ToString("F4"),
            targetGroup[idx].transform.lossyScale.ToString("F4"),
            distance.ToString("F4")));

        targetData.diameter = targetGroup[idx].transform.lossyScale.x;
        targetData.distance = distance;
        targetData.index = idx;
        targetData.setupTime = Time.time;
        targetData.errorRate = 0;

        if (isReactivated)
        {
            targetData.timeoutRate++;

        }
        else
        {
            targetData.timeoutRate = 0;
        }
    }


    private void createTargets(int t_num)
    {
        targetSpots = GameObject.FindGameObjectsWithTag("target_spot");

        Assert.IsNotNull(targetSpots, "Target spot has not been setup in the scene.");
        Assert.IsNotNull(applePrefab, "Apple prefab has not been setup in the editor.");
        Assert.IsTrue(targetSpots.Length + 1 >= t_num, "Target number is larger than max target spot.");

        numOfTargets = (targetSpots.Length + 1 >= t_num)? t_num : targetSpots.Length;

        if (targetGroup != null)
            targetGroup.Clear();

        for (int idx = 0; idx < numOfTargets; idx++)
        {
            GameObject appleObject = (GameObject)Instantiate(applePrefab, targetSpots[idx].transform.position, targetSpots[idx].transform.rotation);
            appleObject.transform.localScale = appleObject.transform.localScale * UnityEngine.Random.Range(targetDiameterRange.x, targetDiameterRange.y);
            appleObject.transform.parent = this.transform;
            appleObject.name = "apple-target" + (idx + 1).ToString();
            appleObject.tag = "target_object";
            targetGroup.Add(appleObject);
            appleObject.SetActive(false);
        }
        
        //targetGroup = GameObject.FindGameObjectsWithTag("target_object");
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>Create{1}Targets</event></t>",
                                        Time.time.ToString("F3"),
                                        targetGroup.Count));
        Assert.IsNotNull(targetGroup, "Target group setup fails.");
    }
}
