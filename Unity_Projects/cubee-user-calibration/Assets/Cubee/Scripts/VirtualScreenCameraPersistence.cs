using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class VirtualScreenCameraPersistence : MonoBehaviour {
    public string displayName;
    public OffCenterPerspectiveCamera[] cameras;
    public GameObject[] screens;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(displayName, "Display Name cannot be null. Please set a name in the Editor.");
        Assert.IsFalse(displayName == "", "Display Name cannot be empty. Please set a name in the Editor.");
        Assert.IsNotNull(cameras, "Cameras array cannot be null. Please set an array in the Editor.");
        Assert.IsNotNull(screens, "Screens array cannot be null. Please set an array in the Editor.");
        Assert.IsTrue(cameras.Length == screens.Length, "Cameras array must be the same length as the Screen array. Please fix in the Editor.");
        if (cameras.Length < 2 || screens.Length < 2)
        {
            gameObject.SetActive(false);
            Debug.Log("Not enough cameras/screens to be useful. Disabling: " + gameObject.name);
        }
    }

    void Start () {
        LoadMappings();
	}
    #endregion

    private string GetMappingKey(GameObject camera)
    {
        return displayName + "_" + camera.name;
    }

    public void SaveMappings()
    {
        foreach(OffCenterPerspectiveCamera camera in cameras)
        {
            string key = GetMappingKey(camera.gameObject);
            string value = camera.virtualScreenGameObject.name;
            PlayerPrefs.SetString(key, value);
            Debug.Log("Saved mapping: " + camera.gameObject.name + " : " + value);
        }
    }

    public void LoadMappings()
    {
        List<string> screensTaken = new List<string>();
        List<OffCenterPerspectiveCamera> camerasNeeded = new List<OffCenterPerspectiveCamera>();
        foreach(OffCenterPerspectiveCamera camera in cameras)
        {
            string key = GetMappingKey(camera.gameObject);
            string screenName = PlayerPrefs.GetString(key, "none");
            if(screenName == "none") // No screen saved, so add the camera to the list of cameras that need screens
            {
                camerasNeeded.Add(camera);
            }
            else
            {
                foreach(GameObject screen in screens)
                {
                    if(screen.name == screenName)
                    {
                        camera.ChangeVirtualScreen(screen);
                        screensTaken.Add(screenName);
                        Debug.Log("Loaded mapping: " + camera.gameObject.name + " : " + screen.name);
                    }
                }
            }
        }

        // Assign screens to cameras that still need screens
        foreach(OffCenterPerspectiveCamera camera in camerasNeeded)
        {
            foreach(GameObject screen in screens)
            {
                if(!screensTaken.Contains(screen.name))
                {
                    camera.ChangeVirtualScreen(screen);
                    screensTaken.Add(screen.name);
                    Debug.Log("Loaded mapping: " + camera.gameObject.name + " : " + screen.name);
                    break;
                }
            }
        }
    }
}
