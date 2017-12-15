using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// temp: this script might be deleted once Quaternion has been setup properly

public class HeadInDisplay : MonoBehaviour {

    private Vector3 axisX;
    private Vector3 axisY;
    private Vector3 axisZ;
    public Text textbox;

    public void Update()
    {
        if (textbox != null)
        {
            textbox.text = transform.position.ToString();
        }
    }

    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position= value; }
    }

    public Vector3 AxisX 
    {
        get { return axisX; }
        set { axisX = value; transform.right = axisX; }
    }

    public Vector3 AxisY
    {
        get { return axisY; }
        set { axisY = value; //transform.up = axisY; 
        }
    }

    public Vector3 AxisZ
    {
        get { return axisZ; }
        set { axisZ = value; //transform.forward = axisZ; 
        }
    }
}
