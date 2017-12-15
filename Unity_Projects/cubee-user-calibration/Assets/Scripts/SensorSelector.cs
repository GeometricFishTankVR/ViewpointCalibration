using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SensorSelector : MonoBehaviour {
    public Object[] Sensors;
    public Dropdown SensorDropdown;

    #region monobehaviour
    // Use this for initialization
    void Start () {
        Assert.IsNotNull(SensorDropdown, "Sensor dropdown cannot be null. Please specify an object in the Editor.");
        SensorDropdown.ClearOptions();

        List<string> sensorNames = new List<string>(Sensors.Length);
        foreach(Object sensor in Sensors)
        {
            sensorNames.Add(sensor.name);
        }

        SensorDropdown.AddOptions(new List<string>(sensorNames));
        OnDropdownChange(); // Set default
    }
    #endregion

    public void OnDropdownChange()
    {
        Object selectedSensor = null;
        foreach(Object sensor in Sensors)
        {
            if(sensor.name == SensorDropdown.options[SensorDropdown.value].text)
            {
                selectedSensor = sensor;
            }
        }
        SelectSensor(selectedSensor);
    }

    public void SelectSensor(Object sensor)
    {
        PersistentProjectStorage.Instance.SelectedHeadTracker = sensor;
    }
}
