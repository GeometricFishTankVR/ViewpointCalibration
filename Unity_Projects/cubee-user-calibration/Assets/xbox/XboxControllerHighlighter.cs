using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class XboxControllerHighlighter : MonoBehaviour {
    public enum XboxControllerInput { A, B, X, Y, RightStick, LeftStick, Dpad, LeftBumper, RightBumper, LeftTrigger, RightTrigger, Menu, View};
    public GameObject Abutton;
    public GameObject Bbutton;
    public GameObject Xbutton;
    public GameObject Ybutton;
    public GameObject RightStick;
    public GameObject LeftStick;
    public GameObject Dpad;
    public GameObject LeftBumper;
    public GameObject RightBumper;
    public GameObject LeftTrigger;
    public GameObject RightTrigger;
    public GameObject Menu;
    public GameObject View;

    protected Dictionary<XboxControllerInput, GameObject> xboxInputMap;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(Abutton, "A button cannot be null.");
        Assert.IsNotNull(Bbutton, "B button cannot be null.");
        Assert.IsNotNull(Xbutton, "X button cannot be null.");
        Assert.IsNotNull(Ybutton, "Y button cannot be null.");
        Assert.IsNotNull(RightStick, "Right stick cannot be null.");
        Assert.IsNotNull(LeftStick, "Left stick cannot be null.");
        Assert.IsNotNull(Dpad, "Dpad cannot be null.");
        Assert.IsNotNull(LeftBumper, "Left bumper cannot be null.");
        Assert.IsNotNull(RightBumper, "Right bumper cannot be null.");
        Assert.IsNotNull(LeftTrigger, "Left trigger cannot be null.");
        Assert.IsNotNull(RightTrigger, "Right trigger cannot be null.");
        Assert.IsNotNull(Menu, "Menu cannot be null.");
        Assert.IsNotNull(View, "View cannot be null.");
    }

    void Start () {
        xboxInputMap = new Dictionary<XboxControllerInput, GameObject>();
        xboxInputMap.Add(XboxControllerInput.A, Abutton);
        xboxInputMap.Add(XboxControllerInput.B, Bbutton);
        xboxInputMap.Add(XboxControllerInput.X, Xbutton);
        xboxInputMap.Add(XboxControllerInput.Y, Ybutton);
        xboxInputMap.Add(XboxControllerInput.RightStick, RightStick);
        xboxInputMap.Add(XboxControllerInput.LeftStick, LeftStick);
        xboxInputMap.Add(XboxControllerInput.Dpad, Dpad);
        xboxInputMap.Add(XboxControllerInput.LeftBumper, LeftBumper);
        xboxInputMap.Add(XboxControllerInput.RightBumper, RightBumper);
        xboxInputMap.Add(XboxControllerInput.LeftTrigger, LeftTrigger);
        xboxInputMap.Add(XboxControllerInput.RightTrigger, RightTrigger);
        xboxInputMap.Add(XboxControllerInput.Menu, Menu);
        xboxInputMap.Add(XboxControllerInput.View, View);
    }
    #endregion

    public void HighlightInput(XboxControllerInput input, float timeout = 0)
    {
        GameObject inputGameObject = xboxInputMap[input];
        TextureAlbedoPingPong script = inputGameObject.AddComponent<TextureAlbedoPingPong>();
        script.speed = 7;
        script.AlbedoColor = Color.white;

        if(timeout > 0)
        {
            StartCoroutine(HighlightInputAnimation(script, timeout));
        }
    }

    protected IEnumerator HighlightInputAnimation(TextureAlbedoPingPong script, float timeout)
    {
        yield return new WaitForSeconds(timeout);
        Destroy(script);
    }

    public void UnHighlightInput(XboxControllerInput input)
    {
        GameObject inputGameObject = xboxInputMap[input];
        foreach(Transform child in inputGameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
