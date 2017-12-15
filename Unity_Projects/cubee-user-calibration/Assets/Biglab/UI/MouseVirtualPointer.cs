using UnityEngine;

namespace Biglab.UI
{
    using UnityInput = UnityEngine.Input;

    /// <summary>
    /// A utilty that animates a virtual pointer with the mouse.
    /// </summary>
    public class MouseVirtualPointer : MonoBehaviour
    {
        /// <summary>
        /// Which virtual pointer to animate.
        /// </summary>
        [Tooltip( "Which virtual pointer to animate." )]
        public VirtualPointer VirtualPointer;

        private void Start()
        {
            if( VirtualPointer == null )
                VirtualPointer = FindObjectOfType<VirtualPointer>();
        }

        private void Update()
        {
            var mpos = UnityInput.mousePosition;
            var mray = Camera.main.ScreenPointToRay( mpos );
            VirtualPointer.transform.rotation = Quaternion.LookRotation( mray.direction );
            VirtualPointer.transform.position = mray.origin;
        }
    }
}
