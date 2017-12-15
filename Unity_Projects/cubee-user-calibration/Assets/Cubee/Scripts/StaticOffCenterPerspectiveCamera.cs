using UnityEngine;

public class StaticOffCenterPerspectiveCamera : OffCenterPerspectiveCamera {
    protected override void OrientCamera()
    {
        transform.rotation = Quaternion.LookRotation(virtualScreen.forward, virtualScreen.up);
        transform.position = virtualScreen.TransformPoint(-Vector3.forward);
    }

}
