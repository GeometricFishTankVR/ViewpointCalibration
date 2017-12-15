using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class VirtualScreen : MonoBehaviour {
    public string TransformationFile;
    public Matrix4x4 TransformationFromParent;
    public GameObject ParentScreen;

    void OnEnable()
    {
        Assert.IsNotNull(ParentScreen, "Parent Screen cannot be null. Please specify an object in the Editor.");
    }

    public void LoadIdentity()
    {
        TransformationFromParent = Matrix4x4.identity;
    }

    public void LoadTransformation()
    {
        Matrix4x4 m = Serializer.DeSerializeObject<Matrix4x4>(TransformationFile);
        if (m != default(Matrix4x4))
        {
            TransformationFromParent = m;
        }
        else
        {
            Debug.Log("Unable to load transformation file: " + TransformationFile);
        }
    }

    public void SaveTransformation()
    {
        Serializer.SerializeObject(TransformationFromParent, TransformationFile);
    }

    public void UpdateScreenOrientation()
    {
        transform.localPosition = ParentScreen.transform.localRotation * CubeeMath.ExtractTranslationFromMatrix(ref TransformationFromParent);
        transform.localRotation = ParentScreen.transform.localRotation * CubeeMath.ExtractRotationFromMatrix(ref TransformationFromParent);
    }
}
