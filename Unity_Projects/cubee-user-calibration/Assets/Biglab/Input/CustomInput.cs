using Malee;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Biglab.Input
{
    using UnityInput = UnityEngine.Input;

    /// <summary>
    /// A class that allows a sort of aliasing / extension to the built in input system in Unity.
    /// </summary>
    public class CustomInput : MonoBehaviour
    /*
     * TODO:
     * Should an exception be thrown in the case of AllowUnityInputPassthrough is false and the button isn't defined?
     */
    {
        /// <summary>
        /// Should this input component pass any missing buttons/axis to the Unity subsystem?
        /// </summary>
        [Tooltip( "Should this component pass any missing input to the Unity input manager or only accept defined buttons." )]
        public bool AllowUnityInputPassthrough = true;

        [Reorderable, SerializeField]
        private ButtonList m_Buttons;

        [Reorderable, SerializeField]
        private AxisList m_Axes;

        /// <summary>
        /// All defined button definitions.
        /// </summary>
        public IEnumerable<ButtonDefinition> Buttons { get { return m_Buttons; } }

        /// <summary>
        /// All defines axis definitions.
        /// </summary>
        public IEnumerable<AxisDefinition> Axes { get { return m_Axes; } }

        [Serializable]
        private class ButtonList : ReorderableArray<ButtonDefinition>
        { }

        [Serializable]
        private class AxisList : ReorderableArray<AxisDefinition>
        { }

        private Dictionary<string, State> ButtonState;
        private Dictionary<string, float> AxisState;

        private CustomInputSource[] CustomInputSources;

        void Start()
        {
            // 
            CustomInputSources = GetComponents<CustomInputSource>();

            // 
            ButtonState = new Dictionary<string, State>();
            foreach( var button in Buttons )
                ButtonState[button.Name] = State.Released;

            // 
            AxisState = new Dictionary<string, float>();
            foreach( var axis in Axes )
                AxisState[axis.Name] = 0F;
        }

        void Update()
        {
            // Poll each button
            foreach( var button in Buttons )
            {
                var previous = ButtonState[button.Name];
                ButtonState[button.Name] = button.PollInternal( previous, CustomInputSources );
            }

            // Poll each axis
            foreach( var axis in Axes )
            {
                var previous = AxisState[axis.Name];
                AxisState[axis.Name] = axis.PollInternal( previous, CustomInputSources );
            }
        }

        /// <summary>
        /// Determine if a 'button' was pressed.
        /// </summary>
        public bool GetButton( string name )
        {
            if( ButtonState.ContainsKey( name ) ) return ButtonState[name].HasFlag( State.Pressed );
            else if( AllowUnityInputPassthrough ) return UnityInput.GetButton( name );
            else return false;
        }

        /// <summary>
        /// Determine if a 'button' was pressed this frame.
        /// </summary>
        public bool GetButtonDown( string name )
        {
            if( ButtonState.ContainsKey( name ) ) return ButtonState[name].HasFlag( State.Pressed | State.Now );
            else if( AllowUnityInputPassthrough ) return UnityInput.GetButtonDown( name );
            else return false;
        }

        /// <summary>
        /// Determine if a 'button' was released this frame.
        /// </summary>
        public bool GetButtonUp( string name )
        {
            if( ButtonState.ContainsKey( name ) ) return ButtonState[name].HasFlag( State.Released | State.Now );
            else if( AllowUnityInputPassthrough ) return UnityInput.GetButtonUp( name );
            else return false;
        }

        /// <summary>
        /// Gets the state value of a particular axis.
        /// </summary>
        public float GetAxis( string name )
        {
            if( AxisState.ContainsKey( name ) ) return AxisState[name];
            else if( AllowUnityInputPassthrough ) return UnityInput.GetAxisRaw( name );
            else return 0F;
        }

        [Flags]
        internal enum State
        {
            Released = 1 << 0,
            Pressed = 1 << 1,
            Now = 1 << 2
        }

        [Serializable]
        public class ButtonDefinition
        {
            private const float ACTIVATION_THRESHOLD = 0.25F;

            [SerializeField, Tooltip( "Identifier to use when checking if this button is pressed." )]
            private string m_Name;

            [SerializeField, Tooltip( "Interval this button will repeat the ButtonPressed event." )]
            private float m_RepeatInterval = 0.0F;

            [SerializeField, Tooltip( "Name of a button that will trigger the local button state." )]
            private string m_Button;

            [SerializeField, Tooltip( "Name of an axis that will trigger the local button state." )]
            private string m_Axis;

            [SerializeField, Tooltip( "Should the axis specified trigger in the negative domain?" )]
            private bool m_IsNegativeAxis = false;

            // Determines if an axis or button is defined
            private bool HasAxis { get { return !string.IsNullOrEmpty( m_Axis ); } }
            private bool HasButton { get { return !string.IsNullOrEmpty( m_Button ); } }

            //  
            private float RepeatTime = 0F;

            // Only allow repeating if the interval is defined.
            private bool AllowRepeat { get { return m_RepeatInterval > 0; } }

            /// <summary>
            /// The name/alias of the button.
            /// </summary>
            public string Name { get { return m_Name; } }

            /// <summary>
            /// How often should this button repeat its 'down' event ( in seconds ).
            /// </summary>
            public float RepeatInterval { get { return m_RepeatInterval; } }

            /// <summary>
            /// Name of the button to check for events.
            /// </summary>
            public string Button { get { return m_Button; } }

            /// <summary>
            /// Name of an axis to check for events.
            /// </summary>
            public string Axis { get { return m_Axis; } }

            /// <summary>
            /// Called automatically, do no call this yourself.
            /// </summary>
            internal State PollInternal( State previous, IEnumerable<CustomInputSource> customInputSources = null )
            {
                // 
                if( customInputSources == null )
                    customInputSources = Enumerable.Empty<CustomInputSource>();

                // == Determine if the button is pressed

                bool isPressed = false;

                // Check if the axis representation was pressed
                if( HasAxis )
                {
                    // Check primary
                    if( !( isPressed = isAxisThreshold( UnityInput.GetAxis( m_Axis ) ) ) )
                    {
                        // Iterate each axis
                        foreach( var source in customInputSources )
                        {
                            if( !source.enabled ) continue;

                            isPressed = isAxisThreshold( source.GetAxis( m_Axis ) );
                            if( isPressed ) break;
                        }
                    }
                }

                // Check if a button was pressed 
                if( HasButton && isPressed == false )
                {
                    // Check primary
                    if( !( isPressed = UnityInput.GetButton( m_Button ) ) )
                    {
                        // Iterate each button
                        foreach( var source in customInputSources )
                        {
                            if( !source.enabled ) continue;

                            isPressed = source.GetButton( m_Button );
                            if( isPressed ) break;
                        }
                    }
                }

                // Release conditions
                if( isPressed == false )
                {
                    RepeatTime = 0F;
                    isPressed = false;
                }

                // == Determine "if the button is pressed, should actually trigger it?"

                // Button is pressed, and we've waited long enough
                if( isPressed )
                {
                    var state = State.Pressed;

                    // Decrement repeat time
                    if( RepeatTime > 0 )
                        RepeatTime -= Time.deltaTime;

                    // Wasn't previously pressed, so must be new ( or was repeated ). 
                    if( ( AllowRepeat && RepeatTime <= 0F ) || previous.HasFlag( State.Released ) )
                    {
                        // Add interval to repeat time
                        if( AllowRepeat )
                            RepeatTime += m_RepeatInterval;

                        // Pressed.Invoke();
                        state |= State.Now;
                    }

                    return state;
                }
                else
                {
                    var state = State.Released;

                    // Wasn't previously released, must be new.
                    if( !previous.HasFlag( State.Released ) )
                    {
                        // Released.Invoke();
                        state |= State.Now;
                    }

                    return state;
                }
            }

            private bool isAxisThreshold( float raw )
            {
                if( m_IsNegativeAxis ) return ( raw < -ACTIVATION_THRESHOLD );
                else return ( raw > +ACTIVATION_THRESHOLD );
            }
        }

        [Serializable]
        public class AxisDefinition
        {
            [Tooltip( "Identifier to use when checking if this button is pressed." )]
            public string m_Name;

            [Tooltip( "Name of an axis that will trigger the local button state." )]
            public string m_Axis;

            public string Name { get { return m_Name; } }

            public string Axis { get { return m_Axis; } }

            /// <summary>
            /// Called automatically, do no call this yourself.
            /// </summary>
            public float PollInternal( float previous, IEnumerable<CustomInputSource> customInputSources = null )
            {
                // 
                if( customInputSources == null )
                    customInputSources = Enumerable.Empty<CustomInputSource>();

                var raw = UnityInput.GetAxis( m_Axis );

                // Check axis
                if( Mathf.Abs( raw ) > 0 ) return UnityInput.GetAxis( m_Axis );
                else
                {
                    // Iterate each axis
                    foreach( var source in customInputSources )
                    {
                        if( !source.enabled ) continue;

                        raw = source.GetAxis( m_Axis );
                        if( Mathf.Abs( raw ) > 0 )
                            return raw;
                    }
                }

                // 
                return 0;
            }
        }
    }
}