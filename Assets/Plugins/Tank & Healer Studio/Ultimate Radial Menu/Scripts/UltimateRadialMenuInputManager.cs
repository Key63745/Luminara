/* UltimateRadialMenuInputManager.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif

public class UltimateRadialMenuInputManager : MonoBehaviour
{
	public static UltimateRadialMenuInputManager Instance
	{
		get;
		private set;
	}
	protected Camera mainCamera;
	public class UltimateRadialMenuInfomation
	{
		public UltimateRadialMenu radialMenu;
		public bool lastRadialMenuState = false;

		public bool customInputThisFrame;
		public Vector2 customInput;
		public float customDistance;
		public bool customInputDown, customInputUp;

		public float interactHoldTime = 0.0f;
		public int currentButtonIndex = -1;
	}
	public List<UltimateRadialMenuInfomation> UltimateRadialMenuInformations
	{
		get;
		private set;
	}
	LayerMask worldSpaceMask;
	bool isUniqueInputManager = false;

	public enum InputDevice
	{
		None,
		Mouse,
		Controller,
		Touch,
		CenterScreen,
		Other
	}
	public InputDevice CurrentInputDevice
	{
		get;
		private set;
	}

	// INTERACT SETTINGS //
	public enum InvokeAction
	{
		OnButtonDown,
		OnButtonClick
	}
	[Header( "Interact Settings" )]
	[Tooltip( "The action required to invoke the radial button." )]
	public InvokeAction invokeAction = InvokeAction.OnButtonDown;
	[Tooltip( "Determines whether or not the Ultimate Radial Menu will receive input when the Ultimate Radial Menu is released and disabled." )]
	public bool onMenuRelease = false;
	[Tooltip( "Determines if the Ultimate Radial Menu should be disabled when the interaction occurs. \n\nNOTE: World space radial menus will not be disabled on interact. They must be disabled manually." )]
	public bool disableOnInteract = false;
	public enum EnableMenuSetting
	{
		Toggle,
		Hold,
		Manual
	}
	[Tooltip( "Determines if this Input Manager should handle enabling and disabling the radial menus in the scene, and how that should be done." )]
	public EnableMenuSetting enableMenuSetting = EnableMenuSetting.Hold;

	// MOUSE SETTINGS //
	[Header( "Mouse and Keyboard Settings" )]
	[Tooltip( "Determines if mouse and keyboard input should be used to send to the Ultimate Radial Menu." )]
	public bool keyboardInput = true;
	public enum MouseInteractButtons
	{
		Left,
		Right,
		Both
	}
	[Tooltip( "The mouse button to be used to interact with the radial menus." )]
	public MouseInteractButtons mouseInteractButton;
	public enum KeyboardEnableKeys
	{
		Tab,
		LeftAlt,
		LeftControl,
		EnableManually,
	}
	[Tooltip( "The keyboard key to be used to enable/disable the radial menus." )]
	public KeyboardEnableKeys keyboardEnableKey;
	
	// CONTROLLER SETTINGS //
	[Header( "Controller Settings" )]
	[Tooltip( "Determines if controller input should be used to send to the Ultimate Radial Menu." )]
	public bool controllerInput = false;
	[Tooltip( "Determines if the horizontal input should be inverted or not." )]
	public bool invertHorizontal = false;
	[Tooltip( "Determines if the vertical input should be inverted or not." )]
	public bool invertVertical = false;
#if ENABLE_INPUT_SYSTEM
	public enum Joysticks
	{
		Left,
		Right
	}
	[Tooltip( "The controller joystick to be used for navigating the radial menu." )]
	public Joysticks joystick = Joysticks.Left;
	[System.Flags]
	public enum ControllerButtons
	{
		Nothing =		0,
		North =			1,
		South =			2,
		East =			4,
		West =			8,
		LeftJoystick =	16,
		LeftShoulder =	32,
		LeftTrigger =	64,
		RightJoystick =	128,
		RightShoulder =	256,
		RightTrigger =	512,
		Start =			1024,
		Select =		2048,
		DpadUp =		4096,
		DpadDown =		8192,
		DpadLeft =		16384,
		DpadRight =		32768,
	}
	[Tooltip( "The buttons to be used for interacting with the radial menu buttons." )]
	public ControllerButtons interactButtons;
	[Tooltip( "The buttons to be used for enabling the radial menu." )]
	public ControllerButtons enableButtons;
#else
	[Tooltip( "The input key for the controller horizontal axis." )]
	public string horizontalAxisController = "Horizontal";
	[Tooltip( "The input key for the controller vertical axis." )]
	public string verticalAxisController = "Vertical";
	[Tooltip( "The input key for the controller button interaction." )]
	public string interactButtonController = "Cancel";
	[Tooltip( "The input key used for enabling and disabling the Ultimate Radial Menu." )]
	public string enableButtonController = "Submit";
#endif

	// TOUCH SETTINGS //
	[Header( "Touch Settings" )]
	[Tooltip( "Determines if touch input should be used to send to the Ultimate Radial Menu." )]
	public bool touchInput = false;
	[Tooltip( "Should the radial menu move to the initial touch position?" )]
	public bool dynamicPositioning = false;
	[Range( 0.0f, 2.0f )]
	[Tooltip( "The activation radius for enabling the menu." )]
	public float activationRadius = 0.25f;
	[Tooltip( "Time in seconds that the user needs to hold the touch within the activation radius." )]
	public float activationHoldTime = 0.25f;
	class TouchHoldInformation
	{
		public float currentHoldTime = 0.0f;
		public int interactFingerID = -1;
		public UltimateRadialMenu radialMenu;
		public bool touchActivatedRadialMenu = false;


		public void ResetMenuPosition ()
		{
			if( !touchActivatedRadialMenu )
				return;

			touchActivatedRadialMenu = false;

			if( radialMenu != null )
				radialMenu.ResetPosition();
		}
	}
	List<TouchHoldInformation> TouchHoldInformations = new List<TouchHoldInformation>();
	bool touchInformationReset = true;

	// CENTER SCREEN SETTINGS //
	[Header( "Center Screen Settings" )]
	[Tooltip( "Determines if the menu should activated by the center of the screen." )]
	public bool centerScreenInput = false;
	[Tooltip( "Should hovering over the menu for an amount of time interact with the menu?" )]
	public bool interactOnHover = false;
	[Tooltip( "Time is seconds the player must hover over a button to interact with it." )]
	public float interactHoverTime = 1.0f;
	[Tooltip( "Should the calculations use two cameras in order to calculate the center of where the player is looking?" )]
	public bool virtualReality = false;
	public Camera leftEyeCamera, rightEyeCamera;

	// CUSTOM INPUT SETTINGS //
	[Header( "Custom Input Settings" )]
	public bool customInput = false;
	

	protected virtual void Awake ()
	{
		// If this input manager is not located on the event system or an Ultimate Radial Menu object...
		if( !GetComponent<EventSystem>() && !GetComponent<UltimateRadialMenu>() )
		{
			// Log an error to the user explaining the issue and what to do to fix it.
			Debug.LogError( "Ultimate Radial Menu Input Manager\nThis component is not attached to the EventSystem in your scene or an Ultimate Radial Menu component. Please make sure that you have only one Ultimate Radial Menu Input Manager in your scene and that it is located on the EventSystem, unless you want unique controller input then place the Ultimate Radial Menu Input Manager on the Ultimate Radial Menu component that you want to have unique controller input." );

			// Destroy this component and return.
			Destroy( this );
			return;
		}

		// If this gameObject has the EventSystem component...
		if( GetComponent<EventSystem>() )
		{
			// If the current instance is assigned, and the object still exists and is in hierarchy...
			if( Instance != null && Instance.gameObject != null && Instance.gameObject.activeInHierarchy )
			{
				// Then destroy this component so that we will continue to use our current input manager and return.
				Destroy( this );
				return;
			}

			// Assign this component as the current instance.
			Instance = this;
		}
		// Else make sure it is known that this component is a unique input manager.
		else
			isUniqueInputManager = true;

		// Reset the Informations list.
		UltimateRadialMenuInformations = new List<UltimateRadialMenuInfomation>();

		// Store the LayerMask for the UI so that it can be used for world space menus.
		worldSpaceMask = LayerMask.GetMask( "UI" );

#if ENABLE_INPUT_SYSTEM
		if( touchInput )
			EnhancedTouchSupport.Enable();
#endif
	}

	protected virtual void Start ()
	{
		// Set the main camera for calculations.
		UpdateCamera();
	}
	
	/// <summary>
	/// [INTERNAL] Called by each Ultimate Radial Menu.
	/// </summary>
	public void AddRadialMenuToList ( UltimateRadialMenu radialMenu, ref UltimateRadialMenuInputManager inputManager )
	{
		// Add this radial menu to the list for calculations.
		UltimateRadialMenuInformations.Add( new UltimateRadialMenuInfomation() { radialMenu = radialMenu } );

		// If the user wants to use touch input, then store this radial menu.
		if( touchInput )
		{
			TouchHoldInformations.Add( new TouchHoldInformation() { radialMenu = radialMenu } );
			radialMenu.OnRadialMenuDisabled += TouchHoldInformations[ TouchHoldInformations.Count - 1 ].ResetMenuPosition;
		}

		inputManager = this;
	}

	/// <summary>
	/// Updates the current camera for calculations.
	/// </summary>
	protected void UpdateCamera ()
	{
		// Find all the cameras in the scene.
		Camera[] sceneCameras = FindObjectsOfType<Camera>();

		// Loop through each camera.
		for( int i = 0; i < sceneCameras.Length; i++ )
		{
			// If the camera gameObject is active and the camera component is enabled...
			if( sceneCameras[ i ].gameObject.activeInHierarchy && sceneCameras[ i ].enabled )
			{
				// Set this camera to the main camera.
				mainCamera = sceneCameras[ i ];

				// If this camera is tagged as MainCamera, then break the loop. Otherwise, keep looking for a MainCamera.
				if( sceneCameras[ i ].tag == "MainCamera" )
					break;
			}
		}
	}

	/// <summary>
	/// Sets the camera to the provided camera parameter for calculations.
	/// </summary>
	/// <param name="newMainCamera">The new camera to use for calculations.</param>
	public void SetMainCamera ( Camera newMainCamera )
	{
		mainCamera = newMainCamera;
	}

	/// <summary>
	/// Sets the VR cameras for center screen calculations.
	/// </summary>
	/// <param name="newLeftEyeCamera">The new camera assigned to the left eye of the VR device.</param>
	/// <param name="newRightEyeCamera">The new camera assigned to the right eye of the VR device.</param>
	public void SetCamerasVR ( Camera newLeftEyeCamera, Camera newRightEyeCamera )
	{
		leftEyeCamera = newLeftEyeCamera;
		rightEyeCamera = newRightEyeCamera;
	}

	/// <summary>
	/// Performs a physics raycast using the provided input information to see if the input collides with any world space menus.
	/// </summary>
	protected void RaycastWorldSpaceRadialMenu ( ref Vector2 input, ref float distance, Vector2 rayOrigin, int radialMenuIndex )
	{
		// If the current radial menu is not used in world space, then just return.
		if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
			return;

		// If the main camera is null, not active, or the camera component is not enabled, then update the camera reference.
		if( mainCamera == null || !mainCamera.gameObject.activeInHierarchy || !mainCamera.enabled )
			UpdateCamera();

		// Cast a ray from the mouse position.
		Ray ray = mainCamera.ScreenPointToRay( rayOrigin );

		// Temporary hit variable to store hit information.
		RaycastHit hit;

		// Raycast with the calculated information.
		if( Physics.Raycast( ray, out hit, Mathf.Infinity, worldSpaceMask ) )
		{
			// If the collider that was hit is this radial menu...
			if( hit.collider.gameObject == UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.gameObject )
			{
				// Configure the local 3D Position of hit.
				Vector3 localHitPosition = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.InverseTransformPoint( hit.point );
				
				// Assign the input to being the local input value.
				input = localHitPosition;
				
				// Configure the distance of the input position from center.
				distance = Vector3.Distance( Vector2.zero, localHitPosition );
			}
		}
	}

	/// <summary>
	/// [INTERNAL] Checks and handles how the radial menu is enabled and disabled.
	/// </summary>
	protected void CheckEnableMenu ( bool buttonPressed, bool buttonReleased, int radialMenuIndex )
	{
		// If the user wants to enable the menu only when holding the button...
		if( enableMenuSetting == EnableMenuSetting.Hold )
		{
			// If the button is pressed and the radial menu isn't active, then enable it.
			if( buttonPressed && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
				UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.EnableRadialMenu();

			// If the button has been released and the radial menu is active, then disable it.
			if( buttonReleased && UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
				UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();
		}
		// Else the user wants to toggle the enabled state of the radial menu.
		else
		{
			// If the button has been pressed...
			if( buttonPressed )
			{
				// If the radial menu is currently disabled, then enable it.
				if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
					UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.EnableRadialMenu();
				// Else disable the menu.
				else
					UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();
			}
		}
	}

	void Update ()
	{
		// Reset the last known input device.
		CurrentInputDevice = InputDevice.None;

		// Loop through each of the radial menus.
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			// If the menu is null, then it must have been deleted...
			if( UltimateRadialMenuInformations[ i ].radialMenu == null )
			{
				// Update the list and break the loop to avoid errors.
				UltimateRadialMenuInformations.RemoveAt( i );

				// If the user has touch input enabled, then remove this index from the list.
				if( touchInput )
					TouchHoldInformations.RemoveAt( i );

				break;
			}

			// Booleans to check if we want to enable or disable the radial menu this frame.
			bool enableMenu = false;
			bool disableMenu = false;
			bool inputDown = false;
			bool inputUp = false;

			// This is for the current input of the selected Input Type. ( Mouse input for Keyboard controls, and joystick input for controllers )
			Vector2 input = Vector2.zero;

			// This will store the distance from the center of the radial menu to help calculate if the input is within range.
			float distance = 0.0f;

			// If the user wants to use keyboard input then run the MouseAndKeyboardInput function.
			if( keyboardInput )
				MouseAndKeyboardInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user wants to use controller input then run the ControllerInput function.
			if( controllerInput )
				ControllerInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user wants to use touch input, then call the TouchInput function.
			if( touchInput )
				TouchInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user wants to use any VR devices then run the VirtualRealityInput function.
			if( centerScreenInput )
				CenterScreenInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user has created custom input, then call that here.
			if( customInput )
				CustomInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user has sent in some custom calculated input this frame...
			if( UltimateRadialMenuInformations[ i ].customInputThisFrame )
			{
				// If the input value is assigned, then copy the custom input values.
				if( UltimateRadialMenuInformations[ i ].customInput != Vector2.zero )
				{
					input = UltimateRadialMenuInformations[ i ].customInput;
					distance = UltimateRadialMenuInformations[ i ].customDistance;
				}

				// If the custom input has been set, then copy it.
				if( UltimateRadialMenuInformations[ i ].customInputDown )
					inputDown = true;

				// If the custom input up was set this frame, copy it.
				if( UltimateRadialMenuInformations[ i ].customInputUp )
					inputUp = true;
				
				// Since the input down and up was caught, reset it.
				UltimateRadialMenuInformations[ i ].customInputDown = false;
				UltimateRadialMenuInformations[ i ].customInputUp = false;
			}

			// If we want to activate the radial menu when we release the menu when hovering over a button...
			if( onMenuRelease )
			{
				// Check the last known radial menu state to see if it was active. If we are going to disable the menu on this frame and the last known state was true, then set interact to true.
				if( UltimateRadialMenuInformations[ i ].lastRadialMenuState == true && disableMenu == true )
					inputDown = inputUp = true;
			}

			// If the user wants to enable the menus through the input manager, check for that.
			if( enableMenuSetting != EnableMenuSetting.Manual )
				CheckEnableMenu( enableMenu, disableMenu, i );

			// Send all of the calculations to the Ultimate Radial Menu to process.
			UltimateRadialMenuInformations[ i ].radialMenu.ProcessInput( input, distance, inputDown, inputUp );
			
			// Store the last known state for calculations.
			UltimateRadialMenuInformations[ i ].lastRadialMenuState = UltimateRadialMenuInformations[ i ].radialMenu.RadialMenuActive;
		}
	}

	/// <summary>
	/// This function will catch input from the Mouse and Keyboard and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void MouseAndKeyboardInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		Vector2 mousePosition = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
		// Store the mouse from the input system.
		Mouse mouse = InputSystem.GetDevice<Mouse>();

		// If the mouse couldn't be stored from the input system, then return.
		if( mouse == null )
			return;

		// Store the mouse position.
		mousePosition = mouse.position.ReadValue();

		// If the user doesn't want to exclusively use the right mouse button for interacting...
		if( mouseInteractButton != MouseInteractButtons.Right )
		{
			// Check the state of the left mouse button.
			if( mouse.leftButton.wasPressedThisFrame )
				inputDown = true;
			if( mouse.leftButton.wasReleasedThisFrame )
				inputUp = true;
		}

		// If the user doesn't want to exclusively use the left mouse button...
		if( mouseInteractButton != MouseInteractButtons.Left )
		{
			// Then check the state of the right mouse button.
			if( mouse.rightButton.wasPressedThisFrame )
				inputDown = true;
			if( mouse.rightButton.wasReleasedThisFrame )
				inputUp = true;
		}

		// Store the keyboard from the input system.
		Keyboard keyboard = InputSystem.GetDevice<Keyboard>();

		// If the user wants to enable the radial menus through this input manager, and the keyboard was found, and this is not a world space menu...
		if( enableMenuSetting != EnableMenuSetting.Manual && keyboardEnableKey != KeyboardEnableKeys.EnableManually && keyboard != null && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
		{
			// Catch the input from the key that the user wants to use.
			switch( keyboardEnableKey )
			{
				default:
				case KeyboardEnableKeys.Tab:
				{
					enableMenu = keyboard.tabKey.wasPressedThisFrame;
					disableMenu = keyboard.tabKey.wasReleasedThisFrame;
				}
				break;
				case KeyboardEnableKeys.LeftAlt:
				{
					enableMenu = keyboard.leftAltKey.wasPressedThisFrame;
					disableMenu = keyboard.leftAltKey.wasReleasedThisFrame;
				}
				break;
				case KeyboardEnableKeys.LeftControl:
				{
					enableMenu = keyboard.leftCtrlKey.wasPressedThisFrame;
					if( !enableMenu )
						enableMenu = keyboard.leftCommandKey.wasPressedThisFrame;
					
					disableMenu = keyboard.leftCtrlKey.wasReleasedThisFrame;
					if( !disableMenu )
						disableMenu = keyboard.leftCommandKey.wasReleasedThisFrame;
				}
				break;
			}
		}
#else
		// If the mouse is not present, then just return.
		if( !Input.mousePresent )
			return;

		// Store the mouse position.
		mousePosition = Input.mousePosition;

		// If the user doesn't want to exclusively use the right mouse button for interacting...
		if( mouseInteractButton != MouseInteractButtons.Right )
		{
			// Check the state of the left mouse button.
			if( Input.GetMouseButtonDown( 0 ) )
				inputDown = true;
			if( Input.GetMouseButtonUp( 0 ) )
				inputUp = true;
		}

		// If the user doesn't want to exclusively use the left mouse button...
		if( mouseInteractButton != MouseInteractButtons.Left )
		{
			// Then check the state of the right mouse button.
			if( Input.GetMouseButtonDown( 1 ) )
				inputDown = true;
			if( Input.GetMouseButtonUp( 1 ) )
				inputUp = true;
		}
		
		// If the keyboard enable button is assigned, and the menu is not in world space...
		if( enableMenuSetting != EnableMenuSetting.Manual && keyboardEnableKey != KeyboardEnableKeys.EnableManually && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
		{
			switch( keyboardEnableKey )
			{
				default:
				case KeyboardEnableKeys.Tab:
				{
					enableMenu = Input.GetKeyDown( KeyCode.Tab );
					disableMenu = Input.GetKeyUp( KeyCode.Tab );
				}
				break;
				case KeyboardEnableKeys.LeftAlt:
				{
					enableMenu = Input.GetKeyDown( KeyCode.LeftAlt );
					disableMenu = Input.GetKeyUp( KeyCode.LeftAlt );
				}
				break;
				case KeyboardEnableKeys.LeftControl:
				{
					enableMenu = Input.GetKeyDown( KeyCode.LeftControl );
					disableMenu = Input.GetKeyUp( KeyCode.LeftControl );

					if( !enableMenu )
						enableMenu = Input.GetKeyDown( KeyCode.LeftCommand );

					if( !disableMenu )
						disableMenu = Input.GetKeyUp( KeyCode.LeftCommand );
				}
				break;
			}
		}
#endif
		// Set the current input device for reference.
		CurrentInputDevice = InputDevice.Mouse;

		// If this radial menu is world space then send in the information to raycast from.
		if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
			RaycastWorldSpaceRadialMenu( ref input, ref distance, mousePosition, radialMenuIndex );
		// Else the radial menu is on the screen, so process mouse input.
		else
		{
			// Figure out the position of the input on the canvas. ( mouse position / canvas scale factor ) - ( half the canvas size );
			Vector2 inputPositionOnCanvas = ( mousePosition / UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.scaleFactor ) - ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.GetComponent<RectTransform>().sizeDelta / 2 );

			// Apply our new calculated input. ( input position - local position of the menu ) / ( half the menu size );
			input = ( inputPositionOnCanvas - ( Vector2 )UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition ) / ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.sizeDelta.x / 2 );

			// Configure the distance of the mouse position from the Radial Menu's base position.
			distance = Vector2.Distance( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition, inputPositionOnCanvas );
		}
	}

	/// <summary>
	/// This function will catch input from the Controller and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void ControllerInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// Store the horizontal and vertical axis of the targeted joystick axis.
		Vector2 controllerInput = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
		// Store the gamepad from the input system.
		Gamepad gamepad = InputSystem.GetDevice<Gamepad>();

		// If the gamepad is null, then just return.
		if( gamepad == null )
			return;
		
		// Store the input data of the stick determined by the user.
		controllerInput = joystick == Joysticks.Left ? gamepad.leftStick.ReadValue() : gamepad.rightStick.ReadValue();

		// Check the controller buttons for interacting.
		CheckControllerButtons( gamepad, interactButtons, ref inputDown, ref inputUp );

		// Check the controller buttons for enabling/disabling the menu if the user wants that.
		if( enableMenuSetting != EnableMenuSetting.Manual && ( isUniqueInputManager || !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu ) )
			CheckControllerButtons( gamepad, enableButtons, ref enableMenu, ref disableMenu );
#else
		// Store the horizontal and vertical axis of the targeted joystick axis.
		controllerInput = new Vector2( Input.GetAxis( horizontalAxisController ), Input.GetAxis( verticalAxisController ) );

		// If the activation action is set to being the press of a button on the controller...
		if( Input.GetButtonDown( interactButtonController ) )
			inputDown = true;
		else if( Input.GetButtonUp( interactButtonController ) )
			inputUp = true;
		
		// If the user has a enable key assigned...
		if( enableMenuSetting != EnableMenuSetting.Manual && enableButtonController != string.Empty && ( isUniqueInputManager || !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu ) )
		{
			// Check for the Enable and Disable button keys and set the enable or disable booleans accordingly.
			if( Input.GetButtonDown( enableButtonController ) )
				enableMenu = true;
			else if( Input.GetButtonUp( enableButtonController ) )
				disableMenu = true;
		}
#endif
		// If the user wants to invert the horizontal axis, then multiply by -1.
		if( invertHorizontal )
			controllerInput.x *= -1;

		// If the user wants to invert the vertical axis, then do that here.
		if( invertVertical )
			controllerInput.y *= -1;

		// Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
		float tempDist = Vector2.Distance( Vector2.zero, controllerInput );

		// If the controller input is not zero...
		if( controllerInput != Vector2.zero )
		{
			// Set the current input device for reference.
			CurrentInputDevice = InputDevice.Controller;

			// Set the input to what was calculated.
			input = controllerInput;
		}

		// If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
		if( tempDist >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.minRange )
			distance = Mathf.Lerp( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange, 0.5f );
	}

#if ENABLE_INPUT_SYSTEM
	/// <summary>
	/// [INTERNAL] This function checks the gamepad for input.
	/// </summary>
	void CheckControllerButtons ( Gamepad gamepad, ControllerButtons buttonsToCheck, ref bool buttonPress, ref bool buttonRelease )
	{
		// Check for North, South, East and West buttons on the controller.
		if( buttonsToCheck.HasFlag( ControllerButtons.North ) )
		{
			if ( !buttonPress) 
			buttonPress = gamepad.buttonNorth.wasPressedThisFrame;

			if ( !buttonRelease )
			buttonRelease = gamepad.buttonNorth.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.South ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.buttonSouth.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.buttonSouth.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.East ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.buttonEast.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.buttonEast.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.West ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.buttonWest.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.buttonWest.wasReleasedThisFrame;
		}
	
		// Check for left side controller buttons.
		if( buttonsToCheck.HasFlag( ControllerButtons.LeftJoystick ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.leftStickButton.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.leftStickButton.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.LeftShoulder ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.leftShoulder.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.leftShoulder.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.LeftTrigger ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.leftTrigger.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.leftTrigger.wasReleasedThisFrame;
		}
		
		// Check for right side controller buttons.
		if( buttonsToCheck.HasFlag( ControllerButtons.RightJoystick ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.rightStickButton.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.rightStickButton.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.RightShoulder ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.rightShoulder.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.rightShoulder.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.RightTrigger ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.rightTrigger.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.rightTrigger.wasReleasedThisFrame;
		}

		// Check for Start and Select buttons.
		if( buttonsToCheck.HasFlag( ControllerButtons.Start ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.startButton.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.startButton.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.Select ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.selectButton.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.selectButton.wasReleasedThisFrame;
		}

		// Check for Dpad directional buttons.
		if( buttonsToCheck.HasFlag( ControllerButtons.DpadUp ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.dpad.up.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.dpad.up.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.DpadDown ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.dpad.down.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.dpad.down.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.DpadLeft ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.dpad.left.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.dpad.left.wasReleasedThisFrame;
		}
		if( buttonsToCheck.HasFlag( ControllerButtons.DpadRight ) )
		{
			if( !buttonPress )
				buttonPress = gamepad.dpad.right.wasPressedThisFrame;

			if( !buttonRelease )
				buttonRelease = gamepad.dpad.right.wasReleasedThisFrame;
		}
	}
#endif

	/// <summary>
	/// This function will catch touch input and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void TouchInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		int touchCount = 0;

#if ENABLE_INPUT_SYSTEM
		touchCount = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers.Count;
#else
		touchCount = Input.touchCount;
#endif
		// If there are touches on the screen...
		if( touchCount > 0 )
		{
			CurrentInputDevice = InputDevice.Touch;

			// If the touch information is reset, then set to false so that it will be reset when there are no touches on the screen.
			if( touchInformationReset )
				touchInformationReset = false;

			for( int i = 0; i < touchCount; i++ )
			{
				Vector2 touchPosition = Vector2.zero;
				bool touchBegan = false;
				bool touchEnded = false;
				int fingerId = -1;

#if ENABLE_INPUT_SYSTEM
				touchPosition = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers[ fingerId ].screenPosition;
				touchBegan = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers[ fingerId ].currentTouch.phase == UnityEngine.InputSystem.TouchPhase.Began;
				touchEnded = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers[ fingerId ].currentTouch.phase == UnityEngine.InputSystem.TouchPhase.Ended;
				fingerId = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers[ fingerId ].index;
#else
				touchPosition = Input.GetTouch( i ).position;
				touchBegan = Input.GetTouch( i ).phase == TouchPhase.Began;
				touchEnded = Input.GetTouch( i ).phase == TouchPhase.Ended;
				fingerId = Input.GetTouch( i ).fingerId;
#endif

				// If a finger id has been stored, and this finger id is not the same as the stored finger id, then continue.
				if( TouchHoldInformations[ radialMenuIndex ].interactFingerID >= 0 && TouchHoldInformations[ radialMenuIndex ].interactFingerID != fingerId )
					continue;

				// Configure the menu radius for calculations.
				float menuRadius = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.sizeDelta.x / 2;

				// Store the touch position on the canvas. ( touch position / canvas scale factor ) - ( half the canvas size );
				Vector2 touchPositionOnCanvas = ( touchPosition / UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.scaleFactor ) - ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.GetComponent<RectTransform>().sizeDelta / 2 );

				// By subtracting the mouse position from the radial menu's position we get a relative number. Then we divide by the height of the screen space to give us an easier and more consistent number to work with.
				Vector2 modInput = ( touchPositionOnCanvas - ( Vector2 )UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition ) / menuRadius;

				// Configure the distance of the mouse position from the Radial Menu's base position.
				float dist = Vector2.Distance( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition, touchPositionOnCanvas );

				// If the radial menu is used in world space, then raycast the input.
				if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
					RaycastWorldSpaceRadialMenu( ref modInput, ref dist, touchPosition, radialMenuIndex );

				// If the input phase began, then store the finger id.
				if( touchBegan )
				{
					// If the input is within distance of the target, then store the finger id.
					if( enableMenuSetting == EnableMenuSetting.Manual || dist < menuRadius * activationRadius )
						TouchHoldInformations[ radialMenuIndex ].interactFingerID = fingerId;

					// If the radial menu is active...
					if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
					{
						// Configure a deactivation radius by using the radial menu maxRange.
						float deactivationRadius = TouchHoldInformations[ radialMenuIndex ].radialMenu.maxRange;

						// If the user wants the input radius to be infinite then set that here.
						if( TouchHoldInformations[ radialMenuIndex ].radialMenu.infiniteMaxRange )
							deactivationRadius = Mathf.Infinity;

						// Set the input to down since the touch began.
						inputDown = true;

						// If the distance is within the input range of the radial menu, then store the finger id.
						if( dist > UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange && dist < UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange )
							TouchHoldInformations[ radialMenuIndex ].interactFingerID = fingerId;
						// Else if the distance of the input is out of the deactivation range.
						else if( enableMenuSetting != EnableMenuSetting.Manual && ( dist > menuRadius * deactivationRadius || dist < TouchHoldInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange ) && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu && activationRadius > 0 )
							UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();
					}
				}

				// If the finger id is still not stored, then just continue to the next touch.
				if( TouchHoldInformations[ radialMenuIndex ].interactFingerID == -1 )
					continue;

				// If the radial menu is not active and not in the middle of a transition...
				if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.InTransition )
				{
					if( enableMenuSetting != EnableMenuSetting.Manual && dist < menuRadius * activationRadius )
					{
						// Increases the current hold time for calculations to enable.
						TouchHoldInformations[ radialMenuIndex ].currentHoldTime += Time.deltaTime;

						// If the current hold time has reached the target time...
						if( TouchHoldInformations[ radialMenuIndex ].currentHoldTime >= activationHoldTime )
						{
							// Reset the current hold time, and enable the menu.
							TouchHoldInformations[ radialMenuIndex ].currentHoldTime = 0.0f;
							UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.EnableRadialMenu();

							// Since the radial menu was enabled using text, set touchActivatedRadialMenu to true.
							TouchHoldInformations[ radialMenuIndex ].touchActivatedRadialMenu = true;

							// If the user wants to move to the touch position, move to that position now.
							if( dynamicPositioning && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
								UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.SetPosition( touchPositionOnCanvas, true );
						}
					}
				}
				else
				{
					// Set the current input device for reference.
					CurrentInputDevice = InputDevice.Touch;

					// Store the input to send to the radial menu.
					input = modInput;

					// Store the distance as well.
					distance = dist;
				}

				// If the touch has ended...
				if( touchEnded )
				{
					// Reset the finger id.
					TouchHoldInformations[ radialMenuIndex ].interactFingerID = -1;

					// Reset the current hold time.
					TouchHoldInformations[ radialMenuIndex ].currentHoldTime = 0.0f;

					// Set input down and up to true so that the interact function will be called on the button.
					inputDown = true;
					inputUp = true;

					// If the input is not over a button when released, then just disable the menu.
					if( enableMenuSetting != EnableMenuSetting.Manual && UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex < 0 && dist > menuRadius * activationRadius && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu && activationRadius > 0 )
						UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();
				}
			}
		}
		else
		{
			// If the touch information has not been reset...
			if( !touchInformationReset )
			{
				// Set touchInformationReset so that it will not be reset again until there are touches on the screen again.
				touchInformationReset = true;

				// Loop through each information and reset the values.
				for( int i = 0; i < TouchHoldInformations.Count; i++ )
				{
					TouchHoldInformations[ i ].currentHoldTime = 0.0f;
					TouchHoldInformations[ i ].interactFingerID = -1;
				}
			}
		}
	}

	/// <summary>
	/// This function will catch input from the center of the screen and VR device and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void CenterScreenInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// If the radial menu is not being used in world space, then just return.
		if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
			return;

		// Store the current input so that it can be checked after the raycast.
		Vector2 storedInput = input;

		// If the user wants to calculate with VR...
		if( virtualReality )
		{
			// If either of the eyes are unassigned, then inform the user and return.
			if( leftEyeCamera == null || rightEyeCamera == null )
			{
				Debug.LogError( $"Ultimate Radial Menu - The left or right eye cameras are unassigned. Please ensure that they are assigned in the inspector. If you need to update them at runtime, please use the SetCamerasVR() function." );
				return;
			}

			// Create a ray from the center position from the two eyes.
			Ray ray = new Ray( Vector3.Lerp( leftEyeCamera.transform.position, rightEyeCamera.transform.position, 0.5f ), leftEyeCamera.transform.forward );

			// Temporary hit variable to store hit information.
			RaycastHit hit;

			// Raycast with the calculated information.
			if( Physics.Raycast( ray, out hit, Mathf.Infinity, worldSpaceMask ) )
			{
				// If the collider that was hit is this radial menu...
				if( hit.collider.gameObject == UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.gameObject )
				{
					// Configure the local 3D Position of hit.
					Vector3 localHitPosition = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.InverseTransformPoint( hit.point );

					// Assign the input to being the local input value.
					input = localHitPosition;

					// Configure the distance of the input position from center.
					distance = Vector3.Distance( Vector2.zero, localHitPosition );
				}
			}
		}
		// Raycast from the center of the screen.
		else
			RaycastWorldSpaceRadialMenu( ref input, ref distance, new Vector3( Screen.width / 2, Screen.height / 2, 0 ), radialMenuIndex );

		// If the stored input is difference for the input that was just calculated...
		if( input != storedInput )
		{
			// Then set the current input device to Center Screen for reference.
			CurrentInputDevice = InputDevice.CenterScreen;

			// If the user wants to interact on hover, and the time is assigned...
			if( interactOnHover && interactHoverTime > 0 )
			{
				// If the calculated distance above is within range of the menu...
				if( distance >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange && distance <= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange )
				{
					// If the radial menu has a button index that is assigned, and it is different from the stored button index...
					if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex >= 0 && UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex != UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex )
					{
						// Store the current button index and reset the current hold time for reference.
						UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex;
						UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime = 0.0f;
					}

					// If the current button index is the same as the current button index on the radial menu...
					if( UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex == UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex )
					{
						// If the interact hold time still needs to be increased...
						if( UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime <= interactHoverTime )
						{
							// Increase the hold timer.
							UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime += Time.deltaTime;

							// If the interact timer is above our max hold time, set the input for interact.
							if( UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime >= interactHoverTime )
								inputDown = inputUp = true;
						}
					}
				}
				// Else the input is not within range of the menu...
				else
				{
					// If the current hold time had been calculated at all, reset it.
					if( UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime > 0 )
						UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime = 0.0f;

					// If the button index was stored, reset it also.
					if( UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex > 0 )
						UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex = -1;
				}
			}
		}
	}

	/// <summary>
	/// This function is a virtual void to allow for easy custom input logic.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void CustomInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// WARNING! This is not where you want to put your custom logic. Please check out our video tutorials for more information.
		// Video Tutorials: https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu
	}

	/// <summary>
	/// Send in custom raycast information to send to the Ultimate Radial Menus in the scene.
	/// </summary>
	/// <param name="rayStart">The start point of the ray.</param>
	/// <param name="rayEnd">The end point of the ray.</param>
	/// <param name="inputDown">The input down value to send in to the Ultimate Radial Menus.</param>
	/// <param name="inputUp">The input up value to send to the Ultimate Radial Menus.</param>
	public void SendRaycastInput ( Vector3 rayStart, Vector3 rayEnd, bool inputDown, bool inputUp )
	{
		SendRaycastInput( rayStart, ( rayEnd - rayStart ).normalized, Vector3.Distance( rayStart, rayEnd ), inputDown, inputUp );
	}

	/// <summary>
	/// Send in custom raycast information to send to the Ultimate Radial Menus in the scene.
	/// </summary>
	/// <param name="rayOrigin">The origin of the ray.</param>
	/// <param name="rayDirection">The direction of the ray.</param>
	/// <param name="rayDistance">The distance of the ray.</param>
	/// <param name="inputDown">The input down value to send in to the Ultimate Radial Menus.</param>
	/// <param name="inputUp">The input up value to send to the Ultimate Radial Menus.</param>
	public void SendRaycastInput ( Vector3 rayOrigin, Vector3 rayDirection, float rayDistance, bool inputDown, bool inputUp )
	{
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			// If the current radial menu is not used in world space, then just return.
			if( !UltimateRadialMenuInformations[ i ].radialMenu.IsWorldSpaceRadialMenu )
				return;

			// If the main camera is null, not active, or the camera component is not enabled, then update the camera reference.
			if( mainCamera == null || !mainCamera.gameObject.activeInHierarchy || !mainCamera.enabled )
				UpdateCamera();

			// Create a ray with the information provided.
			Ray ray = new Ray( rayOrigin, rayDirection );

			// Temporary hit variable to store hit information.
			RaycastHit hit;
			Vector2 input = Vector2.zero;
			float distance = 0.0f;

			// Raycast with the calculated information.
			if( Physics.Raycast( ray, out hit, rayDistance, worldSpaceMask ) )
			{
				// If the collider that was hit is this radial menu...
				if( hit.collider.gameObject == UltimateRadialMenuInformations[ i ].radialMenu.gameObject )
				{
					// Configure the local 3D Position of hit.
					Vector3 localHitPosition = UltimateRadialMenuInformations[ i ].radialMenu.BaseTransform.InverseTransformPoint( hit.point );

					// Assign the input to being the local input value.
					input = localHitPosition;

					// Configure the distance of the input position from center.
					distance = Vector3.Distance( Vector2.zero, localHitPosition );

					// If the distance value is within range of the radial menu...
					if( distance >= UltimateRadialMenuInformations[ i ].radialMenu.CalculatedMinRange && distance <= UltimateRadialMenuInformations[ i ].radialMenu.CalculatedMaxRange )
					{
						// Then assign the custom input data.
						UltimateRadialMenuInformations[ i ].customInputThisFrame = true;
						UltimateRadialMenuInformations[ i ].customInput = input;
						UltimateRadialMenuInformations[ i ].customDistance = distance;
						UltimateRadialMenuInformations[ i ].customInputDown = inputDown;
						UltimateRadialMenuInformations[ i ].customInputUp = inputUp;

						// Set the input device as other.
						CurrentInputDevice = InputDevice.Other;
					}
				}
			}
		}
	}

	/// <summary>
	/// Sends the custom controller input value to this input manager so that it can send it to all the radial menus in the scene.
	/// </summary>
	/// <param name="input">The input value of the controller joystick.</param>
	public void SendControllerInput ( Vector2 input )
	{
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			// If the menu is null, then it must have been deleted...
			if( UltimateRadialMenuInformations[ i ].radialMenu == null )
			{
				// Update the list and break the loop to avoid errors.
				UltimateRadialMenuInformations.RemoveAt( i );

				// If the user has touch input enabled, then remove this index from the list.
				if( touchInput )
					TouchHoldInformations.RemoveAt( i );

				break;
			}

			// If the controller input is not zero, set the current input device for reference.
			if( input != Vector2.zero )
			{
				CurrentInputDevice = InputDevice.Controller;

				// Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
				float distance = Vector2.Distance( Vector2.zero, input );

				// If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
				if( distance >= UltimateRadialMenuInformations[ i ].radialMenu.minRange )
					distance = Mathf.Lerp( UltimateRadialMenuInformations[ i ].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[ i ].radialMenu.CalculatedMaxRange, 0.5f );

				UltimateRadialMenuInformations[ i ].customInputThisFrame = true;
				UltimateRadialMenuInformations[ i ].customInput = input;
				UltimateRadialMenuInformations[ i ].customDistance = distance;
			}
		}
	}
	
	/// <summary>
	/// Sends the custom controller input down value to this input manager so that it can send it to all the radial menus in the scene.
	/// </summary>
	/// <param name="inputDown">The input down value coming from the controller.</param>
	public void SendControllerInputDown ( bool inputDown )
	{
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			UltimateRadialMenuInformations[ i ].customInputThisFrame = true;
			UltimateRadialMenuInformations[ i ].customInputDown = inputDown;
		}
	}

	/// <summary>
	/// Sends the custom controller input up value to this input manager so that it can send it to all the radial menus in the scene.
	/// </summary>
	/// <param name="inputUp">The input up value coming from the controller.</param>
	public void SendControllerInputUp ( bool inputUp )
	{
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			UltimateRadialMenuInformations[ i ].customInputThisFrame = true;
			UltimateRadialMenuInformations[ i ].customInputUp = inputUp;
		}
	}

	/// <summary>
	/// Reset the custom controller input.
	/// </summary>
	public void ResetCustomInput ()
	{
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			UltimateRadialMenuInformations[ i ].customInputThisFrame = false;
			UltimateRadialMenuInformations[ i ].customInput = Vector2.zero;
			UltimateRadialMenuInformations[ i ].customDistance = 0.0f;
			UltimateRadialMenuInformations[ i ].customInputDown = false;
			UltimateRadialMenuInformations[ i ].customInputUp = false;
		}
	}
}