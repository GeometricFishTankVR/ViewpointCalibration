using Biglab.Input;
using Malee;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Biglab
{
    [ExecuteInEditMode]
    public class CanvasMenu : MonoBehaviour
    {
        public CustomInput Input;

        [SerializeField, Reorderable( add = false, remove = false )]
        private CanvasList Canvases;

        [Serializable]
        private class CanvasList : ReorderableArray<Canvas> { }

        public IEnumerable<string> MenuNames { get; private set; }

        private int currentMenuIndex = 0;

        void Start()
        {
            MenuNames = Canvases.Select( canvas => canvas.name );
            SetCanvasEnableStates();
        }

        void OnEnable()
        {
            SetCanvasEnableStates();
        }

        void Update()
        {
            if( Application.isPlaying )
            {
                if( Input.GetButtonDown( "Next" ) ) GotoNext();
                if( Input.GetButtonDown( "Previous" ) ) GotoPrevious();
            }
        }

        void OnTransformChildrenChanged()
        {
            SetCanvasEnableStates();
        }

        private void SetCanvasEnableStates()
        {
            // Detect child canvases
            Canvases.Clear();
            foreach( var canvas in transform.GetComponentsInChildren<Canvas>( true ) )
                Canvases.Add( canvas );

            // Disable the canvases
            foreach( var canvas in Canvases )
                canvas.gameObject.SetActive( false );

            // Enable active canvas
            Goto( currentMenuIndex );
        }

        public void Goto( string name )
        {
            var targetIndex = Canvases.FindIndex( canvas => string.Equals( canvas.name, name, StringComparison.InvariantCultureIgnoreCase ) );
            if( targetIndex == -1 ) throw new InvalidOperationException( string.Format( "Unable to change to menu '{0}', not found.", name ) );
            else Goto( targetIndex ); // Switch to menu at index
        }

        public void Goto( int index )
        {
            // Validate index
            if( index < 0 || index >= Canvases.Count ) throw new InvalidOperationException( "Unable to change menu index, out of bounds." );
            else
            {
                var oldCanvas = Canvases[currentMenuIndex];
                var newCanvas = Canvases[index];

                oldCanvas.gameObject.SetActive( false );
                newCanvas.gameObject.SetActive( true );

                if( Application.isPlaying )
                {
                    EventSystem.current.SetSelectedGameObject( null );
                    var selectable = newCanvas.GetComponentInChildren<Selectable>();
                    if( selectable != null ) EventSystem.current.SetSelectedGameObject( selectable.gameObject );
                    else Debug.LogErrorFormat( "Unable to find a Selectable object in target canvas '{0}'.", newCanvas.name );
                }

                currentMenuIndex = index;
            }
        }

        public void GotoNext()
        {
            // Move to the next index
            var targetIndex = currentMenuIndex + 1;
            if( targetIndex >= Canvases.Count )
                targetIndex = 0;

            // Change the canvas
            Goto( targetIndex );
        }

        public void GotoPrevious()
        {
            // Move to the previous index
            var targetIndex = currentMenuIndex - 1;
            if( targetIndex < 0 )
                targetIndex = Canvases.Count - 1;

            // Change the canvas
            Goto( targetIndex );
        }
    }
}