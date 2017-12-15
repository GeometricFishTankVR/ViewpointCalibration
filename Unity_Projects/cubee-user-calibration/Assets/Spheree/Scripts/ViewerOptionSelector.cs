using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace ViewerOption
{
    public enum EyeOption { Left, Right, Both };
    public enum HandOption { Right, Left };
}

public class ViewerOptionSelector : MonoBehaviour {
    
    public string[] handednessString;
    public string[] viewingEyeString;
    public SphereeTrackerCalibrationTransformer trackerTransformerController;
    public FlyingClawController clawController;

    public Dropdown handednessDropdown;
    public Dropdown viewingEyeDropdown;

    void Start()
    {
        setupDropdown(handednessDropdown, handednessString);
        setupDropdown(viewingEyeDropdown, viewingEyeString);
        handednessDropdown.onValueChanged.AddListener(delegate
        {
            OnHandDropdownChange();
        });
        viewingEyeDropdown.onValueChanged.AddListener(delegate
        {
            OnEyeDropdownChange();
        });

        OnEyeDropdownChange(); //default
        OnHandDropdownChange(); //default
    }

    void Destroy()
    {
        handednessDropdown.onValueChanged.RemoveAllListeners();
        viewingEyeDropdown.onValueChanged.RemoveAllListeners();
    }
 

    public ViewerOption.EyeOption eyeOption 
    {
        get { return eyeoption; }
        set { eyeoption = value; }
    }

    public ViewerOption.HandOption handOption
    {
        get { return handoption; }
        set { handoption = value; }
    }

    private ViewerOption.EyeOption eyeoption;
    private ViewerOption.HandOption handoption;

    private void setupDropdown(Dropdown dropdown, string[] dd_names)
    {
        Assert.IsNotNull(dropdown, "Scene dropdown cannot be null. Please specify an object in the Editor.");
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(dd_names));
        dropdown.RefreshShownValue();
    }

    public void OnEyeDropdownChange()
    {
        eyeOption = (ViewerOption.EyeOption)viewingEyeDropdown.value;
        trackerTransformerController.setOffSet(eyeOption);
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<eye>{1}</eye></t>", 
            Time.time.ToString("F3"),
            Enum.GetName(typeof(ViewerOption.EyeOption), eyeOption)));

    }
    public void OnHandDropdownChange()
    {
        handOption = (ViewerOption.HandOption)handednessDropdown.value;
        clawController.setHandedOption(handOption);
        Debug.logger.Log(string.Format("<t><time>{0}</time>\r\n<hand>{1}</hand></t>",
            Time.time.ToString("F3"),
            Enum.GetName(typeof(ViewerOption.HandOption), handOption)));
    }

}
