using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereeProjectorMapper : MonoBehaviour {

    public Transform projectorParent;
    public string displayName;
    private Camera[] projectors;

    // Use this for initialization
    void Start () {
        projectors = new Camera[projectorParent.childCount];
        for (int i  = 0; i < projectorParent.childCount; i++)
        {
            projectors[i] = projectorParent.GetChild(i).gameObject.GetComponent<Camera>();
        }
        LoadMapping();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private string GetMappingKey(GameObject camera)
    {
        return displayName + "_" + camera.name;
    }

    /// <summary>
    /// Load the projector display mapping from player preferences
    /// </summary>
    public void LoadMapping()
    {

        List<int> screensTaken = new List<int>();
        List<Camera> projectorsNeeded = new List<Camera>();
        foreach (Camera projector in projectors)
        {
            string key = GetMappingKey(projector.gameObject);
            string screenName = PlayerPrefs.GetString(key, "none");
            if (screenName == "none") // No screen saved, so add the camera to the list of cameras that need screens
            {
                projectorsNeeded.Add(projector);
            }
            else
            {
                projector.targetDisplay = int.Parse(screenName);
                screensTaken.Add(int.Parse(screenName));
                Debug.Log("Loaded mapping: " + projector.gameObject.name + " : " + screenName);                
            }
        }

        // Assign screens to cameras that still need screens
        foreach (Camera projector in projectorsNeeded)
        {
            for (int i = 1; i <= projectors.Length; i++)
            {
                if (!screensTaken.Contains(i))
                {
                    projector.targetDisplay = i;
                    screensTaken.Add(i);
                    Debug.Log("Loaded mapping: " + projector.gameObject.name + " : " + i.ToString());
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Record the current projector display mapping to player preferences
    /// </summary>
    public void SaveMapping()
    {
        foreach (Camera projector in projectors)
        {
            string key = GetMappingKey(projector.gameObject);
            int value = projector.targetDisplay;
            PlayerPrefs.SetString(key, value.ToString());
            Debug.Log("Saved mapping: " + projector.gameObject.name + " : " + value.ToString());
        }
    }
}
