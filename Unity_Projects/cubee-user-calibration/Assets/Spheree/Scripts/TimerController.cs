using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TimerController : MonoBehaviour {
    
    public GameObject targetScenario;
    public Transform viewerTransform;
    public float maxTime;

    //private bool timeOut;
    //private bool isActive;
    //private float startTime;
    //private float currentTime;
    private TextMesh textMesh;
    private AudioSource[] audioSources;
    private float remainTime;
 
	void Start () {
        textMesh = this.GetComponent<TextMesh>();
        setupTextMesh();
                
        audioSources = this.GetComponents<AudioSource>(); // sound effect0 : timeout // soundeffect1 : tictac
        Assert.IsTrue(audioSources.Length >= 2, "Not enough audio sources attached to Timer.");

        TimerOn = false;
        Pause = false;

        remainTime = maxTime;
	}
	
    void Update () 
    {
        rotateText();
    }

    public void showCompleteMsg()
    {
        
        textMesh.text = "Complete!";
    }

    public bool TimerOn
    {
        get
        {
            return timerOn;
        }
        set
        {
            timerOn = value;
            if (timerOn)
            {
                Pause = false;
                setupTextMesh();
                //Debug.logger.Log(Time.time.ToString("F3"), "Timer ON.");
                Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>TimerON</event></t>", 
                    Time.time.ToString("F3")));

                audioSources[1].Play();
                StopCoroutine("Timer");
                StartCoroutine("Timer");
            }
            else
            {
                audioSources[1].Stop();
                StopCoroutine("Timer");
                textMesh.text = "";
            }
        
        }
    
    }

    private bool timerOn;

    public bool Pause 
    {
        get { return pause;}
        set { 
            pause = value;
            if (pause)
            {
                TimerOn = false;
                displayTime(remainTime);
                textMesh.color = Color.green;
                Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<event>TimerPause</event></t>",
                Time.time.ToString("F3")));
            }
        }
    }

    private bool pause;

    IEnumerator Timer()
    {
        float startTime = Time.time;
        float currentTime = startTime;
        float elapseTime = currentTime - startTime;
        
        while (elapseTime <= maxTime)
        {
            remainTime = maxTime - elapseTime;
            displayTime(remainTime);

            currentTime = Time.time;
            elapseTime = currentTime - startTime;
            yield return new WaitForSeconds(0.1f);
        }

        audioSources[1].Stop();
        textMesh.text = "Time Out! Restart...";
        audioSources[0].Play(); // timeout sound effect
        //Debug.logger.Log(Time.time.ToString("F3"), "Time out!");
        yield return new WaitForSeconds(2.0f);
        
        targetScenario.BroadcastMessage("targetTimeOut");
        yield return null;
    }

    private void displayTime(float remainTime)
    {
        float seconds = remainTime % 60;
        float fractions = (remainTime * 100) % 100;
        string textTime = string.Format("{0:00}:{1:00}", seconds, fractions);
        textMesh.text = textTime;
        if (remainTime < 5.0f)
            textMesh.color = Color.red;
    }


    //public void startTimer()
    //{
    //    startTime = Time.time;
    //    timeOut = false; 
    //}

    //public void resetTimer()
    //{
    //    startTime = 0;
    //    timeOut = true;
    //}



    //public void turnOnTimer(bool isOn)
    //{
    //    if (isOn)
    //    {
    //        textMesh.text = currentTime.ToString();
    //    }
    //    else 
    //    {
    //        textMesh.text = "";
    //        startTime = 0;
    //        timeOut = true;
    //    }
    //    isActive = isOn;
    //}

    private void setupTextMesh()
    {
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontSize = 80;
        textMesh.fontStyle = FontStyle.Bold;
        textMesh.text = "";
        textMesh.color = new Color(255, 255, 255, 100);
    }

    //private void updateTimer()
    //{
    //    currentTime = Time.time;

    //    float elapseTime = currentTime - startTime;
    //    float remainTime = maxTime - elapseTime;
        
    //    if (remainTime >= 0)
    //    {
    //        float seconds = remainTime % 60;
    //        float fractions = (remainTime * 100) % 100;
    //        string textTime = string.Format("{0:00}:{1:00}", seconds, fractions);
    //        textMesh.text = textTime;
    //    }
    //    else
    //    {
    //        textMesh.text = "Time Out";
    //        timeOut = true;
    //        isActive = false;
    //        targetScenario.BroadcastMessage("targetTimeOut");
    //    }
    //}

    private void rotateText()
    {
        this.transform.rotation = viewerTransform.rotation;
    }


}
