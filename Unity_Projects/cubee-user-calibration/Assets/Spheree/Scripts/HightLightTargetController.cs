using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HightLightTargetController : MonoBehaviour {

    public Transform clawTransform;
    private ParticleSystem particleHighLight;
    private ParticleSystem.MainModule particleSetting;
    void onEnable()
    {
        Assert.IsNotNull(clawTransform, "HightLightTarget: clawTransform can not be null.");
    }

    void Start()
    {
        particleHighLight = transform.Find("particle").GetComponent<ParticleSystem>();
        particleSetting = particleHighLight.main;
	}
	
    void Update () {
        if (highlightOn)
        {
            float distance = Vector3.Distance(clawTransform.position, transform.position);
            if (distance < 0.5f)
                particleSetting.startColor = new Color(255, 255, 117, 0);
            else
                particleSetting.startColor = new Color(255, 255, 117, 255);
        }
	}

    public bool HighLightOn
    {
        get { return highlightOn; }
        set {
            highlightOn = value;
            
            if (highlightOn)
            {
                particleHighLight.Play();
            }
            else
            {
                particleHighLight.Stop();
            }
        }
    }

    private bool highlightOn = false;

    public void attachParticleToObject(Transform objtransform)
    {
        transform.position = objtransform.position;
        transform.Translate(0.0f, 0.08f, 0.0f); // lift up a bit
    }
}
