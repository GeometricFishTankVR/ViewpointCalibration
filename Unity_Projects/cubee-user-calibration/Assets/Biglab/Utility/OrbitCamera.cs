using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biglab
{
    using Input;
    using UnityInput = UnityEngine.Input;

    /// <summary>
    /// A behaviour to control an orbiting camera.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent( typeof( Camera ) )]
    public class OrbitCamera : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        [Tooltip( "Input system component." )]
        public CustomInput Input;

        [UnityEngine.Serialization.FormerlySerializedAs( "MouseButton" )]
        public string OrbitRotationButton = "Orbit";

        [Tooltip( "If specified, will orbit around this object." )]
        public Transform Target;

        [Header( "Zoom Range" )]
        [Tooltip( "Minimum distance the camera will zoom." )]
        public float MinDistance = 6F;

        [Tooltip( "Maximum distance the camera will zoom." )]
        public float MaxDistance = 12F;

        [Header( "Sensitivity" )]
        [Tooltip( "The sensitivity of the mouse on the x-axis." )]
        public float HeadingSensitivity = 0.1F;

        [Tooltip( "The sensitivity of the mouse on the y-axis." )]
        public float PitchSensitivity = 0.1F;

        [Tooltip( "The sensitivity of the mouse wheel ( percent of the zoom range per wheel-tick )." )]
        public float ZoomSensitivity = 0.5F;

        [Tooltip( "Apply target transform to the cameras parameters." )]
        public bool UseTargetRotationScale = false;

        private new Camera camera;
        private Vector2 mouseLast;

        [Header( "Parameters" )]
        public float distance;
        public float heading;
        public float pitch;

        void Start()
        {
            //
            camera = GetComponent<Camera>();
            Debug.Assert( camera != null, "OrbitCamera must be attached to an object with a camera." );

            //
            mouseLast = UnityInput.mousePosition;

            // Initial parameters
            //distance = MinDistance * GetTargetScale(); 
            //heading = 0;
            //pitch = 35;
        }

        private void OnValidate()
        {
            // Swap
            if( MaxDistance < MinDistance )
            {
                var realMin = MaxDistance;
                var realMax = MinDistance;
                MaxDistance = realMax;
                MinDistance = realMin;
            }

            // Clamp actual distance
            distance = Mathf.Clamp( distance, MinDistance, MaxDistance );
        }

        void Update()
        {
            // 
            if( Application.isPlaying )
            {
                // Mouse delta
                var delta = ( (Vector2) UnityInput.mousePosition ) - mouseLast;
                mouseLast = UnityInput.mousePosition;

                // If left-mouse is held down
                if( Input.GetButton( OrbitRotationButton ) )
                {
                    // Adjust heading and pitch for mouse input
                    heading += delta.x * HeadingSensitivity;
                    pitch -= delta.y * PitchSensitivity;

                    // Limit pitch to prevent it rolling over the 
                    // top or bottom of the spherical domain.
                    pitch = Mathf.Clamp( pitch, -89, +89 );
                }

                // Adjust zoom, and 
                var zoomRange = ( MaxDistance - MinDistance ) * ZoomSensitivity;
                distance -= UnityInput.mouseScrollDelta.y * zoomRange;
                distance = Mathf.Clamp( distance, MinDistance, MaxDistance );
            }

            // Compute rotation
            var tRotation = GetTargetRotation();
            tRotation = Quaternion.Euler( -pitch, heading, /* Roll */ 0 ) * tRotation;

            // Compute position
            var tPosition = GetTargetPosition();
            var offset = ( tRotation * Vector3.forward ) * ( GetTargetScale() * distance );

            // Position camera and look at target
            camera.transform.position = tPosition + offset;
            camera.transform.LookAt( tPosition );
        }

        #region Target Transform Components

        Vector3 GetTargetPosition()
        {
            if( Target == null ) return Vector3.zero;
            else return Target.position;
        }

        float GetTargetScale()
        {
            if( UseTargetRotationScale && Target != null )
                return Target.lossyScale.x;

            return 1F;
        }

        Quaternion GetTargetRotation()
        {
            if( UseTargetRotationScale && Target != null )
                return Target.rotation;

            return Quaternion.identity;
        }

        #endregion
    }
}