using Malee;
using System.Collections.Generic;
using UnityEngine;

namespace Biglab.Input
{
    /// <summary>
    /// An implementation of the wiimote input source.
    /// </summary>
    public class WiimoteInputSource : CustomInputSource
    {
        /// <summary>
        /// Which Wiimote to listen from.
        /// </summary>
        public int WiimoteIndex;

        [Reorderable, SerializeField]
        private ButtonMapList Buttons;

        [Reorderable, SerializeField]
        private AxisMapList Axes;

        private Dictionary<string, List<WiimoteButton>> ButtonDict;
        private Dictionary<string, List<WiimoteButton>> AxisPositiveDict;
        private Dictionary<string, List<WiimoteButton>> AxisNegativeDict;

        [System.Serializable]
        private class ButtonMapList : ReorderableArray<ButtonMap> { }

        [System.Serializable]
        private class AxisMapList : ReorderableArray<AxisMap> { }

        private Wiimote Wiimote;
        private WiimoteManager WiimoteManager;

        void Start()
        {
            // Find Wiimote Manager
            WiimoteManager = FindObjectOfType<WiimoteManager>();

            // If Wiimote Manager isn't found, create one on this object.
            if( WiimoteManager == null )
                WiimoteManager = gameObject.AddComponent<WiimoteManager>();

            // When a wiimote is connected and it is the same index as desired, keep a reference to that wiimote.
            WiimoteManager.WiimoteConnected += ( index, wiimote ) =>
            {
                if( WiimoteIndex == index )
                    Wiimote = wiimote;
            };

            // Allocate button storage
            ButtonDict = new Dictionary<string, List<WiimoteButton>>();
            foreach( var button in Buttons )
                ButtonDict[button.Name] = new List<WiimoteButton>() { button.Button };

            // Allocate axis storage
            AxisPositiveDict = new Dictionary<string, List<WiimoteButton>>();
            AxisNegativeDict = new Dictionary<string, List<WiimoteButton>>();
            foreach( var axis in Axes )
            {
                AxisPositiveDict[axis.Name] = new List<WiimoteButton>() { axis.PositiveButton };
                AxisNegativeDict[axis.Name] = new List<WiimoteButton>() { axis.NegativeButton };
            }
        }

        internal protected override float GetAxis( string name )
        {
            // We don't have a wiimote, so return neutral state.
            if( Wiimote == null ) return 0F;
            else
            {
                // If there is a definition for the positive axis as button state.
                if( AxisPositiveDict.ContainsKey( name ) )
                {
                    // Try all known button names
                    foreach( var btn in AxisPositiveDict[name] )
                        if( Wiimote.GetButton( btn ) ) return +1F;
                }

                // If there is a definition for the negative axis as button state.
                if( AxisNegativeDict.ContainsKey( name ) )
                {
                    // Try all known button names
                    foreach( var btn in AxisNegativeDict[name] )
                        if( Wiimote.GetButton( btn ) ) return -1F;
                }

                // Neutral axis state
                return 0F;
            }
        }

        internal protected override bool GetButton( string name )
        {
            // We don't have a wiimote, so return released state.
            if( Wiimote == null ) return false;
            else
            {
                // If there is a definition for the button.
                if( ButtonDict.ContainsKey( name ) )
                {
                    // Try all known button names
                    foreach( var btn in ButtonDict[name] )
                        if( Wiimote.GetButton( btn ) ) return true;
                }

                // Released button state
                return false;
            }
        }

        [System.Serializable]
        private class ButtonMap
        {
            /// <summary>
            /// The name/alias of the button.
            /// </summary>
            public string Name = "Jump";

            /// <summary>
            /// Which button on the wiimote.
            /// </summary>
            public WiimoteButton Button = WiimoteButton.A;
        }

        [System.Serializable]
        private class AxisMap
        {
            /// <summary>
            /// The name/alias of the axis.
            /// </summary>
            public string Name = "Horizontal";

            /// <summary>
            /// Which button on the wiimote represents the positive axis.
            /// </summary>
            public WiimoteButton PositiveButton = WiimoteButton.Right;

            /// <summary>
            /// Which button on the wiimote represents the negative axis.
            /// </summary>
            public WiimoteButton NegativeButton = WiimoteButton.Left;
        }
    }
}
