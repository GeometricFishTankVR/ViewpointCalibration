using UnityEngine;

namespace Biglab.Input
{
    /// <summary>
    /// A custom input source is a way to inject input from other devices / systems that aren't possoble to use with the built in Unity input system.
    /// For example, a Wiimote.
    /// </summary>
    public abstract class CustomInputSource : MonoBehaviour
    {
        /// <summary>
        /// Is the given button held down?
        /// </summary>
        internal protected abstract bool GetButton( string name );

        /// <summary>
        /// Gets a -1.0 to +1.0 value of the given axis state.
        /// </summary>
        internal protected abstract float GetAxis( string name );
    }
}
