using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour {
    public string[] SceneNames;
    public Dropdown SceneDropdown;   

    // Use this for initialization
	void Start () {
        Assert.IsNotNull(SceneDropdown, "Scene dropdown cannot be null. Please specify an object in the Editor.");
        SceneDropdown.ClearOptions();
        SceneDropdown.AddOptions(new List<string>(SceneNames));
        SceneDropdown.RefreshShownValue();
    }

    public void OnButtonPress()
    {
        ChangeToScene(SceneDropdown.options[SceneDropdown.value].text);
    }

    public void ChangeToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
