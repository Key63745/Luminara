/* UltimateRadialMenuPointer.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu( "Ultimate Radial Menu/Pointer" )]
public class UltimateRadialMenuPointer : MonoBehaviour
{
	public UltimateRadialMenu radialMenu;
	public RectTransform pointerTransform;
	public Image pointerImage;

	// POINTER POSITIONING //
	public float pointerSize = 0.25f, targetingSpeed = 5.0f;
	public enum SnappingOption
	{
		Instant,
		Smooth,
		Free
	}
	public SnappingOption snappingOption = SnappingOption.Smooth;
	Quaternion targetRotation;
	public float rotationOffset = 90;

	// VISUAL OPTIONS //
	public bool colorChange = false, changeOverTime = false;
	public float fadeInDuration = 0.25f, fadeOutDuration = 0.5f;
	public Color normalColor = Color.white, activeColor = Color.white;
	bool radialMenuFocused = false;

	public bool usePointerStyle = false;
	[System.Serializable]
	public class PointerStyle
	{
		public int buttonCount;
		public Sprite pointerSprite;
	}
	public List<PointerStyle> PointerStyles = new List<PointerStyle>();

	public enum SetSiblingIndex
	{
		Disabled,
		First,
		Last
	}
	public SetSiblingIndex setSiblingIndex = SetSiblingIndex.Disabled;


	void Awake ()
	{
		// If the game is not running, then return.
		if( !Application.isPlaying )
			return;

		// If the radial menu is null...
		if( radialMenu == null )
		{
			// Attempt to find it in the parent gameobject.
			radialMenu = GetComponentInParent<UltimateRadialMenu>();

			// If the menu is still null, then send an error to the console and disable this component.
			if( radialMenu == null )
			{
				Debug.LogError( "Ultimate Radial Menu Pointer\nThere is not a Ultimate Radial Menu assigned to this pointer. This component was not able to find a Ultimate Radial Menu in any parent objects either. Disabling this component to avoid errors." );
				enabled = false;
			}
		}
	}

	void Start ()
	{
		// If the game is not running, then return.
		if( !Application.isPlaying )
			return;

		// Subscribe to the needed events of the radial menu.
		radialMenu.OnUpdatePositioning += OnUpdatePositioning;
		radialMenu.OnRadialButtonEnter += OnRadialButtonEnter;
		radialMenu.OnRadialMenuLostFocus += OnRadialMenuLostFocus;
		radialMenu.OnRadialMenuDisabled += OnRadialMenuDisabled;
		radialMenu.OnRadialMenuButtonCountModified += OnRadialMenuButtonCountModified;

		// Attempt to assign it.
		radialMenu = GetComponentInParent<UltimateRadialMenu>();

		// If it is still null, return to avoid errors.
		if( radialMenu == null )
		{
			Debug.LogError( "Ultimate Radial Menu Pointer\nThis component is not placed within an Ultimate Radial Menu. Disabling this component to avoid errors." );
			enabled = false;
			return;
		}

		// Attempt to find the pointer rect transform.
		pointerTransform = GetComponent<RectTransform>();

		// If the pointer is still null, then warn the user and disable this component to avoid errors.
		if( pointerTransform == null )
		{
			Debug.LogError( "Ultimate Radial Menu Pointer\nThis gameObject does not have an RectTransform component. Disabling this component to avoid errors." );
			enabled = false;
			return;
		}

		pointerImage = GetComponent<Image>();

		// If the image is still null, then warn the user and disable this component to avoid errors.
		if( pointerImage == null )
		{
			Debug.LogError( "Ultimate Radial Menu Pointer\nThis gameObject does not have an Image component. Disabling this component to avoid errors." );
			enabled = false;
			return;
		}

		// If the user wants to change colors, then apply the normal color here.
		if( colorChange && pointerImage != null )
			pointerImage.color = normalColor;

		// Configure the style of the pointer according to the count of the radial buttons.
		OnRadialMenuButtonCountModified( radialMenu.UltimateRadialButtonList.Count );
	}

	/// <summary>
	/// This function will be called when the radial menu loses focus.
	/// </summary>
	void OnRadialMenuLostFocus ()
	{
		// If the button pointer is currently on...
		if( radialMenuFocused )
		{
			// If the user wants to change the color, but not over time, then apply the normal color.
			if( colorChange && pointerImage != null && !changeOverTime )
				pointerImage.color = normalColor;
		}

		// Set radial menu focused to false so that the other functions know it is not longer in focus.
		radialMenuFocused = false;
	}

	/// <summary>
	/// This function will be called when the radial menu enters a new button.
	/// </summary>
	/// <param name="index">The index of the new button found.</param>
	void OnRadialButtonEnter ( int index )
	{
		// If the radial pointer is assigned...
		if( pointerTransform != null )
		{
			// Store the target rotation to being the angle of the radial button at the index.
			targetRotation = Quaternion.Euler( 0, 0, radialMenu.UltimateRadialButtonList[ index ].angle - rotationOffset );

			// If the radial menu has not been focused yet, or the user wants to apply the rotation instantly, then simply apply the rotation.
			if( !radialMenuFocused || snappingOption == SnappingOption.Instant )
				pointerTransform.localRotation = targetRotation;
		}

		// If the pointer is not currently on...
		if( !radialMenuFocused )
		{
			// If the user wants to change color...
			if( colorChange && pointerImage != null )
			{
				// If the user wants to change color over time then start the coroutine.
				if( changeOverTime )
					StartCoroutine( UpdateColor() );
				// Else just apply the active color.
				else
					pointerImage.color = activeColor;
			}
		}

		// Set radialMenuFocused as true so that other functions know that the pointer has already been focused.
		radialMenuFocused = true;
	}

	/// <summary>
	/// This function will be called when the radial menu gets disabled.
	/// </summary>
	void OnRadialMenuDisabled ()
	{
		// If the button pointer is current on...
		if( radialMenuFocused )
		{
			// If the user wants to change the color, but not over time, then apply the normal color.
			if( colorChange && pointerImage != null && !changeOverTime )
				pointerImage.color = normalColor;
		}

		// Set radial menu focused to false so that the other functions know it is not longer in focus.
		radialMenuFocused = false;
	}

	/// <summary>
	/// This function is called when the radial menu button count is modified.
	/// </summary>
	/// <param name="buttonCount">This parameter is the new button count after the modification.</param>
	void OnRadialMenuButtonCountModified ( int buttonCount )
	{
		// If the user is not using a style, then return.
		if( !usePointerStyle )
			return;

		// Loop through each style and check for the corresponding button count.
		for( int i = 0; i < PointerStyles.Count; i++ )
		{
			// If this button count is lower than the pointer style button count...
			if( buttonCount <= PointerStyles[ i ].buttonCount )
			{
				// Assign this sprite and break the loop.
				pointerImage.sprite = PointerStyles[ i ].pointerSprite;
				break;
			}
		}

		// If the user wants the sibling index set...
		if( setSiblingIndex != SetSiblingIndex.Disabled )
		{
			// If the user wants this pointer as the last sibling, then set that.
			if( setSiblingIndex == SetSiblingIndex.Last )
				transform.SetAsLastSibling();
			// Else set this transform as the first sibling.
			else if( setSiblingIndex == SetSiblingIndex.First && transform.GetSiblingIndex() > 0 )
				transform.SetAsFirstSibling();
		}
	}

	/// <summary>
	/// This function will be called with the radial menu updates it's size and positioning.
	/// </summary>
	void OnUpdatePositioning ()
	{
		// If the radial pointer is now assigned...
		if( pointerTransform != null )
		{
			// Store a new pointer size based off of the radial menu's size multiplied by the pointerSize option set by the user.
			float _pointerSize = radialMenu.GetComponent<RectTransform>().sizeDelta.x * pointerSize;

			// Apply the size position to the pointer transform.
			pointerTransform.sizeDelta = new Vector2( _pointerSize, _pointerSize );
			pointerTransform.position = radialMenu.BasePosition;
			
			// If the game is not running, then apply the rotation to look at the first button plus the rotation offset that the user has set.
			if( !Application.isPlaying && radialMenu.UltimateRadialButtonList.Count > 0 )
				pointerTransform.localRotation = Quaternion.Euler( 0, 0, radialMenu.UltimateRadialButtonList[ 0 ].angle - rotationOffset );
		}
	}

	void Update ()
	{
		// If the game is not running, then simply updated the positioning and return.
		if( !Application.isPlaying )
		{
			OnUpdatePositioning();
			return;
		}

		// If the user wants to apply the rotation instantly( which has already been done in the OnRadialMenuItemFound() function ) or the pointer transform is null, then return.
		if( snappingOption == SnappingOption.Instant || pointerTransform == null )
			return;

		// If the snapping option is set to free, then transition the rotation to the current angle of the input.
		if( snappingOption == SnappingOption.Free )
			pointerTransform.localRotation = Quaternion.Slerp( pointerTransform.localRotation, Quaternion.Euler( 0, 0, radialMenu.GetCurrentInputAngle - rotationOffset ), Time.unscaledDeltaTime * targetingSpeed );
		// Else transition the rotation to the target rotation of the currently selected button.
		else
			pointerTransform.localRotation = Quaternion.Slerp( pointerTransform.localRotation, targetRotation, Time.unscaledDeltaTime * targetingSpeed );
	}
	
	/// <summary>
	/// Changes the color of the image over time.
	/// </summary>
	IEnumerator UpdateColor ()
	{
		// Set the radial menu focused as true so that this coroutine will at least run the first frame.
		radialMenuFocused = true;

		// Store a temporary float for the speed of the fade in transition.
		float fadeInSpeed = 1.0f / fadeInDuration;
		for( float t = 0.0f; t < 1.0f && radialMenuFocused; t += Time.unscaledDeltaTime * fadeInSpeed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( fadeInSpeed ) )
				break;

			// Transition the color from normal to active by t.
			pointerImage.color = Color.Lerp( normalColor, activeColor, t );
			yield return null;
		}

		// If the pointer is still focused then apply the final color.
		if( radialMenuFocused )
			pointerImage.color = activeColor;

		// While the radial menu is focused, wait here.
		while( radialMenuFocused )
			yield return null;

		// Configure the fade out speed.
		float fadeOutSpeed = 1.0f / fadeOutDuration;
		for( float t = 0.0f; t < 1.0f && !radialMenuFocused; t += Time.unscaledDeltaTime * fadeOutSpeed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( fadeOutDuration ) )
				break;

			// Transition the color from active to normal by t.
			pointerImage.color = Color.Lerp( activeColor, normalColor, t );
			yield return null;
		}

		// If the radial menu is still not focused then apply the normal.
		if( !radialMenuFocused )
			pointerImage.color = normalColor;
	}
}