using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SphereeVirtualCamera : MonoBehaviour {
    public Transform SphereeCenter;
    public Transform ViewPoint;
    private Camera thisCamera;

    #region monobehaviour
    void Start()
    {
        Assert.IsNotNull(SphereeCenter, "Sphere Center cannot be null. Please specify an object in the Editor.");
        Assert.IsNotNull(ViewPoint, "View point cannot be null. Please specify an object in the Editor.");
        thisCamera = gameObject.GetComponent<Camera>();

    }

    void FixedUpdate()
    {
        // Update Position
        transform.localPosition = ViewPoint.localPosition;
        //transform.localRotation = ViewPoint.localRotation;
        // Look at Spheree GameObject
        transform.LookAt(SphereeCenter.position, Vector3.up);

        // dynamic fov
        float distanceToSpheree = Vector3.Distance(transform.position, SphereeCenter.position);
        float scaledRadius = SphereeCenter.localScale.x / 2 * transform.parent.localScale.x;

        thisCamera.nearClipPlane = 0.01f; //QZ: make near plane close; allow objects outside the sphere right in front of viewer
        thisCamera.farClipPlane = distanceToSpheree + scaledRadius * 2; // QZ: extend further
        // Calculate FoV of the camera
        Vector3 separation = transform.position - SphereeCenter.position;
        //thisCamera.fieldOfView = 2 * Mathf.Atan(transform.parent.localScale.x * SphereeCenter.localScale.x / (2 * separation.magnitude)) * Mathf.Rad2Deg;

        //thisCamera.fieldOfView *= 1.2f;// QZ: Give a little buffer for objects that are right on the sphere 
        thisCamera.fieldOfView = 2 * Mathf.Asin(transform.parent.localScale.x * SphereeCenter.localScale.x / (2*separation.magnitude)) * Mathf.Rad2Deg; 

    }
    void Update()
    {
        Vector4 positionHomogenous = transform.position;
        positionHomogenous.w = 1;

        // Pass camera matrices to shader
        Shader.SetGlobalMatrix("view_V", thisCamera.worldToCameraMatrix);
        Shader.SetGlobalMatrix("view_P", thisCamera.projectionMatrix);
        Shader.SetGlobalMatrix("inv_view_V", thisCamera.cameraToWorldMatrix);
        Shader.SetGlobalVector("view_pos", positionHomogenous);
    }
    #endregion
}
