/* RadialMenuLookAtInput.cs */
/* Written by Kaz */
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class RadialMenuLookAtInput : MonoBehaviour
{
	UltimateRadialMenuInputManager inputManager;
	public Transform cameraTransform;

	public bool invert = false;
	public float distanceModifier = 7.5f;


	private void Start ()
	{
		// If this gameObject does not have a Ultimate Radial Menu component...
		if( !GetComponent<UltimateRadialMenu>() )
		{
			// Send a log to the user, disable this component to avoid errors, and return.
			Debug.LogError( "This component is not attached to an Ultimate Radial Menu gameObject. Disabling this component to avoid errors." );
			enabled = false;
			return;
		}

		// If this gameObject has a unique Input Manager, then assign the inputManager to the one found on this gameObject.
		if( GetComponent<UltimateRadialMenuInputManager>() )
			inputManager = GetComponent<UltimateRadialMenuInputManager>();
		// Else use the global input manager.
		else
			inputManager = UltimateRadialMenuInputManager.Instance;
	}

	void Update ()
	{
		// Vector variable for the input value.
		Vector2 inputValue = Vector2.zero;

		// If the input manager has the keyboard input enabled, and the mouse is present...
		if( inputManager.keyboardInput )
		{
#if ENABLE_INPUT_SYSTEM
			// Store the mouse from the input system.
			Mouse mouse = InputSystem.GetDevice<Mouse>();

			// If the mouse could be stored from the input system, then catch the input value.
			if( mouse != null )
			{
				inputValue = mouse.position.ReadValue();
#else
			if( Input.mousePresent )
			{
				// Store the mouse position.
				inputValue = Input.mousePosition;
#endif
				// Recalculate so that the center of the screen is the new zero/zero.
				inputValue -= ( new Vector2( Screen.width, Screen.height ) / 2 );

				// Divide the input by the screen size so that it is a more manageable value.
				inputValue /= ( ( Screen.width > Screen.height ? Screen.width : Screen.height ) / 2 );
			}
		}

		// If the input manager has the controller input enabled...
		if( inputManager.controllerInput )
		{
			// Catch the controllers axis.
			Vector2 controllerInput = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
			// Store the gamepad from the input system.
			Gamepad gamepad = InputSystem.GetDevice<Gamepad>();

			// If the gamepad is assigned, then catch the input.
			if( gamepad != null )
				controllerInput = inputManager.joystick == UltimateRadialMenuInputManager.Joysticks.Left ? gamepad.leftStick.ReadValue() : gamepad.rightStick.ReadValue();
#else
			controllerInput = new Vector2( Input.GetAxis( inputManager.horizontalAxisController ), Input.GetAxis( inputManager.verticalAxisController ) );
#endif
			// If the value is not zero, then assign the input.
			if( controllerInput != Vector2.zero )
				inputValue = controllerInput;
		}

		// Configure where the input places the LookAt point according to the camera transform.
		Vector3 worldLookAtPosition = cameraTransform.TransformPoint( new Vector3( ( inputValue.x * ( invert ? 1 : -1 ) ) * distanceModifier, ( inputValue.y * ( invert ? 1 : -1 ) * distanceModifier ), 25 ) );
		
		// Force this transform to look at the world position.
		transform.LookAt( worldLookAtPosition );

		// Zero out the local z rotation so that it doesn't get all wobbly.
		transform.localEulerAngles = new Vector3( transform.localEulerAngles.x, transform.localEulerAngles.y, 0 );
	}
}