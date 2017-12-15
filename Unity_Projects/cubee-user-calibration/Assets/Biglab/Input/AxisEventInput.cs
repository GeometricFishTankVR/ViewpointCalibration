using UnityEngine;
using UnityEngine.Events;

namespace Biglab.Input
{
    /// <summary>
    /// Utility class to trigger events based on the state of an axis.
    /// </summary>
    public class AxisEventInput : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        public CustomInput Input;

        [Tooltip( "Name of the axis monitored." )]
        public string AxisName = "Horizontal";

        [Tooltip( "Enable or disable repeated axis events." )]
        public bool AllowRepeats = true;

        [Tooltip( "The rate that holding the axis will trigger repeats." )]
        public float InputRate = 0.25F;

        [Tooltip( "Triggered when the axis is positive." )]
        public UnityEvent OnPositive;

        [Tooltip( "Triggered when the axis is negative." )]
        public UnityEvent OnNegative;

        // 

        private float AxisLast = Mathf.Epsilon;
        private bool AllowEvent = true;

        void Update()
        {
            var axis = Input.GetAxis( AxisName );

            // 
            if( Mathf.Abs( axis ) < AxisLast )
            {
                CancelInvoke( "AllowInputEvent" );
                AllowEvent = true;
            }
            // 
            else if( AllowEvent )
            {
                // Trigger correct event
                if( axis > 0 ) OnPositive.Invoke();
                else if( axis < 0 ) OnNegative.Invoke();

                // 
                AllowEvent = false;
                AxisLast = Mathf.Abs( axis );

                // Throttles how many events per second
                if( AllowRepeats )
                {
                    CancelInvoke( "AllowInputEvent" );
                    Invoke( "AllowInputEvent", InputRate );
                }
            }
        }

        void AllowInputEvent()
        {
            AxisLast = Mathf.Epsilon;
            AllowEvent = true;
        }
    }
}