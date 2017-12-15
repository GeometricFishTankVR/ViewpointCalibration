using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientTowards : MonoBehaviour
{
    public Transform TargetTransform;

    [Range( 0F, 1F )]
    [Tooltip( "Percent to interpolate per-frame." )]
    public float InterpolationFactor = 0.1F;

    void Update()
    {
        // 
        var dir = ( TargetTransform.position - transform.position ).normalized;
        var rot = Quaternion.LookRotation( dir );

        // Interpolates at a rate of 90 degree per second
        // transform.rotation = Interpolator.Slerp( transform.rotation, rot, 90.0F );
        transform.rotation = Quaternion.Slerp( transform.rotation, rot, InterpolationFactor );
    }
}
