using UnityEngine;
using UnityEngine.Assertions;

public class CameraTrackball : MonoBehaviour {
    public Transform Focus;
    public float MinimumZoomDistance;
    public float MaximumZoomDistance;
    public float Sensitivity;
    public int MouseButton;

    private Vector3 previousMousePosition;
    private Vector3 currentMousePosition;
    private Vector3 targetSphericalPosition;
    private const float PixelToRadian = 0.003f;

    private bool isMouseButtonDown;


    #region monobehaviour
    void OnEnable()
    {
        Assert.IsNotNull(Focus, "Focus cannot be null. Please specify an object in the Editor.");
        Assert.IsTrue(MinimumZoomDistance > 0, "Minimum zoom distance must be greater than 0.");
        Assert.IsTrue(MaximumZoomDistance > 0, "Maximum zoom distance must be greater than 0.");
        Assert.IsTrue(MaximumZoomDistance > MinimumZoomDistance, "Maximum zoom distance must be greater than minimum zoom distance.");
        Assert.IsTrue(MouseButton == 0 || MouseButton == 1 || MouseButton == 2, "Mouse button can only be either be 0, 1, or 2.");
    }

    void Start()
    {
        targetSphericalPosition = CubeeMath.CartesianToSpherical(transform.localPosition);
    }

	void Update () {
        float DisplayScale = Focus.lossyScale.x;

        currentMousePosition = Input.mousePosition;
        if (Input.GetMouseButtonDown(MouseButton))
        {
            isMouseButtonDown = true;
            previousMousePosition = currentMousePosition;
        }
        if (Input.GetMouseButtonUp(MouseButton))
        {
            isMouseButtonDown = false;
        }

        if (isMouseButtonDown)
        {
            
            Vector3 deltaMousePosition = currentMousePosition - previousMousePosition;

            
            float deltaTheta = deltaMousePosition.x * PixelToRadian * Sensitivity;
            float deltaPhi = deltaMousePosition.y * PixelToRadian * Sensitivity;
            targetSphericalPosition += new Vector3(0, deltaTheta * Mathf.Sin(targetSphericalPosition.z), deltaPhi);
            targetSphericalPosition.y = targetSphericalPosition.y % (2 * Mathf.PI);
            targetSphericalPosition.z = targetSphericalPosition.z % (2 * Mathf.PI);
            previousMousePosition = currentMousePosition;
        }

        float deltaR = -Input.GetAxis("Mouse ScrollWheel") * DisplayScale / 2 * Sensitivity;
        targetSphericalPosition += new Vector3(deltaR, 0, 0);
        targetSphericalPosition.x = Mathf.Clamp(targetSphericalPosition.x, MinimumZoomDistance * DisplayScale, MaximumZoomDistance * DisplayScale);

        Vector3 targetPosition = CubeeMath.SphericalToCartesian(targetSphericalPosition);


        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 10);
        transform.rotation = Quaternion.LookRotation(Focus.transform.position - transform.position);
    }
    #endregion
}
