using UnityEngine;
using System.Collections;

public class TrackballCamera : MonoBehaviour
{

    public float distance = 100;
    public const float minDistance = 1.0f;
    public const float maxDistance = 300.0f;
    public float virtualTrackballDistance = 0.25f; // distance of the virtual trackball.

    public GameObject target;
    private Vector3? lastMousePosition;
    // Use this for initialization
    void Start()
    {
        var startPosn = (this.transform.position - target.transform.position).normalized * distance;
        var position = startPosn + target.transform.position;
        transform.position = position;
        transform.LookAt(target.transform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var mousePosn = Input.mousePosition;
        // update distance
       
        var mouseBtn = Input.GetMouseButton(0);
        
        if (mouseBtn)
        {
            if (lastMousePosition.HasValue)
            {
               
                // we are moving from here
                var lastPosn = this.transform.position;
                var targetPosn = target.transform.position;

                var rotation = FigureOutAxisAngleRotation(lastMousePosition.Value, mousePosn);

                var vecPos = (targetPosn - lastPosn).normalized * -distance;

                this.transform.position = rotation * vecPos + targetPosn;
                this.transform.LookAt(targetPosn);

                lastMousePosition = mousePosn;
            }
            else {
                lastMousePosition = mousePosn;
            }
        }
        else {

            lastMousePosition = null;
        }
        var scrollWhl = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWhl < 0 || scrollWhl > 0)
        {
            distance = distance - Mathf.Sign(scrollWhl) * Mathf.Pow(Mathf.Abs(scrollWhl), 0.5f) * 10.0f;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            var lastPosn = this.transform.position;
            var targetPosn = target.transform.position;
            var vecPos = (targetPosn - lastPosn).normalized * -distance;

            this.transform.position = vecPos + targetPosn;
        }

    }



    Quaternion FigureOutAxisAngleRotation(Vector3 lastMousePosn, Vector3 mousePosn)
    {
        if (lastMousePosn.x == mousePosn.x && lastMousePosn.y == mousePosn.y)
            return Quaternion.identity;

        Vector3 near = new Vector3(0, 0, Camera.main.nearClipPlane);

        Vector3 p1 = Camera.main.ScreenToWorldPoint(lastMousePosn + near);
        Vector3 p2 = Camera.main.ScreenToWorldPoint(mousePosn + near);

        //WriteLine("## {0} {1}", p1,p2);
        var axisOfRotation = Vector3.Cross(p2, p1);

        var twist = (p2 - p1).magnitude / (2.0f * virtualTrackballDistance);

        if (twist > 1.0f)
            twist = 1.0f;
        if (twist < -1.0f)
            twist = -1.0f;

        var phi = (2.0f * Mathf.Asin(twist)) * 180 / Mathf.PI;

        //WriteLine("AA: {0} angle: {1}",axisOfRotation, phi);

        return Quaternion.AngleAxis(phi, axisOfRotation);
    }
}