using UnityEngine;
using Biglab.Input;

namespace Biglab.UI
{
    /// <summary>
    /// A component/object that represents a sort of 'laser-pointer' based input for VR and the Spheree project.
    /// </summary>
    public class VirtualPointer : MonoBehaviour
    {
        /// <summary>
        /// Input system component.
        /// </summary>
        public CustomInput Input;

        /// <summary>
        /// The primary button that represents a 'click' on an interface/button.
        /// </summary>
        public string PrimaryButton;

        /// <summary>
        /// The color of the laser graphic.
        /// </summary>
        public Color Color;

        private float _LaserLength;

        [SerializeField]
        private Material _Material;

        /// <summary>
        /// Length of the laser pointer visual.
        /// </summary>
        public float LaserLength
        {
            get { return _LaserLength; }

            set
            {
                _LaserLength = value;
                _LaserLength = Mathf.Max( 0.01F, _LaserLength + 0.01F );
                LineRenderer.SetPosition( 1, new Vector3( 0, 0, _LaserLength ) );
            }
        }

        private LineRenderer LineRenderer;

        /// <summary>
        /// The ray this pointer object is oriented.
        /// </summary>
        public Ray Ray
        {
            get
            {
                var pos = transform.position;
                var dir = transform.forward;
                return new Ray( pos, dir );
            }
        }

        internal bool GetButtonDown()
        {
            return Input.GetButtonDown( PrimaryButton );
        }

        internal bool GetButtonUp()
        {
            return Input.GetButtonUp( PrimaryButton );
        }

        void Start()
        {
            _Material = new Material( _Material );
            _Material.SetColor( "_TintColor", Color );
            AttachLineRenderer();
        }

        private void AttachLineRenderer()
        {
            LineRenderer = gameObject.AddComponent<LineRenderer>();
            LineRenderer.widthMultiplier = 0.01F;
            LineRenderer.useWorldSpace = false;
            LineRenderer.material = _Material;

            LaserLength = _LaserLength;
        }
    }
}