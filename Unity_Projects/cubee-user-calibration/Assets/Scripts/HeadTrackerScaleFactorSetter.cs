using UnityEngine;
using UnityEngine.Assertions;

public class HeadTrackerScaleFactorSetter : MonoBehaviour {
    public float HeadTrackerToDisplayScaleFactor;

    #region monobehaviour
    void OnEnable()
    {
        Assert.IsTrue(HeadTrackerToDisplayScaleFactor > 0, "Scale factor must be greater than 0. Please fix in the Editor.");
    }

    void Start () {
        PersistentProjectStorage.Instance.HeadTrackerToDisplayScaleFactor = HeadTrackerToDisplayScaleFactor;
	}
    #endregion
}
