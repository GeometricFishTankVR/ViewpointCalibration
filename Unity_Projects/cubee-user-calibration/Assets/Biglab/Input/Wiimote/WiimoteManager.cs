using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WM = WiimoteApi.WiimoteManager;

namespace Biglab.Input
{
    // [ExecuteInEditMode]
    public class WiimoteManager : MonoBehaviour
    // Author: Christopher Chamberlain - 2017
    /*
     * Notes:
     * 
     * Wiimote disconnect event isn't current implemented properly.
     */
    {
        private List<Wiimote> Wiimotes = new List<Wiimote>();

        /// <summary>
        /// The number of wiimotes connected.
        /// </summary>
        public int NumberConnected { get { return WM.HasWiimote() ? Wiimotes.Count : 0; } }

        /// <summary>
        /// Event triggered when a wiimote is connected.
        /// </summary>
        public Action<int, Wiimote> WiimoteConnected;

        /// <summary>
        /// Event triggered when a wiimote is disconnected.
        /// </summary>
        public Action<int, Wiimote> WiimoteDisconnected;

        void Awake()
        {
            var handlers = FindObjectsOfType<WiimoteManager>();
            if( handlers.Length > 1 ) Debug.LogError( "More than one WiimoteHandler found in scene." );

            //
            WM.FindWiimotes();
        }

        void Start()
        {
            for( int i = 0; i < 4; i++ )
                GetWiimote( i );
        }

        void OnDestroy()
        {
            if( Wiimotes != null )
            {
                // 
                foreach( var wiimote in Wiimotes )
                {
                    if( wiimote._Wiimote != null )
                    {
                        if( WiimoteDisconnected != null )
                            WiimoteDisconnected.Invoke( wiimote.Index, wiimote );

                        WM.Cleanup( wiimote._Wiimote );
                    }
                }

                // 
                Wiimotes = null;
            }
        }

        void Update()
        {
            //
            if( WM.HasWiimote() )
            {
                // Are there more controllers than before?
                if( WM.Wiimotes.Count > Wiimotes.Count )
                {
                    // Controller connected
                    for( int i = 0; i < WM.Wiimotes.Count; i++ )
                    {
                        if( !IsConnected( i ) )
                        {
                            var wiimote = GetWiimote( i );
                            wiimote.gameObject.SetActive( true );
                            Wiimotes.Add( wiimote );
                        }
                    }
                }

                // Controller disconnected?
            }
        }

        private Wiimote GetWiimote( int index )
        {
            var wiimote_name = string.Format( "Wiimote {0}", index );
            var wiimote_transform = transform.Find( wiimote_name );
            if( wiimote_transform == null )
            {
                var obj = new GameObject( wiimote_name );
                obj.SetActive( false );

                var wiimote = obj.AddComponent<Wiimote>();
                wiimote.Index = index;

                // Child Wiimote to manager
                obj.transform.SetParent( transform );

                // 
                if( WiimoteConnected != null )
                    WiimoteConnected.Invoke( index, wiimote );

                return wiimote;
            }
            else
            {
                // 
                return wiimote_transform.GetComponent<Wiimote>();
            }
        }

        /// <summary>
        /// Determines if a given wiimote index is connected.
        /// </summary>
        /// <param name="index">The index of the desired wiimote.</param>
        /// <returns>True, if the wiimote is connected and detected.</returns>
        public bool IsConnected( int index )
        {
            if( index < 0 ) return false;
            if( index >= Wiimotes.Count ) return false;
            else return true;
        }

        /// <summary>
        /// Gets the nth wiimote available.
        /// </summary>
        /// <param name="index">The index of the desired wiimote.</param>
        /// <returns>The corresponding wiimote object.</returns>
        public Wiimote Get( int index )
        {
            if( IsConnected( index ) == false )
                throw new ArgumentOutOfRangeException( string.Format( "Unable acquire wiimote {0}. Not connected.", index ) );

            return Wiimotes[index];
        }
    }
}