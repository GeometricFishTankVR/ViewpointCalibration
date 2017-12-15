using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AnamorphosisScatterer : MonoBehaviour {
    public Object Image;
    public float MaximumScatterDistance;
    public float ViewingDistance;

    private GameObject imageCopy;
    private GameObject viewPoint;
    public Vector3 ViewingPosition;

    #region monobehaviour
    // Use this for initialization
    void OnEnable()
    {
        Assert.IsNotNull(Image, "Image cannot be null. Please specify an object in the Editor.");
        Assert.IsTrue(MaximumScatterDistance > 0, "Scatter distance must be greater than 0. Please fix in the Editor.");
        Assert.IsTrue(ViewingDistance > 0, "Viewing distance must be greater than 0. Please fix in the Editor.");
    }

    void Start()
    {
        ViewingPosition = transform.up * ViewingDistance;
    }
    #endregion

    public void ScatterChildren()
    {
        ResetChildren();
        foreach (Transform child in imageCopy.transform)
        {
            float distanceToViewpoint = Vector3.Distance(child.localPosition, ViewingPosition);
            Vector3 displacementDirection = (child.localPosition - ViewingPosition).normalized;
            float displacmentMagnitude = Random.Range(0, MaximumScatterDistance);
            Vector3 displacement = displacementDirection * displacmentMagnitude;
            child.localPosition += displacement;
           
            float scaleFactor = (distanceToViewpoint + displacmentMagnitude) / distanceToViewpoint;
            child.localScale *= scaleFactor;
        }
    }


    public void ResetChildren()
    {
        if(imageCopy != null)
        {
            DestroyImmediate(imageCopy);
        }
        imageCopy = Instantiate(Image, transform) as GameObject;
        if (viewPoint != null)
        {
            DestroyImmediate(viewPoint);
        }
        viewPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        viewPoint.transform.parent = transform;
        viewPoint.transform.localPosition = ViewingPosition;
        viewPoint.transform.localScale = Vector3.one * 0.05f;
    }
}
