using UnityEngine;
using UnityEngine.Assertions;


public class TransformLinkerByName : TransformLinker {
    public string TargetGameObjectName;

    #region monobehaviour
    void Start () {
        Assert.IsNotNull(TargetGameObjectName, "Target game object name cannot be null. Please specify in the Editor.");
        Assert.IsFalse(TargetGameObjectName == "", "Target game object name cannot be empty. Please specify in the Editor.");
        GameObject targetGameObject = GameObject.Find(TargetGameObjectName);
        if (targetGameObject != null)
        {
            targetTransform = targetGameObject.transform;
        }
        else
        {
            Debug.Log("Unable to find Transform by name (" + TargetGameObjectName + "). Disabling TransformLinkerByName. ");
            GetComponent<TransformLinkerByName>().enabled = false;
        }
	}
    #endregion
}
