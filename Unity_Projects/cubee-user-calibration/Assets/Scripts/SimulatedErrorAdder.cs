using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SimulatedErrorAdder : MonoBehaviour {
    public ViewpointTransformerAddedError ViewpointTransformationScript;
    public enum ErrorMode { None, qVH, oV, qTD, tTD, tSD, qSD };
    public Text StatusText;
    protected ErrorMode currentErrorMode;
    private int currentErrorModeIdx;
    private List<ErrorMode> ErrorModes;

    public Transform[] Screens;
    private Vector3[] tSDs;
    private Quaternion[] qSDs;

    public Vector3 tSDerror = Vector3.zero;
    public Quaternion qSDerror = Quaternion.identity;

    private bool hasPressedDirectionalPad = false;
    private bool shouldAddqSDerror = false;
    private bool shouldAddtSDerror = false;

    #region monobehaviour
    // Use this for initialization
    void OnEnable()
    {
        Assert.IsNotNull(ViewpointTransformationScript, "Viewpoint transformation script cannot be null.");
        Assert.IsNotNull(Screens, "Screens cannot be null.");
        Assert.IsNotNull(StatusText, "Status text cannot be null.");
    }
    void Start()
    {
        ErrorModes = new List<ErrorMode>();
        ErrorModes.Add(ErrorMode.None);
        ErrorModes.Add(ErrorMode.qVH);
        ErrorModes.Add(ErrorMode.oV);
        ErrorModes.Add(ErrorMode.qTD);
        ErrorModes.Add(ErrorMode.tTD);
        ErrorModes.Add(ErrorMode.tSD);
        ErrorModes.Add(ErrorMode.qSD);
        currentErrorModeIdx = 0;
        currentErrorMode = ErrorModes[currentErrorModeIdx];
        tSDs = new Vector3[Screens.Length];
        qSDs = new Quaternion[Screens.Length];
        for(int i = 0; i < Screens.Length; i++)
        {
            tSDs[i] = Screens[i].localPosition;
            qSDs[i] = Screens[i].localRotation;
        }
        SetErrorType();
    }

    // Update is called once per frame
    void Update () {
        float axis = Input.GetAxis("HorizontalTurn");
        if (Mathf.Abs(axis) > 0.8f && hasPressedDirectionalPad == false)
        {
            int idxIncrementer = (int)Mathf.Sign(axis);
            currentErrorModeIdx = mod(currentErrorModeIdx + idxIncrementer, ErrorModes.Count);
            currentErrorMode = ErrorModes[currentErrorModeIdx];
            SetErrorType();
            hasPressedDirectionalPad = true;
        }
        else if (Mathf.Abs(Input.GetAxis("HorizontalTurn")) < 0.2f)
        {
            hasPressedDirectionalPad = false;
        }

        if(shouldAddqSDerror)
        {
            for (int i = 0; i < Screens.Length; i++)
            {
                Screens[i].localRotation = qSDerror * qSDs[i];
            }
        }
        else
        {
            for (int i = 0; i < Screens.Length; i++)
            {
                Screens[i].localRotation = qSDs[i];
            }
        }
        if(shouldAddtSDerror)
        {
            for (int i = 0; i < Screens.Length; i++)
            {
                Screens[i].localPosition = tSDerror + tSDs[i];
            }
        }
        else
        {
            for (int i = 0; i < Screens.Length; i++)
            {
                Screens[i].localPosition = tSDs[i];
            }
        }
    }
    #endregion

    private int mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
    private void SetErrorType()
    {
        ViewpointTransformationScript.shouldAddqVHerror = currentErrorMode == ErrorMode.qVH;
        ViewpointTransformationScript.shouldAddoVerror = currentErrorMode == ErrorMode.oV;
        ViewpointTransformationScript.shouldAddqTDerror = currentErrorMode == ErrorMode.qTD;
        ViewpointTransformationScript.shouldAddtTDerror = currentErrorMode == ErrorMode.tTD;
        shouldAddqSDerror = currentErrorMode == ErrorMode.qSD;
        shouldAddtSDerror = currentErrorMode == ErrorMode.tSD;
        StatusText.text = "Error #" + currentErrorMode.ToString();
    }
}
