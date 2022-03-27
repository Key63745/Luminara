using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace UltimateRadialMenuExample.CharacterInventory2D
{
	[RequireComponent( typeof( Rigidbody2D ) )]
	public class PlayerController : MonoBehaviour
	{
		public float speed = 10;
		Rigidbody2D myRigidbody;
		SpriteRenderer mySpriteRenderer;

		
		void Start ()
		{
			myRigidbody = GetComponent<Rigidbody2D>();
			mySpriteRenderer = GetComponent<SpriteRenderer>();
		}
		
		void FixedUpdate ()
		{
			Vector2 movement = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
			// Store the keyboard from the input system.
			Keyboard keyboard = InputSystem.GetDevice<Keyboard>();

			movement.x = keyboard.aKey.isPressed ? -1 : keyboard.dKey.isPressed ? 1 : 0;
			movement.y = keyboard.sKey.isPressed ? -1 : keyboard.wKey.isPressed ? 1 : 0;
#else
			//Store the current horizontal input in the float moveHorizontal.
			movement = new Vector2( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ) );
#endif
			// If the horizontal input is not zero, then flip the x of the sprite based on the horizontal input.
			if( Mathf.Abs( movement.x ) > 0 )
				mySpriteRenderer.flipX = Mathf.Sign( movement.x ) == -1;
	
			// Store the position that the character is in into view port coordinates.
			Vector3 pos = Camera.main.WorldToViewportPoint( myRigidbody.position + ( movement * speed ) );

			// Then clamp the position to not allow the character to leave the screen bounds.
			pos.x = Mathf.Clamp( pos.x, 0.05f, 0.95f );
			pos.y = Mathf.Clamp( pos.y, 0.1f, 0.9f );

			// Reconfigure the position into world position and apply it to the rigidbody.
			myRigidbody.MovePosition( Camera.main.ViewportToWorldPoint( pos ) );
		}
	}
}