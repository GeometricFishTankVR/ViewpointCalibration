using UnityEngine;

namespace Biglab
{
    /// <summary>
    /// Read only attribute, marks a non-custom-editor field as read only. <para/>
    /// This prevents accidental editing from the inspector while still allowing viewing the field in the inspector.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Determines if the field is readonly always or only during play.
        /// </summary>
        public readonly bool OnlyDisableWhenPlaying;

        /// <summary>
        /// Marks a field as read only for the inspector.
        /// </summary>
        /// <param name="onlyDisableWhenPlaying"> A flag to allow editing when not in play mode. </param>
        public ReadOnlyAttribute( bool onlyDisableWhenPlaying = false )
        {
            OnlyDisableWhenPlaying = onlyDisableWhenPlaying;
        }
    }
}