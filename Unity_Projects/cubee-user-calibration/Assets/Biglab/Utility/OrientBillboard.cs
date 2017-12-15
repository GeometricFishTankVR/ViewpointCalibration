using UnityEngine;

namespace Biglab
{
    /// <summary>
    /// Orients an object to be a 'billboard' by rotating it to align with the camera forward vector.
    /// </summary>
    public class OrientBillboard : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        /// <summary>
        /// Target camera. If not specified, will use the main camera.
        /// </summary>
        [Tooltip( "Target camera. If not specified, will use the main camera." )]
        public Camera TargetCamera;

        /// <summary>
        /// Negates the forward vector to flip which side of the object looks at the camera.
        /// </summary>
        [Tooltip( "Negates the forward vector to flip which side of the object looks at the camera." )]
        public bool FlipFront = false;

        void Start()
        {
            Orient();
        }

        void LateUpdate()
        {
            Orient();
        }

        void Orient()
        {
            var target = TargetCamera;
            if( target == null ) target = Camera.main;

            // 
            var dir = target.transform.forward;
            if( FlipFront ) dir = -dir;

            // Compute and assign rotation
            transform.rotation = Quaternion.LookRotation( dir );
        }
    }
}