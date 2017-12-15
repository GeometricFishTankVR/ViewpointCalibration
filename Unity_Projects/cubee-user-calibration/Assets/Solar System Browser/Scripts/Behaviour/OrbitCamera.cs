using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
// Author: Christopher Chamberlain - 2017
{
    [Tooltip( "If specified, will orbit around this object." )]
    public Transform Target;

    [Header( "Zoom Limits" )]
    [Tooltip( "Minimum distance the camera will zoom." )]
    public float MinDistance = 6F;

    [Tooltip( "Maximum distance the camera will zoom." )]
    public float MaxDistance = 12F;

    [Header( "Sensitivity" )]
    [Tooltip( "The sensitivity of the mouse on the x-axis." )]
    public float HeadingSensitivity = 0.1F;

    [Tooltip( "The sensitivity of the mouse on the y-axis." )]
    public float PitchSensitivity = 0.1F;

    [Tooltip( "The sensitivity of the mouse wheel." )]
    public float ZoomSensitivity = 0.5F;

    private Camera Camera;

    private Vector2 MouseLast;

    private float Distance;
    private float Heading;
    private float Pitch;

    void Start()
    {
        //
        Camera = GetComponent<Camera>();

        //
        MouseLast = Input.mousePosition;

        // 
        Distance = Mathf.Lerp( MinDistance, MaxDistance, 0.75F );
        Heading = 0;
        Pitch = 0;
    }

    void Update()
    {
        // Mouse delta
        var delta = ( (Vector2) Input.mousePosition ) - MouseLast;
        MouseLast = Input.mousePosition;

        // If left-mouse is held down
        if( Input.GetMouseButton( 0 ) )
        {
            // 
            Heading += delta.x * HeadingSensitivity;
            Pitch += delta.y * PitchSensitivity;

            // Limit 
            Pitch = Mathf.Clamp( Pitch, -89, +89 );
        }

        // 
        Distance -= Input.mouseScrollDelta.y * ZoomSensitivity;
        Distance = Mathf.Clamp( Distance, MinDistance, MaxDistance );

        // Compute rotation
        var rot = Quaternion.Euler( Pitch, Heading, /* Roll */ 0 );
        var vec = rot * Vector3.forward;

        // Position camera
        var target = GetTargetPosition();
        Camera.transform.position = target + ( vec * Distance );
        Camera.transform.LookAt( target );
    }

    Vector3 GetTargetPosition()
    {
        if( Target == null ) return Vector3.zero;
        else return Target.position;
    }
}
