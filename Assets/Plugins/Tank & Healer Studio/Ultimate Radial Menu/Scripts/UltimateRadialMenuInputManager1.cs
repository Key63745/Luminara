///* UltimateRadialMenuInputManager.cs */
///* Written by Kaz Crowe */
//using UnityEngine;
//using UnityEngine.EventSystems;
//using System.Collections.Generic;

//public class UltimateRadialMenuInputManager : MonoBehaviour
//{
//	public static UltimateRadialMenuInputManager Instance
//	{
//		get;
//		private set;
//	}
//	protected Camera mainCamera;
//	public class UltimateRadialMenuInfomation
//	{
//		public UltimateRadialMenu radialMenu;
//		public bool lastRadialMenuState = false;
//	}
//	public List<UltimateRadialMenuInfomation> UltimateRadialMenuInformations
//	{
//		get;
//		private set;
//	}
//	LayerMask worldSpaceMask;
//	bool isUniqueInputManager = false;

//	// INTERACT SETTINGS //
//	public enum InvokeAction
//	{
//		OnButtonDown,
//		OnButtonClick
//	}
//	[Header( "Interact Settings" )]
//	[Tooltip( "The action required to invoke the radial button." )]
//	public InvokeAction invokeAction = InvokeAction.OnButtonDown;
//	[Tooltip( "Determines whether or not the Ultimate Radial Menu will receive input when the Ultimate Radial Menu is released and disabled." )]
//	public bool onMenuRelease = false;
//	[Tooltip( "Determines if the Ultimate Radial Menu should be disabled when the interaction occurs. \n\nNOTE: World space radial menus will not be disabled on interact. They must be disabled manually." )]
//	public bool disableOnInteract = false;

//	// MOUSE SETTINGS //
//	[Header( "Mouse and Keyboard Settings" )]
//	[Tooltip( "Determines if mouse and keyboard input should be used to send to the Ultimate Radial Menu." )]
//	public bool keyboardInput = true;
//	[Tooltip( "The mouse button index to use for interacting." )]
//	public int mouseButtonIndex = 0;
//	[Tooltip( "Determines if this Input Manager should handle the Enabling/Disabling of the Ultimate Radial Menu or if the user will do it manually." )]
//	public bool enableWithKeyboard = true;
//	[Tooltip( "The input key used for enabling and disabling the Ultimate Radial Menu." )]
//	public string enableButtonKeyboard = "Submit";

//	// CONTROLLER SETTINGS //
//	[Header( "Controller Settings" )]
//	[Tooltip( "Determines if controller input should be used to send to the Ultimate Radial Menu." )]
//	public bool controllerInput = false;
//	[Tooltip( "The input key for the controller horizontal axis." )]
//	public string horizontalAxisController = "Horizontal";
//	[Tooltip( "The input key for the controller vertical axis." )]
//	public string verticalAxisController = "Vertical";
//	[Tooltip( "The input key for the controller button interaction." )]
//	public string interactButtonController = "Cancel";
//	[Tooltip( "Determines if this Input Manager should handle the Enabling/Disabling of the Ultimate Radial Menu or if the user will do it manually." )]
//	public bool enableWithController = true;
//	[Tooltip( "The input key used for enabling and disabling the Ultimate Radial Menu." )]
//	public string enableButtonController = "Submit";
//	[Tooltip( "Determines if the horizontal input should be inverted or not." )]
//	public bool invertHorizontal = false;
//	[Tooltip( "Determines if the vertical input should be inverted or not." )]
//	public bool invertVertical = false;

//	// TOUCH SETTINGS //
//	[ Header( "Touch Settings" )]
//	[Tooltip( "Determines if touch input should be used to send to the Ultimate Radial Menu." )]
//	public bool touchInput = false;
//	[Tooltip( "Determines if this Input Manager should handle the Enabling/Disabling of the Ultimate Radial Menu or if the user will do it manually." )]
//	public bool enableWithTouch = true;
//	[Tooltip( "Should the radial menu move to the initial touch position?" )]
//	public bool dynamicPositioning = false;
//	[Range( 0.0f, 2.0f )]
//	[Tooltip( "The activation radius for enabling the menu." )]
//	public float activationRadius = 0.25f;
//	[Tooltip( "Time in seconds that the user needs to hold the touch within the activation radius." )]
//	public float activationHoldTime = 0.25f;
//	class TouchHoldInformation
//	{
//		public float currentHoldTime = 0.0f;
//		public int interactFingerID = -1;
//		public UltimateRadialMenu radialMenu;
//		public bool touchActivatedRadialMenu = false;


//		public void ResetMenuPosition ()
//		{
//			if( !touchActivatedRadialMenu )
//				return;

//			touchActivatedRadialMenu = false;

//			if( radialMenu != null )
//				radialMenu.ResetPosition();
//		}
//	}
//	List<TouchHoldInformation> TouchHoldInformations = new List<TouchHoldInformation>();
//	bool touchInformationReset = true;

//	// VR SETTINGS //
//	[Header( "Virtual Reality Settings" )]
//	[Tooltip( "Determines if the menu should activated by the center of the screen." )]
//	public bool virtualRealityInput = false;
//	[Tooltip( "The input key for the virtual reality button interaction." )]
//	public string interactButtonVirtualReality = "Submit";

//	// CUSTOM INPUT SETTINGS //
//	[Header( "Custom Input Settings" )]
//	public bool customInput = false;
	

//	void Awake ()
//	{
//		// If this input manager is not located on the event system or an Ultimate Radial Menu object...
//		if( !GetComponent<EventSystem>() && !GetComponent<UltimateRadialMenu>() )
//		{
//			// Log an error to the user explaining the issue and what to do to fix it.
//			Debug.LogError( "Ultimate Radial Menu Input Manager\nThis component is not attached to the EventSystem in your scene or an Ultimate Radial Menu component. Please make sure that you have only one Ultimate Radial Menu Input Manager in your scene and that it is located on the EventSystem, unless you want unique controller input then place the Ultimate Radial Menu Input Manager on the Ultimate Radial Menu component that you want to have unique controller input." );

//			// Destroy this component and return.
//			Destroy( this );
//			return;
//		}

//		// If this gameObject have the EventSystem then assign the instance as this.
//		if( GetComponent<EventSystem>() )
//			Instance = this;
//		else
//			isUniqueInputManager = true;

//		// Reset the Informations list.
//		UltimateRadialMenuInformations = new List<UltimateRadialMenuInfomation>();

//		// Store the LayerMask for the UI so that it can be used for world space menus.
//		worldSpaceMask = LayerMask.GetMask( "UI" );
//	}

//	void Start ()
//	{
//		// Set the main camera for calculations.
//		UpdateCamera();
//	}
	
//	/// <summary>
//	/// [INTERNAL] Called by each Ultimate Radial Menu.
//	/// </summary>
//	public void AddRadialMenuToList ( UltimateRadialMenu radialMenu, ref UltimateRadialMenuInputManager inputManager )
//	{
//		// Add this radial menu to the list for calculations.
//		UltimateRadialMenuInformations.Add( new UltimateRadialMenuInfomation() { radialMenu = radialMenu } );

//		// If the user wants to use touch input, then store this radial menu.
//		if( touchInput )
//		{
//			TouchHoldInformations.Add( new TouchHoldInformation() { radialMenu = radialMenu } );
//			radialMenu.OnRadialMenuDisabled += TouchHoldInformations[ TouchHoldInformations.Count - 1 ].ResetMenuPosition;
//		}

//		inputManager = this;
//	}

//	/// <summary>
//	/// Updates the current camera for calculations.
//	/// </summary>
//	protected void UpdateCamera ()
//	{
//		// Find all the cameras in the scene.
//		Camera[] sceneCameras = FindObjectsOfType<Camera>();

//		// Loop through each camera.
//		for( int i = 0; i < sceneCameras.Length; i++ )
//		{
//			// If the camera gameObject is active and the camera component is enabled...
//			if( sceneCameras[ i ].gameObject.activeInHierarchy && sceneCameras[ i ].enabled )
//			{
//				// Set this camera to the main camera.
//				mainCamera = sceneCameras[ i ];

//				// If this camera is tagged as MainCamera, then break the loop. Otherwise, keep looking for a MainCamera.
//				if( sceneCameras[ i ].tag == "MainCamera" )
//					break;
//			}
//		}
//	}

//	/// <summary>
//	/// Sets the camera to the provided camera parameter for calculations.
//	/// </summary>
//	/// <param name="newMainCamera">The new camera to use for calculations.</param>
//	public void SetMainCamera ( Camera newMainCamera )
//	{
//		mainCamera = newMainCamera;
//	}

//	/// <summary>
//	/// Performs a physics raycast using the provided input information to see if the input collides with any world space menus.
//	/// </summary>
//	protected void RaycastWorldSpaceRadialMenu ( ref Vector2 input, ref float distance, Vector2 rayOrigin, int radialMenuIndex )
//	{
//		// If the current radial menu is not used in world space, then just return.
//		if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
//			return;

//		// If the main camera is null, not active, or the camera component is not enabled, then update the camera reference.
//		if( mainCamera == null || !mainCamera.gameObject.activeInHierarchy || !mainCamera.enabled )
//			UpdateCamera();

//		// Cast a ray from the mouse position.
//		Ray ray = mainCamera.ScreenPointToRay( rayOrigin );

//		// Temporary hit variable to store hit information.
//		RaycastHit hit;

//		// Raycast with the calculated information.
//		if( Physics.Raycast( ray, out hit, Mathf.Infinity, worldSpaceMask ) )
//		{
//			// If the collider that was hit is this radial menu...
//			if( hit.collider.gameObject == UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.gameObject )
//			{
//				// Configure the local 3D Position of hit.
//				Vector3 localHitPosition = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.InverseTransformPoint( hit.point );
				
//				// Assign the input to being the local input value.
//				input = localHitPosition;
				
//				// Configure the distance of the input position from center.
//				distance = Vector3.Distance( Vector2.zero, localHitPosition );
//			}
//		}
//	}

//	void Update ()
//	{
//		// Loop through each of the radial menus.
//		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
//		{
//			// If the menu is null, then it must have been deleted...
//			if( UltimateRadialMenuInformations[ i ].radialMenu == null )
//			{
//				// Update the list and break the loop to avoid errors.
//				UltimateRadialMenuInformations.RemoveAt( i );

//				// If the user has touch input enabled, then remove this index from the list.
//				if( touchInput )
//					TouchHoldInformations.RemoveAt( i );

//				break;
//			}

//			// Booleans to check if we want to enable or disable the radial menu this frame.
//			bool enableMenu = false;
//			bool disableMenu = false;
//			bool inputDown = false;
//			bool inputUp = false;

//			// This is for the current input of the selected Input Type. ( Mouse input for Keyboard controls, and joystick input for controllers )
//			Vector2 input = Vector2.zero;

//			// This will store the distance from the center of the radial menu to help calculate if the input is within range.
//			float distance = 0.0f;

//			// If the user wants to use keyboard input then run the MouseAndKeyboardInput function.
//			if( keyboardInput )
//				MouseAndKeyboardInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

//			// If the user wants to use controller input then run the ControllerInput function.
//			if( controllerInput )
//				ControllerInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

//			// If the user wants to use touch input, then call the TouchInput function.
//			if( touchInput )
//				TouchInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

//			// If the user wants to use any VR devices then run the VirtualRealityInput function.
//			if( virtualRealityInput )
//				VirtualRealityInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

//			// If the user has created custom input, then call that here.
//			if( customInput )
//				CustomInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

//			// If we want to activate the radial menu when we release the menu when hovering over a button...
//			if( onMenuRelease )
//			{
//				// Check the last known radial menu state to see if it was active. If we are going to disable the menu on this frame and the last known state was true, then set interact to true.
//				if( UltimateRadialMenuInformations[ i ].lastRadialMenuState == true && disableMenu == true )
//					inputDown = inputUp = true;
//			}

//			// Send all of the calculations to the Ultimate Radial Menu to process.
//			UltimateRadialMenuInformations[ i ].radialMenu.ProcessInput( input, distance, inputDown, inputUp );

//			// If we want to enable the radial menu on this frame then do that here.
//			if( enableMenu )
//				UltimateRadialMenuInformations[ i ].radialMenu.EnableRadialMenu();

//			// Same this for the disable. Do that here.
//			if( disableMenu )
//				UltimateRadialMenuInformations[ i ].radialMenu.DisableRadialMenu();

//			// Store the last known state for calculations.
//			UltimateRadialMenuInformations[ i ].lastRadialMenuState = UltimateRadialMenuInformations[ i ].radialMenu.RadialMenuActive;
//		}
//	}

//	/// <summary>
//	/// This function will catch input from the Mouse and Keyboard and modify the information to send back to the Update function.
//	/// </summary>
//	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
//	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
//	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
//	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
//	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
//	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
//	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
//	public virtual void MouseAndKeyboardInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
//	{
//		// If this radial menu is world space then send in the information to raycast from.
//		if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
//			RaycastWorldSpaceRadialMenu( ref input, ref distance, Input.mousePosition, radialMenuIndex );
//		// Else the radial menu is on the screen, so process mouse input.
//		else
//		{
//			// If there is a mouse present...
//			if( Input.mousePresent )
//			{
//				// Figure out the position of the input on the canvas. ( mouse position / canvas scale factor ) - ( half the canvas size );
//				Vector2 inputPositionOnCanvas = ( new Vector2( Input.mousePosition.x, Input.mousePosition.y ) / UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.scaleFactor ) - ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.GetComponent<RectTransform>().sizeDelta / 2 );

//				// Apply our new calculated input. ( input position - local position of the menu ) / ( half the menu size );
//				input = ( inputPositionOnCanvas - ( Vector2 )UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition ) / ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.sizeDelta.x / 2 );

//				// Configure the distance of the mouse position from the Radial Menu's base position.
//				distance = Vector2.Distance( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition, inputPositionOnCanvas );
//			}
//		}
		
//		// Check for the mouse button being pressed down, and if so set activate to true.
//		if( Input.GetMouseButtonDown( mouseButtonIndex ) )
//			inputDown = true;
//		if( Input.GetMouseButtonUp( mouseButtonIndex ) )
//			inputUp = true;

//		// If the user has a enable key assigned...
//		if( enableWithKeyboard && enableButtonKeyboard != string.Empty && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
//		{
//			// Check for the Enable and Disable button keys and set the enable or disable booleans accordingly.
//			if( Input.GetButtonDown( enableButtonKeyboard ) )
//				enableMenu = true;
//			else if( Input.GetButtonUp( enableButtonKeyboard ) )
//				disableMenu = true;
//		}
//	}

//	/// <summary>
//	/// This function will catch input from the Controller and modify the information to send back to the Update function.
//	/// </summary>
//	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
//	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
//	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
//	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
//	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
//	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
//	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
//	public virtual void ControllerInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
//	{
//		// Store the horizontal and vertical axis of the targeted joystick axis.
//		Vector2 controllerInput = new Vector2( Input.GetAxis( horizontalAxisController ), Input.GetAxis( verticalAxisController ) );

//		// If the user wants to invert the horizontal axis, then multiply by -1.
//		if( invertHorizontal )
//			controllerInput.x *= -1;

//		// If the user wants to invert the vertical axis, then do that here.
//		if( invertVertical )
//			controllerInput.y *= -1;

//		// Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
//		float tempDist = Vector2.Distance( Vector2.zero, controllerInput );

//		// Set the input to what we have calculated.
//		if( controllerInput != Vector2.zero )
//			input = controllerInput;

//		// If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
//		if( tempDist >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.minRange )
//			distance = Mathf.Lerp( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange, 0.5f );

//		// If the activation action is set to being the press of a button on the controller...
//		if( Input.GetButtonDown( interactButtonController ) )
//			inputDown = true;
//		else if( Input.GetButtonUp( interactButtonController ) )
//			inputUp = true;

//		// If the user has a enable key assigned...
//		if( enableWithController && enableButtonController != string.Empty && ( isUniqueInputManager || !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu ) )
//		{
//			// Check for the Enable and Disable button keys and set the enable or disable booleans accordingly.
//			if( Input.GetButtonDown( enableButtonController ) )
//				enableMenu = true;
//			else if( Input.GetButtonUp( enableButtonController ) )
//				disableMenu = true;
//		}
//	}

//	/// <summary>
//	/// This function will catch touch input and modify the information to send back to the Update function.
//	/// </summary>
//	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
//	public virtual void TouchInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
//	{
//		// If there are touches on the screen...
//		if( Input.touchCount > 0 )
//		{
//			// If the touch information is reset, then set to false so that it will be reset when there are no touches on the screen.
//			if( touchInformationReset )
//				touchInformationReset = false;

//			for( int i = 0; i < Input.touchCount; i++ )
//			{
//				// If a finger id has been stored, and this finger id is not the same as the stored finger id, then continue.
//				if( TouchHoldInformations[ radialMenuIndex ].interactFingerID >= 0 && TouchHoldInformations[ radialMenuIndex ].interactFingerID != Input.GetTouch( i ).fingerId )
//					continue;

//				// Configure the menu radius for calculations.
//				float menuRadius = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.sizeDelta.x / 2;

//				// Store the touch position on the canvas. ( touch position / canvas scale factor ) - ( half the canvas size );
//				Vector2 touchPositionOnCanvas = ( Input.GetTouch( i ).position / UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.scaleFactor ) - ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.GetComponent<RectTransform>().sizeDelta / 2 );

//				// By subtracting the mouse position from the radial menu's position we get a relative number. Then we divide by the height of the screen space to give us an easier and more consistent number to work with.
//				Vector2 modInput = ( touchPositionOnCanvas - ( Vector2 )UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition ) / menuRadius;

//				// Configure the distance of the mouse position from the Radial Menu's base position.
//				float dist = Vector2.Distance( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition, touchPositionOnCanvas );

//				// If the radial menu is used in world space, then raycast the input.
//				if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
//					RaycastWorldSpaceRadialMenu( ref modInput, ref dist, Input.GetTouch( i ).position, radialMenuIndex );

//				// If the input phase began, then store the finger id.
//				if( Input.GetTouch( i ).phase == TouchPhase.Began )
//				{
//					// If the input is within distance of the target, then store the finger id.
//					if( !enableWithTouch || dist < menuRadius * activationRadius )
//						TouchHoldInformations[ radialMenuIndex ].interactFingerID = Input.GetTouch( i ).fingerId;
					
//					// If the radial menu is active...
//					if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
//					{
//						// Configure a deactivation radius by using the radial menu maxRange.
//						float deactivationRadius = TouchHoldInformations[ radialMenuIndex ].radialMenu.maxRange;

//						// If the user wants the input radius to be infinite then set that here.
//						if( TouchHoldInformations[ radialMenuIndex ].radialMenu.infiniteMaxRange )
//							deactivationRadius = Mathf.Infinity;

//						// Set the input to down since the touch began.
//						inputDown = true;

//						// If the distance is within the input range of the radial menu, then store the finger id.
//						if( dist > UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange && dist < UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange )
//							TouchHoldInformations[ radialMenuIndex ].interactFingerID = Input.GetTouch( i ).fingerId;
//						// Else if the distance of the input is out of the deactivation range.
//						else if( enableWithTouch && ( dist > menuRadius * deactivationRadius || dist < TouchHoldInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange ) && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu && activationRadius > 0 )
//							UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();
//					}
//				}

//				// If the finger id is still not stored, then just continue to the next touch.
//				if( TouchHoldInformations[ radialMenuIndex ].interactFingerID == -1 )
//					continue;
				
//				// If the radial menu is not active and not in the middle of a transition...
//				if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.InTransition )
//				{
//					if( enableWithTouch && dist < menuRadius * activationRadius )
//					{
//						// Increases the current hold time for calculations to enable.
//						TouchHoldInformations[ radialMenuIndex ].currentHoldTime += Time.deltaTime;

//						// If the current hold time has reached the target time...
//						if( TouchHoldInformations[ radialMenuIndex ].currentHoldTime >= activationHoldTime )
//						{
//							// Reset the current hold time, and enable the menu.
//							TouchHoldInformations[ radialMenuIndex ].currentHoldTime = 0.0f;
//							UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.EnableRadialMenu();

//							// Since the radial menu was enabled using text, set touchActivatedRadialMenu to true.
//							TouchHoldInformations[ radialMenuIndex ].touchActivatedRadialMenu = true;

//							// If the user wants to move to the touch position, move to that position now.
//							if( dynamicPositioning && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
//								UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.SetPosition( touchPositionOnCanvas, true );
//						}
//					}
//				}
//				else
//				{
//					// Store the input to send to the radial menu.
//					input = modInput;
					
//					// Store the distance as well.
//					distance = dist;
//				}

//				// If the touch has ended...
//				if( Input.GetTouch( i ).phase == TouchPhase.Ended )
//				{
//					// Reset the finger id.
//					TouchHoldInformations[ radialMenuIndex ].interactFingerID = -1;

//					// Reset the current hold time.
//					TouchHoldInformations[ radialMenuIndex ].currentHoldTime = 0.0f;

//					// Set input down and up to true so that the interact function will be called on the button.
//					inputDown = true;
//					inputUp = true;
					
//					// If the input is not over a button when released, then just disable the menu.
//					if( enableWithTouch && UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex < 0 && dist > menuRadius * activationRadius && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu && activationRadius > 0 )
//						UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();
//				}
//			}
//		}
//		else
//		{
//			// If the touch information has not been reset...
//			if( !touchInformationReset )
//			{
//				// Set touchInformationReset so that it will not be reset again until there are touches on the screen again.
//				touchInformationReset = true;

//				// Loop through each information and reset the values.
//				for( int i = 0; i < TouchHoldInformations.Count; i++ )
//				{
//					TouchHoldInformations[ i ].currentHoldTime = 0.0f;
//					TouchHoldInformations[ i ].interactFingerID = -1;
//				}
//			}
//		}
//	}

//	/// <summary>
//	/// This function will catch input from the center of the screen and VR device and modify the information to send back to the Update function.
//	/// </summary>
//	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
//	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
//	public virtual void VirtualRealityInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
//	{
//		// If the radial menu is not being used in world space, then just return.
//		if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
//			return;

//		// Raycast from the center of the screen.
//		RaycastWorldSpaceRadialMenu( ref input, ref distance, new Vector3( Screen.width / 2, Screen.height / 2, 0 ), radialMenuIndex );

//		// If the activation action is set to being the press of a button on the controller...
//		if( Input.GetButtonDown( interactButtonVirtualReality ) )
//			inputDown = true;
//		else if( Input.GetButtonUp( interactButtonVirtualReality ) )
//			inputUp = true;
//	}

//	/// <summary>
//	/// This function is a virtual void to allow for easy custom input logic.
//	/// </summary>
//	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
//	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
//	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
//	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
//	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
//	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
//	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
//	public virtual void CustomInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
//	{
//		// WARNING! This is not where you want to put your custom logic. Please check out our video tutorials for more information.
//		// Video Tutorials: https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu
//	}
//}