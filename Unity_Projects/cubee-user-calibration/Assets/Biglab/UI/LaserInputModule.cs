using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Biglab.UI
{
    using System.Linq;
    using Input = UnityEngine.Input;

    public class LaserInputModule : BaseInputModule
    // Based on:
    //     https://github.com/wacki/Unity-VRInputModule/blob/master/Assets/VRInputModule/Scripts/LaserPointerInputModule.cs
    {
        public static LaserInputModule instance { get { return _instance; } }
        private static LaserInputModule _instance = null;

        // storage class for controller specific data
        private class PointerData
        {
            public VirtualPointer VirtualPointer;

            public LaserPointerEventData pointerEvent;

            public GameObject currentPoint;
            public GameObject currentPressed;
            public GameObject currentDragging;

            public PointerData( VirtualPointer vp )
            {
                VirtualPointer = vp;
            }
        };

        private PointerData[] Pointers;

        [Tooltip( "Button to act as the interact/click/submit." )]
        public string SubmitButton = "Submit";

        private Camera UIEventCamera;

        protected override void Awake()
        {
            base.Awake();

            if( _instance != null )
            {
                Debug.LogWarning( "Trying to instantiate multiple LaserPointerInputModule." );
                DestroyImmediate( this.gameObject );
            }

            _instance = this;
        }

        protected override void Start()
        {
            base.Start();

            // 
            UIEventCamera = CreatePointerCamera();
            UIEventCamera.gameObject.AddComponent<PhysicsRaycaster>();

            // Find all virtual pointers and create data for it.
            Pointers = FindObjectsOfType<VirtualPointer>().Select( vp => new PointerData( vp ) ).ToArray();

            // Find canvases in the scene and assign the virtual pointer to it
            // TODO: Watch for any newly spawned canvas?
            foreach( var canvas in Resources.FindObjectsOfTypeAll<Canvas>() )
                canvas.worldCamera = UIEventCamera;
        }

        private static Camera CreatePointerCamera()
        {
            var obj = new GameObject( "Laser Pointer UI Event Camera" );

            var camera = obj.AddComponent<Camera>();
            camera.farClipPlane = 100F;
            camera.nearClipPlane = 0.01F;
            camera.orthographicSize = 0.05F;
            // camera.orthographic = true;
            camera.enabled = false;

            camera.clearFlags = CameraClearFlags.Nothing;
            camera.fieldOfView = 5;

            return camera;
        }

        // clear the current selection
        public void ClearSelection()
        {
            if( base.eventSystem.currentSelectedGameObject )
                base.eventSystem.SetSelectedGameObject( null );
        }

        // select a game object
        private void Select( GameObject go )
        {
            ClearSelection();

            if( ExecuteEvents.GetEventHandler<ISelectHandler>( go ) )
                base.eventSystem.SetSelectedGameObject( go );
        }

        public override bool ShouldActivateModule()
        {
            return false;
        }

        public override void Process()
        {
            // For each 
            foreach( var ptr in Pointers )
            {
                // Align pointer camera to virtual pointer
                UIEventCamera.transform.position = ptr.VirtualPointer.transform.position;
                UIEventCamera.transform.rotation = ptr.VirtualPointer.transform.rotation;

                // Get/Create pointer event
                if( ptr.pointerEvent == null ) ptr.pointerEvent = new LaserPointerEventData( eventSystem );
                else ptr.pointerEvent.Reset();

                // Reset pointer data
                ptr.pointerEvent.delta = Vector2.zero;
                ptr.pointerEvent.position = new Vector2( UIEventCamera.pixelWidth * 0.5f, UIEventCamera.pixelHeight * 0.5f );
                //ctrl.pointerEvent.scrollDelta = Vector2.zero;

                // trigger a raycast
                eventSystem.RaycastAll( ptr.pointerEvent, m_RaycastResultCache );
                ptr.pointerEvent.pointerCurrentRaycast = FindFirstRaycast( m_RaycastResultCache );
                m_RaycastResultCache.Clear();

                // << Change rendering/effect distance of laser >>
                if( ptr.pointerEvent.pointerCurrentRaycast.distance > 0.0f )
                    ptr.VirtualPointer.LaserLength = ptr.pointerEvent.pointerCurrentRaycast.distance + 0.01f;

                // Stop if no UI element was hit ( move to next virtual pointer )
                if( ptr.pointerEvent.pointerCurrentRaycast.gameObject == null )
                {
                    ptr.VirtualPointer.LaserLength = 0F;
                    continue;
                }

                // Send control enter and exit events to our controller
                var hitControl = ptr.pointerEvent.pointerCurrentRaycast.gameObject;
                if( ptr.currentPoint != hitControl )
                {
                    //if( data.currentPoint != null )
                    //    controller.OnExitControl( data.currentPoint );

                    //if( hitControl != null )
                    //    controller.OnEnterControl( hitControl );
                }

                ptr.currentPoint = hitControl;

                // Handle enter and exit events on the GUI controlls that are hit
                base.HandlePointerExitAndEnter( ptr.pointerEvent, ptr.currentPoint );

                if( ptr.VirtualPointer.GetButtonDown() )
                {
                    ClearSelection();

                    ptr.pointerEvent.pressPosition = ptr.pointerEvent.position;
                    ptr.pointerEvent.pointerPressRaycast = ptr.pointerEvent.pointerCurrentRaycast;
                    ptr.pointerEvent.pointerPress = null;

                    // update current pressed if the curser is over an element
                    if( ptr.currentPoint != null )
                    {
                        ptr.currentPressed = ptr.currentPoint;
                        ptr.pointerEvent.current = ptr.currentPressed;
                        GameObject newPressed = ExecuteEvents.ExecuteHierarchy( ptr.currentPressed, ptr.pointerEvent, ExecuteEvents.pointerDownHandler );
                        //ExecuteEvents.Execute( controller.gameObject, data.pointerEvent, ExecuteEvents.pointerDownHandler );

                        if( newPressed == null )
                        {
                            // some UI elements might only have click handler and not pointer down handler
                            newPressed = ExecuteEvents.ExecuteHierarchy( ptr.currentPressed, ptr.pointerEvent, ExecuteEvents.pointerClickHandler );
                            //ExecuteEvents.Execute( controller.gameObject, data.pointerEvent, ExecuteEvents.pointerClickHandler );
                            if( newPressed != null )
                            {
                                ptr.currentPressed = newPressed;
                            }
                        }
                        else
                        {
                            ptr.currentPressed = newPressed;
                            // we want to do click on button down at same time, unlike regular mouse processing
                            // which does click when mouse goes up over same object it went down on
                            // reason to do this is head tracking might be jittery and this makes it easier to click buttons
                            ExecuteEvents.Execute( newPressed, ptr.pointerEvent, ExecuteEvents.pointerClickHandler );
                            //ExecuteEvents.Execute( controller.gameObject, ctrl.pointerEvent, ExecuteEvents.pointerClickHandler );

                        }

                        if( newPressed != null )
                        {
                            ptr.pointerEvent.pointerPress = newPressed;
                            ptr.currentPressed = newPressed;
                            Select( ptr.currentPressed );
                        }

                        ExecuteEvents.Execute( ptr.currentPressed, ptr.pointerEvent, ExecuteEvents.beginDragHandler );
                        //ExecuteEvents.Execute( controller.gameObject, ctrl.pointerEvent, ExecuteEvents.beginDragHandler );

                        ptr.pointerEvent.pointerDrag = ptr.currentPressed;
                        ptr.currentDragging = ptr.currentPressed;
                    }
                } // button down end

                // Button Up ( Release )
                if( ptr.VirtualPointer.GetButtonUp() )
                {
                    if( ptr.currentDragging != null )
                    {
                        ptr.pointerEvent.current = ptr.currentDragging;
                        ExecuteEvents.Execute( ptr.currentDragging, ptr.pointerEvent, ExecuteEvents.endDragHandler );
                        //ExecuteEvents.Execute( controller.gameObject, ctrl.pointerEvent, ExecuteEvents.endDragHandler );
                        if( ptr.currentPoint != null )
                        {
                            ExecuteEvents.ExecuteHierarchy( ptr.currentPoint, ptr.pointerEvent, ExecuteEvents.dropHandler );
                        }
                        ptr.pointerEvent.pointerDrag = null;
                        ptr.currentDragging = null;
                    }

                    if( ptr.currentPressed )
                    {
                        ptr.pointerEvent.current = ptr.currentPressed;
                        ExecuteEvents.Execute( ptr.currentPressed, ptr.pointerEvent, ExecuteEvents.pointerUpHandler );
                        //ExecuteEvents.Execute( controller.gameObject, ctrl.pointerEvent, ExecuteEvents.pointerUpHandler );
                        ptr.pointerEvent.rawPointerPress = null;
                        ptr.pointerEvent.pointerPress = null;
                        ptr.currentPressed = null;
                    }
                }

                // Drag handling
                if( ptr.currentDragging != null )
                {
                    ptr.pointerEvent.current = ptr.currentPressed;
                    ExecuteEvents.Execute( ptr.currentDragging, ptr.pointerEvent, ExecuteEvents.dragHandler );
                    //ExecuteEvents.Execute( controller.gameObject, ctrl.pointerEvent, ExecuteEvents.dragHandler );
                }

                // update selected element for keyboard focus
                if( base.eventSystem.currentSelectedGameObject != null )
                {
                    ptr.pointerEvent.current = eventSystem.currentSelectedGameObject;
                    ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler );
                    //ExecuteEvents.Execute(controller.gameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
                }
            }
        }
    }
}