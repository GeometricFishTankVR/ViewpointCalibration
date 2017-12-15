using UnityEngine;

public class HeadTrackerDoFSetter : MonoBehaviour {
    public bool is6DoF;

    #region monobehaviour
    void Start()
    {
        PersistentProjectStorage.Instance.IsHeadTracker6DoF = is6DoF;
    }
    #endregion
}
