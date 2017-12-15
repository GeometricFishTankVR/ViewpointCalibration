using UnityEngine;

/// <summary>
/// Abstract Transform Linker class. Do not use this class directly on an object.
/// Use a subclass of this class.
/// </summary>
public abstract class TransformLinker : MonoBehaviour {
    public bool IsSourceTransformLocal;
    public bool IsTargetTransformLocal;
    protected Transform targetTransform;

    #region monobehaviour
    void FixedUpdate()
    {
        if (IsSourceTransformLocal)
        {
            if (IsTargetTransformLocal)
            {
                targetTransform.localPosition = transform.localPosition;
                targetTransform.localRotation = transform.localRotation;
            }
            else
            {
                targetTransform.position = transform.localPosition;
                targetTransform.rotation = transform.localRotation;
            }
        }
        else
        {
            if (IsTargetTransformLocal)
            {
                targetTransform.localPosition = transform.position;
                targetTransform.localRotation = transform.rotation;
            }
            else
            {
                targetTransform.position = transform.position;
                targetTransform.rotation = transform.rotation;
            }
        }
    }
    #endregion
}
