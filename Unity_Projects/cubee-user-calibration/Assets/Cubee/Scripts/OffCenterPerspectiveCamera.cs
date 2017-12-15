using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class OffCenterPerspectiveCamera : MonoBehaviour {
    public int ScreenRefreshRate;
    public int ScreenPixelWidth;
    public int ScreenPixelHeight;
    
    public GameObject virtualScreenGameObject;
    public Material renderMaterial;
    protected Transform virtualScreen;
    private Camera thisCamera;

    public bool HasStaticNearPlane = false;
    public bool HasStaticFarPlane = false;
    public float StaticNearPlane = 0.3f;
    public float StaticFarPlane = 1000.0f;
    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(virtualScreenGameObject, "Virtual Screen Game Object is null. Please specify an object in the Editor.");
        Assert.IsNotNull(virtualScreenGameObject, "Render Material is null. Please specify an object in the Editor.");
        Assert.IsTrue(ScreenRefreshRate > 0, "Screen refresh rate must be greater than 0. Please fix in the Editor.");
        Assert.IsTrue(ScreenPixelWidth > 0, "Screen pixel width must be greater than 0. Please fix in the Editor.");
        Assert.IsTrue(ScreenPixelHeight > 0, " Screen pixel height must be greater than 0. Please fix in the Editor.");
        if (HasStaticFarPlane) Assert.IsTrue(StaticFarPlane > 0, "Far plane must be greater than 0.");
        if (HasStaticNearPlane) Assert.IsTrue(StaticNearPlane > 0, "Near plane must be greater than 0.");
        if (HasStaticFarPlane && HasStaticNearPlane) Assert.IsTrue(StaticFarPlane > StaticNearPlane, "Far plane must br greater than near plane.");

        virtualScreen = virtualScreenGameObject.transform;
        virtualScreenGameObject.GetComponent<Renderer>().material = renderMaterial;
        // Make the camera orthogonal to the screen. 
        // this is a requirement for a proper off-axis projection
        thisCamera = GetComponent<Camera>(); // cache the component
    }

    void Start () {
        int displayIdx = thisCamera.targetDisplay;

        if (displayIdx < Display.displays.Length)
        {
            thisCamera.targetTexture = null; // Switch to real-time output
            virtualScreenGameObject.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            Debug.Log("Not enough displays to activate this camera: " + gameObject.name + ". Switching to virtual output.");
            thisCamera.targetTexture = (RenderTexture)renderMaterial.mainTexture; // Switch to virtual output
        }
        OrientCamera();
    }

    void LateUpdate()
    {
        OrientCamera();
        Vector3 displacement = virtualScreen.position - transform.position;
        float forwardDistance = Vector3.Dot(displacement, transform.forward);
        bool isCameraInFrontOfScreen = forwardDistance > 0;
        if (!isCameraInFrontOfScreen)
        {
            // When the viewpoint moves to a position that would put this camera behind the screen
            // move the camera to right in front of the screen. Now, viewer other than the tracked viewpoint
            // will be able to look into the display
            transform.localPosition = transform.parent.InverseTransformPoint(virtualScreen.TransformPoint(Vector3.back));

            // Recalculate the displacement
            displacement = virtualScreen.position - transform.position;
            forwardDistance = Vector3.Dot(displacement, transform.forward);
        }
        thisCamera.nearClipPlane = forwardDistance;
            
        float rightDistance = Vector3.Dot(displacement, transform.right);
        float upDistance = Vector3.Dot(displacement, transform.up);

        float halfScreenWidth = virtualScreen.lossyScale.x / 2;
        float halfScreenHeight = virtualScreen.lossyScale.y / 2;

        thisCamera.farClipPlane = forwardDistance + Mathf.Max(virtualScreen.lossyScale.x, virtualScreen.lossyScale.y) * 2;

        float top = upDistance + halfScreenHeight; float bottom = upDistance - halfScreenHeight;
        float right = rightDistance + halfScreenWidth; float left = rightDistance - halfScreenWidth;
        thisCamera.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, thisCamera.nearClipPlane, thisCamera.farClipPlane);
    }
    #endregion

    protected virtual void OrientCamera()
    {
        // Make the camera orthogonal to the screen. 
        // this is a requirement for a proper off-axis projection
        transform.rotation = Quaternion.LookRotation(virtualScreen.forward, virtualScreen.up);
        transform.localPosition = Vector3.zero;
    }

    public void ChangeVirtualScreen(GameObject screen)
    {
        virtualScreenGameObject = screen;
        virtualScreen = virtualScreenGameObject.transform;
        virtualScreenGameObject.GetComponent<Renderer>().material = renderMaterial;
        OrientCamera();
    }

    // Unity docs example perspective off-center matrix
    public Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        // Compute the parts of the matrix that depend on the dynamic near/far plane values
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);

        // Change to static near/far plane if necessary
        if (HasStaticNearPlane) near = StaticNearPlane;
        if (HasStaticFarPlane) far = StaticFarPlane;

        // Compute the scaling parts of the matrix now
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x; m[0, 1] = 0; m[0, 2] = a; m[0, 3] = 0;
        m[1, 0] = 0; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0;
        m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c; m[2, 3] = d;
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = e; m[3, 3] = 0;
        return m;
    }
}
