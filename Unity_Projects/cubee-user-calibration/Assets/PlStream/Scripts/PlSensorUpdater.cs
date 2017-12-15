using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlSensorUpdater : MonoBehaviour
{
    public int SensorID;
    public PlStream PlStreamReader;

    private Vector3 pol_position;
    private Quaternion pol_rotation;
    private Vector3 unity_position;
    private Quaternion unity_rotation;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsTrue(SensorID >= 0, "Sensor ID must be positive. Please fix in the Editor.");
        Assert.IsNotNull(PlStreamReader, "PL Stream Reader cannot be null. please specify in the Editor.");
    }

    // Update is called once per frame
    void Update()
    {
        if (SensorID < PlStreamReader.active.Length && PlStreamReader.active[SensorID])
        {
            pol_position = PlStreamReader.positions[SensorID];
            Vector4 tmp = PlStreamReader.orientations[SensorID];
            pol_rotation = new Quaternion(tmp[1], tmp[2], tmp[3], tmp[0]);

            // Right-handed to left-handed
            unity_position.x = -pol_position.x;
            unity_position.y = pol_position.y;
            unity_position.z = pol_position.z;

            unity_rotation.w = pol_rotation.w;
            unity_rotation.x = pol_rotation.x;
            unity_rotation.y = -pol_rotation.y;
            unity_rotation.z = -pol_rotation.z;
        }
        #endregion
    }
}