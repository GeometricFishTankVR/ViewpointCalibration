using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewpointCalibrationModifier : MonoBehaviour {
    public float mainAxisSpeed = 0.001f;
    public float secondaryAxisSpeed = 0.00005f;
    public float xtdTranslationSpeed = 0.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float mainHorizontalAxis = Input.GetAxis("Horizontal");
        float mainVerticalAxis = Input.GetAxis("Vertical");
        float secondaryHorizontalAxis = Input.GetAxis("XboxOneDPADHorizontal");
        float secondaryVerticalAxis = Input.GetAxis("XboxOneDPADVertical");
        int posXTranslation = Input.GetKey(KeyCode.Q)?1:0;
        int negXTranslation = Input.GetKey(KeyCode.A)?1:0;
        int posYTranslation = Input.GetKey(KeyCode.W)?1:0;
        int negYTranslation = Input.GetKey(KeyCode.S)?1:0;
        int posZTranslation = Input.GetKey(KeyCode.E)?1:0;
        int negZTranslation = Input.GetKey(KeyCode.D)?1:0;



        ViewpointCalibration calibration = PersistentProjectStorage.Instance.CurrentViewpointCalibration;
        if(calibration != null)
        {

            Vector3 oVfix = Vector3.zero;
            oVfix.x = mainAxisSpeed*mainHorizontalAxis + secondaryAxisSpeed*secondaryHorizontalAxis;
            oVfix.y = mainAxisSpeed*mainVerticalAxis + secondaryAxisSpeed*secondaryVerticalAxis;

            calibration.oV += oVfix;
            calibration.ApplyTranslationToXTD(new Vector3(posXTranslation - negXTranslation, posYTranslation - negYTranslation, posZTranslation - negZTranslation) * xtdTranslationSpeed);
            
        }
	}
}
