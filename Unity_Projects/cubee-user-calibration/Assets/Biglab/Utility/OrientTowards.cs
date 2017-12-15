using UnityEngine;

namespace Biglab
{
    /// <summary>
    /// Orients an object to point towards the given transform.
    /// </summary>
    public class OrientTowards : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        [Tooltip( "Target object to orient this object." )]
        public Transform TargetTransform;

        [Tooltip( "Either the linear rate ( in degrees ) or a percent." )]
        public float InterpolationFactor = 0.25F;

        [Tooltip( "Should this use linear difference or percentage interpolation." )]
        public bool Linear = false;

        [Tooltip( "Negates the direction vector to flip which side of the object is facing the target." )]
        public bool FlipFront = false;

        void Start()
        {
            Debug.Assert( TargetTransform != null, "Must specify target transform" );
        }

        void Update()
        {
            // 
            var dir = ( TargetTransform.position - transform.position ).normalized;
            if( FlipFront ) dir = -dir;

            //
            var rot = Quaternion.LookRotation( dir );

            // Interpolates at a rate of 180 degree per second
            if( Linear ) transform.rotation = Interpolator.Slerp( transform.rotation, rot, InterpolationFactor );
            else transform.rotation = Quaternion.Slerp( transform.rotation, rot, InterpolationFactor );
        }
    }
}