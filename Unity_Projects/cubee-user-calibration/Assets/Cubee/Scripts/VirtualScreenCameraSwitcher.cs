using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class VirtualScreenCameraSwitcher : MonoBehaviour {
    public OffCenterPerspectiveCamera[] cameras;
    public Dropdown swapDropdown1;
    public Dropdown swapDropdown2;

    void OnEnable()
    {
        Assert.IsNotNull(swapDropdown1, "Swap Dropdown 1 cannot be null. Please specify an object in the Editor.");
        Assert.IsNotNull(swapDropdown2, "Swap Dropdown 2 cannot be null. Please specify an object in the Editor.");
        Assert.IsNotNull(cameras, "Cameras array cannot be null. Please specify an object in the Editor.");

        if (cameras.Length < 2)
        {
            Debug.Log("Less than 2 cameras are set. A screen switcher is not needed. Disabling screen switcher.");
            gameObject.SetActive(false);
        }
        else
        {
            // Set defaults
            List<string> screenNames = new List<string>(cameras.Length);
            foreach (OffCenterPerspectiveCamera camera in cameras)
            {
                screenNames.Add(camera.virtualScreenGameObject.name);
            }
            swapDropdown1.ClearOptions();
            swapDropdown2.ClearOptions();
            swapDropdown1.AddOptions(screenNames);
            swapDropdown2.AddOptions(screenNames);
        }
    }

    public void SwapScreensBetweenCameras()
    {
        OffCenterPerspectiveCamera cameraToSwap1 = cameras[0];
        OffCenterPerspectiveCamera cameraToSwap2 = cameras[0];

        foreach(OffCenterPerspectiveCamera camera in cameras)
        {
            if(camera.virtualScreenGameObject.name == swapDropdown1.options[swapDropdown1.value].text)
            {
                cameraToSwap1 = camera;
            }
            if(camera.virtualScreenGameObject.name == swapDropdown2.options[swapDropdown2.value].text)
            {
                cameraToSwap2 = camera;
            }
        }

        GameObject tmp = cameraToSwap1.virtualScreenGameObject;
        cameraToSwap1.ChangeVirtualScreen(cameraToSwap2.virtualScreenGameObject);
        cameraToSwap2.ChangeVirtualScreen(tmp);
    }
}
