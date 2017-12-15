using UnityEngine;
using UnityEngine.Assertions;

public class TransformLinkerByReference : TransformLinker {
    public GameObject TargetGameObject;

    #region monobehaviour
    void Start()
    {
        Assert.IsNotNull(TargetGameObject, "Target game object cannot be null. Please specify in the Editor.");
        targetTransform = TargetGameObject.transform;
    }
    #endregion
}
