using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Biglab
{
    [Serializable]
    public class WebCamTextureUpdateEvent : UnityEvent<WebCamTexture> { }

    /// <summary>
    /// Component that gives you access to a webcam texture and a callback for when it updates.
    /// </summary>
    public class WebCamSource : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    {
        /// <summary>
        /// Callback when the capture device updates the texture.
        /// </summary>
        public WebCamTextureUpdateEvent OnTextureUpdate;

        /// <summary>
        /// Webcam device information.
        /// </summary>
        public WebCamDevice Device { get; private set; }

        /// <summary>
        /// Texture reference.
        /// </summary>
        public WebCamTexture Texture { get; private set; }

        private void OnEnable()
        {
            string device = null;

            // Prefer Insta360
            var insta360 = GetDevice( x => x.name.StartsWith( "Insta" ) );
            if( insta360.HasValue ) device = insta360.Value.name;

            // Set to default device
            ChangeDevice( device );
        }

        private void OnDisable()
        {
            // Disable streaming webcam to texture
            StopCapture();
        }

        /// <summary>
        /// Returns a list of webcam devices.
        /// </summary>
        public static WebCamDevice[] GetDeviceList( bool onlyFrontFacing = false )
        {
            // TODO: Cache?
            if( onlyFrontFacing ) return WebCamTexture.devices.Where( d => d.isFrontFacing ).ToArray();
            else return WebCamTexture.devices;
        }

        /// <summary>
        /// Gets information about a specific webcam.
        /// </summary>
        public static WebCamDevice? GetDevice( Func<WebCamDevice, bool> predicate )
        {
            var devices = GetDeviceList();
            if( devices.Any() ) return devices.First( predicate );
            else return null;
        }

        /// <summary>
        /// Begin streaming from the webcam.
        /// </summary>
        public void BeginCapture()
        {
            if( !Texture.isPlaying )
                Texture.Play();
        }

        /// <summary>
        /// Stop streaming from the webcam.
        /// </summary>
        public void StopCapture()
        {
            if( Texture.isPlaying )
                Texture.Stop();
        }

        /// <summary>
        /// Changes the device and recreates the texture.
        /// </summary>
        public void ChangeDevice( string device )
        {
            // TODO: Validate device is in list?

            // Dispose previous texture
            if( Texture != null )
            {
                Texture.Stop();
                DestroyImmediate( Texture );
            }

            // Create the webcam texture
            if( device == null ) Texture = new WebCamTexture();
            else Texture = new WebCamTexture( device );

            // Get the device information
            Device = GetDevice( d => Texture.deviceName == d.name ) ?? default( WebCamDevice );

            // Begin video capture
            BeginCapture();
        }

        void Update()
        {
            // Respond to the new frame.
            if( Texture.didUpdateThisFrame )
                OnTextureUpdate.Invoke( Texture );
        }

#if UNITY_EDITOR

        [CustomEditor( typeof( WebCamSource ), true )]
        public class WebCamSourceEditor : Editor
        {
            private const string DeviceTooltip =
                "Allows changing the capture device in the inspector, but won't affect builds as the device list will change per-platform.\n\n" +
                "You will have to use ChangeDevice() to choose an alternative camera programatically.";

            public override void OnInspectorGUI()
            {
                var source = target as WebCamSource;
                var isDirty = false;

                EditorGUI.BeginChangeCheck();

                // Display the script reference
                GUI.enabled = false;
                var prop = serializedObject.FindProperty( "m_Script" );
                EditorGUILayout.PropertyField( prop, true );
                GUI.enabled = true;

                // Popup list of devices
                EditorGUILayout.BeginHorizontal();
                var devices = WebCamTexture.devices.Select( x => new GUIContent( x.name ) ).ToArray();
                var idx = Array.FindIndex( devices, x => x.text == source.Device.name );
                idx = EditorGUILayout.Popup( new GUIContent( "Device", DeviceTooltip ), idx, devices );
                EditorGUILayout.EndHorizontal();

                if( EditorGUI.EndChangeCheck() )
                {
                    // 
                    source.ChangeDevice( devices[idx].text );
                    isDirty = true;
                }

                // Update callback
                EditorGUILayout.PropertyField( serializedObject.FindProperty( "OnTextureUpdate" ) );

                if( isDirty )
                    EditorUtility.SetDirty( target );

                // 
                serializedObject.ApplyModifiedProperties();
            }
        }

#endif

    }
}