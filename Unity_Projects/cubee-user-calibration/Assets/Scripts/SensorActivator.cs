using UnityEngine;

public class SensorActivator : MonoBehaviour {
    #region monobehaviour
    void Start () {
        Object selectedHeadTracker = PersistentProjectStorage.Instance.SelectedHeadTracker;
        if (selectedHeadTracker != null)
        {
            GameObject headTracker = Instantiate(selectedHeadTracker, Vector3.zero, Quaternion.identity) as GameObject;
            headTracker.SetActive(true);
        }
        else
        {
            Debug.Log("No head tracker selected.");
        }
	}
    #endregion
}
