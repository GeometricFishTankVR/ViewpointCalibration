using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Biglab.UI
{
    using Input;
    using System.Linq;
    using UnityInput = UnityEngine.Input;

    public class VirtualPointerInputModule : PointerInputModule
    // Based on:
    //     https://github.com/wacki/Unity-VRInputModule/blob/master/Assets/VRInputModule/Scripts/LaserPointerInputModule.cs
    // and
    //     https://bitbucket.org/Unity-Technologies/ui/src/cdf7fd2113e1/UnityEngine.UI/EventSystem/InputModules/?at=4.6
    {
        public CustomInput Input;

        private float m_NextAction;

        private float m_PrevActionTime;
        Vector2 m_LastMoveVector;
        int m_ConsecutiveMoveCount = 0;

        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;

        private GameObject m_CurrentFocusedGameObject;

        protected VirtualPointerInputModule()
        { }

        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";

        [SerializeField]
        private string m_VerticalAxis = "Vertical";

        [SerializeField]
        private string m_SubmitButton = "Submit";

        [SerializeField]
        private string m_CancelButton = "Cancel";

        [SerializeField]
        private float m_InputActionsPerSecond = 10;

        [SerializeField]
        private float m_RepeatDelay = 0.5f;

        public float inputActionsPerSecond
        {
            get { return m_InputActionsPerSecond; }
            set { m_InputActionsPerSecond = value; }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public string horizontalAxis
        {
            get { return m_HorizontalAxis; }
            set { m_HorizontalAxis = value; }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public string verticalAxis
        {
            get { return m_VerticalAxis; }
            set { m_VerticalAxis = value; }
        }

        public string submitButton
        {
            get { return m_SubmitButton; }
            set { m_SubmitButton = value; }
        }

        public string cancelButton
        {
            get { return m_CancelButton; }
            set { m_CancelButton = value; }
        }

        PointerReference[] Pointers;
        private Camera UIEventCamera;

        protected override void Start()
        {
            base.Start();

            // 
            UIEventCamera = CreatePointerCamera();

            // Find all virtual pointers and create data for it.
            Pointers = FindObjectsOfType<VirtualPointer>().Select( p => new PointerReference( p ) ).ToArray();
            Debug.LogFormat( "Found {0} Virtual Pointers", Pointers.Length );

            // Find canvases in the scene and assign the virtual pointer camera to it.
            // TODO: Somehow watch for any newly spawned canvas?
            foreach( var canvas in GetAllWorldspaceCanvases() )
                canvas.worldCamera = UIEventCamera;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach( var canvas in GetAllWorldspaceCanvases() )
                canvas.worldCamera = null; // TODO: Previously set camera?
        }

        private Canvas[] GetAllWorldspaceCanvases()
        {
            var cans = Resources.FindObjectsOfTypeAll<Canvas>();
            return cans.Where( can => can.renderMode == RenderMode.WorldSpace ).ToArray();
        }

        private static Camera CreatePointerCamera()
        {
            var obj = new GameObject( "Virtual Pointer ( Event Camera )" );

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

        public override void UpdateModule()
        {
            m_LastMousePosition = m_MousePosition;
            m_MousePosition = UnityInput.mousePosition;
        }

        public override bool IsModuleSupported()
        {
            // 
            var vps = FindObjectsOfType<VirtualPointer>();
            return input.mousePresent || vps.Length > 0;
        }

        public override bool ShouldActivateModule()
        {
            if( !base.ShouldActivateModule() )
                return false;

            var shouldActivate = Input.GetButtonDown( m_SubmitButton );
            shouldActivate |= Input.GetButtonDown( m_CancelButton );
            shouldActivate |= !Mathf.Approximately( Input.GetAxis( m_HorizontalAxis ), 0.0f );
            shouldActivate |= !Mathf.Approximately( Input.GetAxis( m_VerticalAxis ), 0.0f );
            // shouldActivate |= ( m_MousePosition - m_LastMousePosition ).sqrMagnitude > 0.0f;
            // shouldActivate |= Input.GetMouseButtonDown( 0 );
            return shouldActivate;
        }

        public override void ActivateModule()
        {
            base.ActivateModule();
            m_MousePosition = UnityInput.mousePosition;
            m_LastMousePosition = UnityInput.mousePosition;

            var toSelect = eventSystem.currentSelectedGameObject;
            if( toSelect == null )
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject( toSelect, GetBaseEventData() );
        }

        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }

        // select a game object
        private void Select( GameObject go )
        {
            ClearSelection();

            if( ExecuteEvents.GetEventHandler<ISelectHandler>( go ) )
                eventSystem.SetSelectedGameObject( go );
        }

        public override void Process()
        {
            //bool usedEvent = SendUpdateEventToSelectedObject();

            //if( eventSystem.sendNavigationEvents )
            //{
            //    if( !usedEvent )
            //        usedEvent |= SendMoveEventToSelectedObject();

            //    if( !usedEvent )
            //        SendSubmitEventToSelectedObject();
            //}

            //ProcessMouseEvent();

            //if( !usedEvent )
            //{
            // Check each virtual pointer for events
            foreach( var data in Pointers )
            {
                // Align pointer camera to virtual pointer
                data.UpdateCamera( UIEventCamera );

                // Process pointer events
                ProcessPointer( data );
            }
            //}
        }

        #region Virtual Pointers

        private void ProcessPointer( PointerReference data )
        {
            // Get/Create pointer event
            if( data.PointerEvent == null ) data.PointerEvent = new PointerEventData( eventSystem );
            else data.PointerEvent.Reset();

            // Reset pointer data
            data.PointerEvent.position = new Vector2( UIEventCamera.pixelWidth * 0.5f, UIEventCamera.pixelHeight * 0.5f );
            data.PointerEvent.scrollDelta = Vector2.zero;

            // Compute screen space movement of cursor 
            var endPoint = data.Pointer.transform.position + ( data.Pointer.transform.forward * data.Pointer.LaserLength );
            var v1 = UIEventCamera.WorldToViewportPoint( endPoint ).AsVector2();
            var v2 = UIEventCamera.WorldToViewportPoint( data.LastWorldPosition ).AsVector2();
            data.LastWorldPosition = endPoint;

            //Debug.LogFormat( "{0} {1}", v1 - v2, ( v1 - v2 ).magnitude );

            // TODO: Low pass / smoothing on the threshold/magnitude?
            data.PointerEvent.delta = v1 - v2;
            if( data.PointerEvent.delta.magnitude < 0.03F ) // MOVEMENT/JITTER THRESHOLD
                data.PointerEvent.delta = Vector2.zero;

            // Raycast to detect current object
            data.PerformRaycast( eventSystem, m_RaycastResultCache );

            // Handle enter and exit events on the GUI controlls that are hit
            HandlePointerExitAndEnter( data.PointerEvent, data.PointerEvent.pointerCurrentRaycast.gameObject );

            // Button Down ( Press & Release )
            ProcessPointerDown( data );
            ProcessPointerRelease( data );
            ProcessDrag( data.PointerEvent );

            // Update selected element for keyboard focus
            if( eventSystem.currentSelectedGameObject != null )
            {
                data.PointerEvent.selectedObject = eventSystem.currentSelectedGameObject;
                ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler );
            }
        }

        private static bool ShouldStartDrag( Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold )
        {
            if( !useDragThreshold )
                return true;

            return ( pressPos - currentPos ).sqrMagnitude >= threshold * threshold;
        }

        private void ProcessPointerDown( PointerReference data )
        {
            var currentOverGo = data.PointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if( data.Pointer.GetButtonDown() )
            {
                var pointerEvent = data.PointerEvent;

                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged( currentOverGo, pointerEvent );

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy( currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler );

                // didnt find a press handler... search for a click handler
                if( newPressed == null )
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>( currentOverGo );

                //Debug.Log( "Pressed: " + newPressed );

                float time = Time.unscaledTime;

                pointerEvent.clickCount = 1;
                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>( currentOverGo );

                if( pointerEvent.pointerDrag != null )
                    ExecuteEvents.Execute( pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag );
            }
        }

        private void ProcessPointerRelease( PointerReference data )
        {
            var currentOverGo = data.PointerEvent.pointerCurrentRaycast.gameObject;

            if( data.Pointer.GetButtonUp() )
            {
                var pointerEvent = data.PointerEvent;

                //Debug.Log( "Executing pressup on: " + pointerEvent.pointerPress );
                ExecuteEvents.Execute( pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler );

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>( currentOverGo );

                // PointerClick and Drop events
                // TODO: 0.1 is the time in seconds to 'just accept' a click 
                if( ( Time.unscaledTime - pointerEvent.clickTime ) < 0.1F || ( pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick ) )
                {
                    ExecuteEvents.Execute( pointerUpHandler, pointerEvent, ExecuteEvents.pointerClickHandler );
                }
                else if( pointerEvent.pointerDrag != null && pointerEvent.dragging )
                {
                    ExecuteEvents.ExecuteHierarchy( currentOverGo, pointerEvent, ExecuteEvents.dropHandler );
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if( pointerEvent.pointerDrag != null && pointerEvent.dragging )
                    ExecuteEvents.Execute( pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler );

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if( currentOverGo != pointerEvent.pointerEnter )
                {
                    HandlePointerExitAndEnter( pointerEvent, null );
                    HandlePointerExitAndEnter( pointerEvent, currentOverGo );
                }

                //var pointerEvent = data.PointerEvent;

                //// Debug.Log("Executing pressup on: " + pointer.pointerPress);
                //ExecuteEvents.Execute( pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler );

                //// Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                //// see if we mouse up on the same element that we clicked on...
                //var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>( currentOverGo );

                //// PointerClick and Drop events
                //if( pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick )
                //{
                //    ExecuteEvents.Execute( pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler );
                //}
                //else if( pointerEvent.pointerDrag != null )
                //{
                //    ExecuteEvents.ExecuteHierarchy( currentOverGo, pointerEvent, ExecuteEvents.dropHandler );
                //}

                //pointerEvent.eligibleForClick = false;
                //pointerEvent.pointerPress = null;
                //pointerEvent.rawPointerPress = null;

                //if( pointerEvent.pointerDrag != null && pointerEvent.dragging )
                //    ExecuteEvents.Execute( pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler );

                //pointerEvent.dragging = false;
                //pointerEvent.pointerDrag = null;

                //// redo pointer enter / exit to refresh state
                //// so that if we moused over somethign that ignored it before
                //// due to having pressed on something else
                //// it now gets it.
                //if( currentOverGo != pointerEvent.pointerEnter )
                //{
                //    HandlePointerExitAndEnter( pointerEvent, null );
                //    HandlePointerExitAndEnter( pointerEvent, currentOverGo );
                //}

                //    // 
                //    if( data.CurrentPressed != null )
                //    {
                //        data.PointerEvent.selectedObject = data.CurrentPressed;
                //        ExecuteEvents.Execute( data.CurrentPressed, data.PointerEvent, ExecuteEvents.pointerUpHandler );

                //        data.PointerEvent.eligibleForClick = false;
                //        data.PointerEvent.rawPointerPress = null;
                //        data.PointerEvent.pointerPress = null;
                //        data.CurrentPressed = null;
                //    }

                //    // Release
                //    if( data.PointerEvent.pointerDrag != null )
                //    {
                //        data.PointerEvent.selectedObject = data.PointerEvent.pointerDrag;
                //        ExecuteEvents.Execute( data.PointerEvent.pointerDrag, data.PointerEvent, ExecuteEvents.endDragHandler );

                //        if( data.CurrentPoint != null )
                //            ExecuteEvents.ExecuteHierarchy( data.CurrentPoint, data.PointerEvent, ExecuteEvents.dropHandler );

                //        data.PointerEvent.pointerDrag = null;
                //        data.PointerEvent.dragging = false;
                //    }
            }
        }

        #endregion

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        private void ProcessMouseEvent()
        {
            ProcessMouseEvent( 0 );
        }

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        protected void ProcessMouseEvent( int id )
        {
            var mouseData = GetMousePointerEventData( id );
            var leftButtonData = mouseData.GetButtonState( PointerEventData.InputButton.Left ).eventData;

            m_CurrentFocusedGameObject = leftButtonData.buttonData.pointerCurrentRaycast.gameObject;

            // Process the first mouse button fully
            ProcessMousePress( leftButtonData );
            ProcessMove( leftButtonData.buttonData );
            ProcessDrag( leftButtonData.buttonData );

            // Now process right / middle clicks
            ProcessMousePress( mouseData.GetButtonState( PointerEventData.InputButton.Right ).eventData );
            ProcessDrag( mouseData.GetButtonState( PointerEventData.InputButton.Right ).eventData.buttonData );
            ProcessMousePress( mouseData.GetButtonState( PointerEventData.InputButton.Middle ).eventData );
            ProcessDrag( mouseData.GetButtonState( PointerEventData.InputButton.Middle ).eventData.buttonData );

            if( !Mathf.Approximately( leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f ) )
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>( leftButtonData.buttonData.pointerCurrentRaycast.gameObject );
                ExecuteEvents.ExecuteHierarchy( scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler );
            }
        }

        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        private void ProcessMousePress( MouseButtonEventData data )
        {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if( data.PressedThisFrame() )
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged( currentOverGo, pointerEvent );

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy( currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler );

                // didnt find a press handler... search for a click handler
                if( newPressed == null )
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>( currentOverGo );

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if( newPressed == pointerEvent.lastPress )
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if( diffTime < 0.3f )
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>( currentOverGo );

                if( pointerEvent.pointerDrag != null )
                    ExecuteEvents.Execute( pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag );
            }

            // PointerUp notification
            if( data.ReleasedThisFrame() )
            {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute( pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler );

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>( currentOverGo );

                // PointerClick and Drop events
                if( pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick )
                {
                    ExecuteEvents.Execute( pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler );
                }
                else if( pointerEvent.pointerDrag != null && pointerEvent.dragging )
                {
                    ExecuteEvents.ExecuteHierarchy( currentOverGo, pointerEvent, ExecuteEvents.dropHandler );
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if( pointerEvent.pointerDrag != null && pointerEvent.dragging )
                    ExecuteEvents.Execute( pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler );

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if( currentOverGo != pointerEvent.pointerEnter )
                {
                    HandlePointerExitAndEnter( pointerEvent, null );
                    HandlePointerExitAndEnter( pointerEvent, currentOverGo );
                }
            }
        }

        private Vector2 GetRawMoveVector()
        {
            Vector2 move = Vector2.zero;
            move.x = Input.GetAxis( m_HorizontalAxis );
            move.y = Input.GetAxis( m_VerticalAxis );

            if( Input.GetButtonDown( m_HorizontalAxis ) )
            {
                if( move.x < 0 )
                    move.x = -1f;
                if( move.x > 0 )
                    move.x = 1f;
            }

            if( Input.GetButtonDown( m_VerticalAxis ) )
            {
                if( move.y < 0 )
                    move.y = -1f;
                if( move.y > 0 )
                    move.y = 1f;
            }

            return move;
        }

        /// <summary>
        /// Process submit keys.
        /// </summary>
        private bool SendSubmitEventToSelectedObject()
        {
            if( eventSystem.currentSelectedGameObject == null )
                return false;

            var data = GetBaseEventData();

            if( Input.GetButtonUp( m_SubmitButton ) )
                ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler );

            if( Input.GetButtonDown( m_CancelButton ) )
                ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler );

            return data.used;
        }

        /// <summary>
        /// Process keyboard events.
        /// </summary>
        private bool SendMoveEventToSelectedObject()
        {
            float time = Time.unscaledTime;

            Vector2 movement = GetRawMoveVector();
            if( Mathf.Approximately( movement.x, 0f ) && Mathf.Approximately( movement.y, 0f ) )
            {
                m_ConsecutiveMoveCount = 0;
                return false;
            }

            // If user pressed key again, always allow event
            bool allow = input.GetButtonDown( m_HorizontalAxis ) || input.GetButtonDown( m_VerticalAxis );
            bool similarDir = ( Vector2.Dot( movement, m_LastMoveVector ) > 0 );
            if( !allow )
            {
                // Otherwise, user held down key or axis.
                // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
                if( similarDir && m_ConsecutiveMoveCount == 1 )
                    allow = ( time > m_PrevActionTime + m_RepeatDelay );
                // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
                else
                    allow = ( time > m_PrevActionTime + 1f / m_InputActionsPerSecond );
            }
            if( !allow )
                return false;

            // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
            var axisEventData = GetAxisEventData( movement.x, movement.y, 0.6f );

            if( axisEventData.moveDir != MoveDirection.None )
            {
                ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler );
                if( !similarDir )
                    m_ConsecutiveMoveCount = 0;
                m_ConsecutiveMoveCount++;
                m_PrevActionTime = time;
                m_LastMoveVector = movement;
            }
            else
            {
                m_ConsecutiveMoveCount = 0;
            }

            return axisEventData.used;
        }

        private static bool UseMouse( bool pressed, bool released, PointerEventData pointerData )
        {
            if( pressed || released || pointerData.IsPointerMoving() || pointerData.IsScrolling() )
                return true;

            return false;
        }

        private bool SendUpdateEventToSelectedObject()
        {
            if( eventSystem.currentSelectedGameObject == null )
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler );
            return data.used;
        }

        private class PointerReference
        {
            public PointerEventData PointerEvent;
            public VirtualPointer Pointer;

            public GameObject CurrentPoint;

            //public GameObject CurrentPressed;
            //public GameObject CurrentDragging; 

            public Vector3 LastWorldPosition;

            public PointerReference( VirtualPointer pointer )
            {
                Pointer = pointer;
            }

            public void UpdateCamera( Camera camera )
            {
                camera.transform.position = Pointer.transform.position;
                camera.transform.rotation = Pointer.transform.rotation;
            }

            public void PerformRaycast( EventSystem eventSystem, List<RaycastResult> m_RaycastResultCache )
            {
                // Raycast scene
                eventSystem.RaycastAll( PointerEvent, m_RaycastResultCache );
                PointerEvent.pointerCurrentRaycast = FindFirstRaycast( m_RaycastResultCache );
                m_RaycastResultCache.Clear();

                // Set laser to ray length ( to visualize the contact point )
                if( PointerEvent.pointerCurrentRaycast.distance > 0.0f )
                    Pointer.LaserLength = PointerEvent.pointerCurrentRaycast.distance + 0.01f; // distance + nearPlane

                // If no object was touched, hide laser.
                if( PointerEvent.pointerCurrentRaycast.gameObject == null )
                    Pointer.LaserLength = 0F;

                // 
                CurrentPoint = PointerEvent.pointerCurrentRaycast.gameObject;
            }
        }
    }
}
