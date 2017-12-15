using UnityEngine;

[ExecuteInEditMode]
public class ScalableSpheree : MonoBehaviour {

    public Transform sphereSurface;

    private Matrix4x4 surfaceTransform;

    #region monobehaviour

    private void Start()
    {
        surfaceTransform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, sphereSurface.transform.localScale/2);
        Shader.SetGlobalMatrix("sphere_transform", Matrix4x4.identity);
    }

    void FixedUpdate () {
        Shader.SetGlobalMatrix("sphere_scale", Matrix4x4.TRS(Vector3.zero, Quaternion.identity, surfaceTransform.MultiplyVector(transform.localScale)));
    }
    #endregion
}
