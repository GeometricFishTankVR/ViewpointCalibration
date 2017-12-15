using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WiimoteApi;

namespace Biglab.Input
{
    /**
     * Information about a specific Wii Remote.
     */
    public class Wiimote : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        [Flags]
        private enum ButtonState : byte
        {
            Released = 0 << 1, // 001
            Pressed = 1 << 2,  // 010
            Frame = 1 << 3     // 100
        }

        /// <summary>
        /// Reference to the Flafla2 wiimote api instance. You should probably ignore this.
        /// </summary>
        internal WiimoteApi.Wiimote _Wiimote { get; private set; }

        private Dictionary<WiimoteButton, ButtonState> ButtonStateMap;

        /// <summary>
        /// Accelerometer calibration data for a standard white non-motionplus wiimote.
        /// </summary>
        readonly static int[,] DefaultAccelCalibration = new int[,]
        {
            { 529, 517, 632 },
            { 530, 624, 525 },
            { 426, 516, 528 }
        };

        [ReadOnly]
        public int Index;

        void Start()
        {
            // Maps out each wiimote button
            ButtonStateMap = new Dictionary<WiimoteButton, ButtonState>();
            foreach( WiimoteButton button in Enum.GetValues( typeof( WiimoteButton ) ) )
                ButtonStateMap[button] = ButtonState.Released;

            // Configures the wiimote
            _Wiimote = WiimoteApi.WiimoteManager.Wiimotes[Index];
            _Wiimote.SetupIRCamera( IRDataType.FULL );
            _Wiimote.SendDataReportMode( InputDataType.REPORT_BUTTONS_ACCEL_IR10_EXT6 );
            _Wiimote.SendPlayerLED( Index == 0, Index == 1, Index == 2, Index == 3 );
            _Wiimote.Accel.accel_calib = DefaultAccelCalibration; // Assign default calibration data

            // 
            Debug.LogFormat( "Connecting Remote {0} ( {1} )", Index, _Wiimote.Type );
        }

        void Update()
        {
            int status;
            while( true )
            {
                // Continuously poll device until no changes are found.
                status = _Wiimote.ReadWiimoteData();
                if( status <= 0 ) break; // Exit loop
            }

            // Update button states
            foreach( var button in ButtonStateMap.Keys.ToArray() )
            {
                var pressed = GetButton( button );
                var state = ButtonStateMap[button].HasFlag( ButtonState.Pressed );

                // Debug.LogFormat( "{0}: {1}", button, pressed );

                // Button is held, but record keeping says it was not?
                if( pressed )
                {
                    // 
                    ButtonStateMap[button] = ButtonState.Pressed;
                    if( !state ) ButtonStateMap[button] |= ButtonState.Frame;
                }
                // Button isn't held, but record keeping says it was?
                else
                {
                    ButtonStateMap[button] = ButtonState.Released;
                    if( state ) ButtonStateMap[button] |= ButtonState.Frame;
                }
            }

            /* If holding tab, 'calibration mode' */
            // if( Input.GetKey( KeyCode.Tab ) ) CalibrationMode();
        }

        private void CalibrationMode()
        {
            //// Calibrate X
            //if( Input.GetKeyDown( KeyCode.X ) ) _Wiimote.Accel.CalibrateAccel( AccelCalibrationStep.LEFT_SIDE_UP );

            //// Calibrate Y
            //if( Input.GetKeyDown( KeyCode.Y ) ) _Wiimote.Accel.CalibrateAccel( AccelCalibrationStep.EXPANSION_UP );

            //// Calibrate Z
            //if( Input.GetKeyDown( KeyCode.Z ) ) _Wiimote.Accel.CalibrateAccel( AccelCalibrationStep.A_BUTTON_UP );

            //// Print calibration array
            //if( Input.GetKeyDown( KeyCode.P ) )
            //{
            //    var str = "";
            //    var acc = _Wiimote.Accel.accel_calib;
            //    for( int a = 0; a < 3; a++ )
            //    {
            //        str += "{ ";
            //        for( int c = 0; c < 3; c++ )
            //            str += string.Format( "{0}{1}", acc[a, c], c < 2 ? ", " : "" );

            //        // 
            //        str += " }";
            //        if( a < 2 )
            //            str += ",";
            //        str += "\n";
            //    }

            //    // 
            //    Debug.Log( str );
            //}
        }

        /// <summary>
        /// Wiimote acceleration values.
        /// </summary>
        public Vector3 Accelerometer
        {
            get
            {
                if( _Wiimote == null ) return Vector3.zero;
                else
                {
                    var acc = _Wiimote.Accel.GetCalibratedAccelData();
                    return new Vector3( acc[0], acc[1], acc[2] );
                }
            }
        }

        /// <summary>
        /// Gets the IR position? 
        /// Completely arbitrary code, untested.
        /// </summary>
        public Vector2 IR
        {
            get
            {
                if( _Wiimote == null ) return Vector2.zero;
                else
                {
                    var ir = _Wiimote.Ir.GetIRMidpoint( true );
                    return new Vector2( ir[0], ir[1] );
                }
            }
        }

        /// <summary>
        /// Returns true for the frame the given button on the wiimote was initially pressed.
        /// </summary>
        public bool GetButtonDown( WiimoteButton button )
        {
            if( ButtonStateMap == null ) return false;
            return ButtonStateMap[button] == ( ButtonState.Pressed | ButtonState.Frame );
        }

        /// <summary>
        /// Returns true for the frame the given button on the wiimote was initially released.
        /// </summary>
        public bool GetButtonUp( WiimoteButton button )
        {
            if( ButtonStateMap == null ) return false;
            return ButtonStateMap[button] == ( ButtonState.Released | ButtonState.Frame );
        }

        /// <summary>
        /// Returns true while the given button on the wiimote is pressed.
        /// </summary>
        public bool GetButton( WiimoteButton button )
        {
            if( _Wiimote == null ) return false;
            else
            {
                switch( button )
                {
                    case WiimoteButton.A: return _Wiimote.Button.a;
                    case WiimoteButton.B: return _Wiimote.Button.b;
                    case WiimoteButton.One: return _Wiimote.Button.one;
                    case WiimoteButton.Two: return _Wiimote.Button.two;
                    case WiimoteButton.Plus: return _Wiimote.Button.plus;
                    case WiimoteButton.Minus: return _Wiimote.Button.minus;
                    case WiimoteButton.Left: return _Wiimote.Button.d_left;
                    case WiimoteButton.Right: return _Wiimote.Button.d_right;
                    case WiimoteButton.Up: return _Wiimote.Button.d_up;
                    case WiimoteButton.Down: return _Wiimote.Button.d_down;
                    case WiimoteButton.Home: return _Wiimote.Button.home;

                    default:
                        // TODO: Throw exception?
                        return false;
                }
            }
        }
    }
}