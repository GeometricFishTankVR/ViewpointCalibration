using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Biglab.Input
{
    public class WiimoteDebugUI : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        private Dictionary<WiimoteButton, Text> ButtonMap;
        private Text AccelText;

        /// <summary>
        /// The index of the wiimote to observe.
        /// </summary>
        [Tooltip( "The index of the wiimote to observe." )]
        public int WiimoteIndex = 0;

        private WiimoteManager WiimoteManager;

        void Start()
        {
            // 
            WiimoteManager = FindObjectOfType<WiimoteManager>();

            // 
            ButtonMap = new Dictionary<WiimoteButton, Text>();
            foreach( WiimoteButton button in Enum.GetValues( typeof( WiimoteButton ) ) )
            {
                var child = transform.Find( string.Format( "B_{0}", button ) );
                ButtonMap[button] = child.GetComponent<Text>();
            }

            // 
            AccelText = transform.Find( "Accel" ).GetComponent<Text>();
        }

        void Update()
        {
            if( WiimoteManager == null )
            {
                WiimoteManager = FindObjectOfType<WiimoteManager>();
                return;
            }

            // 
            if( WiimoteManager.IsConnected( WiimoteIndex ) )
            {
                var wiimote = WiimoteManager.Get( WiimoteIndex );

                AccelText.text = "" + wiimote.Accelerometer;

                // 
                foreach( var pair in ButtonMap )
                {
                    var held = wiimote.GetButton( pair.Key );
                    var down = wiimote.GetButtonDown( pair.Key );
                    var up = wiimote.GetButtonUp( pair.Key );

                    var text = pair.Value;

                    // Instant states
                    if( down ) text.color = Color.yellow;
                    else if( up ) text.color = Color.red;
                    // Continuous states
                    else if( held ) text.color = Color.gray;
                    else text.color = Color.black;
                }
            }
        }
    }
}