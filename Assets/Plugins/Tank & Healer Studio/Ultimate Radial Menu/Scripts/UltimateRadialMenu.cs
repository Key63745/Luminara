/* UltimateRadialMenu.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent( typeof( CanvasGroup ) )]
[AddComponentMenu( "Ultimate Radial Menu/Ultimate Radial Menu" )]
public class UltimateRadialMenu : MonoBehaviour
{
	// INTERNAL CALCULATIONS //
	/// <summary>
	/// The parent canvas component that this radial menu is a child of.
	/// </summary>
	public UnityEngine.Canvas ParentCanvas
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns if this is a World Space radial menu or not.
	/// </summary>
	public bool IsWorldSpaceRadialMenu
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the angle of each button, determined by the menu button count.
	/// </summary>
	public float GetAnglePerButton
	{
		get
		{
			return 360f / menuButtonCount;
		}
	}
	/// <summary>
	/// Returns the current input angle so that other scripts can access the current input of this radial menu.
	/// </summary>
	public float GetCurrentInputAngle
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the calculated minimum range of the radial menu.
	/// </summary>
	public float CalculatedMinRange
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the calculated maximum range of the radial menu.
	/// </summary>
	public float CalculatedMaxRange
	{
		get;
		private set;
	}
	/// <summary>
	/// The base RectTransform component.
	/// </summary>
	public RectTransform BaseTransform
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the position of the base transform.
	/// </summary>
	public Vector3 BasePosition
	{
		get
		{
			if( BaseTransform == null )
				return Vector3.zero;

			return BaseTransform.position;
		}
	}
	/// <summary>
	/// Returns the index of the button currently being interacted with.
	/// </summary>
	public int CurrentButtonIndex
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the stored current selected button index.
	/// </summary>
	public int CurrentSelectedButtonIndex
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the current state of the radial menu.
	/// </summary>
	public bool RadialMenuActive
	{
		get;
		private set;
	}
	/// <summary>
	/// Returns the current state of the input on this radial menu.
	/// </summary>
	public bool InputInRange
	{
		get;
		private set;
	}
	/// <summary>
	/// The current state of being able to interact with the radial menu.
	/// </summary>
	public bool Interactable
	{
		get;
		set;
	}
	/// <summary>
	/// Returns the state of transitioning the radial menu toggle.
	/// </summary>
	public bool InTransition
	{
		get;
		private set;
	}
	bool inputInRangeLastFrame = false;
	int buttonIndexOnInputDown = -1;
	Vector3 defaultPosition;
	RectTransform canvasRectTrans;
	Vector2 parentCanvasSize;

	// RADIAL MENU POSITIONING //
	public int menuButtonCount = 4;
	public enum ScalingAxis
	{
		Width,
		Height
	}
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public float menuSize = 5.0f;
	public float horizontalPosition = 50.0f, verticalPosition = 50.0f;
	public float depthPosition = 0.0f;
	public float menuButtonSize = 0.25f;
	public float radialMenuButtonRadius = 1.0f;
	public float startingAngle = 0.0f;
	public enum AngleOffset
	{
		Centered,
		OffCenter,
		OnlyEven,
		OnlyOdd,
	}
	public AngleOffset angleOffset = AngleOffset.Centered;
	public bool followOrbitalRotation = true;
	public float minRange = 0.25f, maxRange = 1.5f;
	public bool infiniteMaxRange = false;
	public float buttonInputAngle = 0.0f;

	// RADIAL MENU SETTINGS //
	public UltimateRadialMenuStyle radialMenuStyle;
	int currentStyleIndex;
	public Sprite normalSprite;
	public Color normalColor = Color.white;
	// Menu Toggle //
	public enum InitialState
	{
		Enabled,
		Disabled
	}
	public InitialState initialState = InitialState.Enabled;
	public enum RadialMenuToggle
	{
		FadeAlpha,
		Scale
	}
	public RadialMenuToggle radialMenuToggle = RadialMenuToggle.FadeAlpha;
	public float toggleInDuration = 0.25f, toggleOutDuration = 0.25f;
	CanvasGroup canvasGroup;
	// Menu Text //
	public bool displayButtonName = false;
	public Text nameText;
	public float nameTextRatioX = 1.0f, nameTextRatioY = 1.0f, nameTextSize = 0.25f;
	public float nameTextHorizontalPosition = 50.0f, nameTextVerticalPosition = 50.0f;
	public bool displayButtonDescription = false;
	public Text descriptionText;
	public float descriptionTextRatioX = 1.0f, descriptionTextRatioY = 1.0f, descriptionTextSize = 0.25f;
	public float descriptionTextHorizontalPosition = 50.0f, descriptionTextVerticalPosition = 50.0f;
	// Button Icon //
	public bool useButtonIcon = false;
	public float iconSize = 0.25f, iconRotation = 0.0f;
	public float iconHorizontalPosition = 50.0f, iconVerticalPosition = 50.0f;
	public bool iconLocalRotation = false;
	public Color iconNormalColor = Color.white;
	// Button Text //
	public bool useButtonText = false;
	public float textAreaRatioX = 1.0f, textAreaRatioY = 0.25f, textSize = 0.25f;
	public float textHorizontalPosition = 50.0f, textVerticalPosition = 50.0f;
	public bool displayNameOnButton = true;
	public bool textLocalPosition = true;
	public bool textLocalRotation = true;
	public Color textNormalColor = Color.white;
	public bool relativeToIcon = false;
	// Text Settings //
	public Font nameFont;
	public Font descriptionFont;
	public Font buttonTextFont;
	public bool nameOutline = false;
	public bool descriptionOutline = false;
	public bool buttonTextOutline = false;
	public Color buttonTextOutlineColor = Color.white;

	// BUTTON INTERACTION //
	public bool spriteSwap = false;
	public bool colorChange = true;
	public bool scaleTransform = false;
	public bool iconColorChange = false;
	public bool iconScaleTransform = false;
	public bool textColorChange = false;
	// Highlighted //
	public Sprite highlightedSprite;
	public Color highlightedColor = Color.white;
	public float highlightedScaleModifier = 1.1f;
	public float positionModifier = 0.0f;
	public Color iconHighlightedColor = Color.white;
	public float iconHighlightedScaleModifier = 1.1f;
	public Color textHighlightedColor = Color.white;
	// Pressed //
	public Sprite pressedSprite;
	public Color pressedColor = Color.white;
	public float pressedScaleModifier = 1.05f;
	public float pressedPositionModifier = 0.0f;
	public Color iconPressedColor = Color.white;
	public float iconPressedScaleModifier = 1.0f;
	public Color textPressedColor = Color.white;
	// Selected //
	public bool selectButtonOnInteract = false;
	public Sprite selectedSprite;
	public Color selectedColor = Color.white;
	public float selectedScaleModifier = 1.0f;
	public float selectedPositionModifier = 0.0f;
	public Color iconSelectedColor = Color.white;
	public float iconSelectedScaleModifier = 1.0f;
	public Color textSelectedColor = Color.white;
	// Disabled //
	public Sprite disabledSprite;
	public Color disabledColor = Color.white;
	public float disabledScaleModifier = 1.0f;
	public float disabledPositionModifier = 0.0f;
	public Color iconDisabledColor = Color.white;
	public float iconDisabledScaleModifier = 1.0f;
	public Color textDisabledColor = Color.white;
	
	// RADIAL BUTTON LIST //
	[Serializable]
	public class UltimateRadialButton
	{
		/// <summary>
		/// Returns the state of having information registered to this button.
		/// </summary>
		public bool Registered
		{
			get
			{
				if( registered )
					return true;

				if( unityEvent != null && unityEvent.GetPersistentEventCount() > 0 )
					return true;

				return false;
			}
		}
		bool registered = false;
		/// <summary>
		/// Returns the current state of being selected.
		/// </summary>
		public bool Selected
		{
			get
			{
				if( radialMenu == null )
					return false;

				if( radialMenu.CurrentSelectedButtonIndex >= 0 && radialMenu.CurrentSelectedButtonIndex == buttonIndex )
					return true;

				return false;
			}
		}

		// BASIC VARIABLES //
		public UltimateRadialMenu radialMenu;
		public RectTransform buttonTransform;
		public Image radialImage;
		public bool buttonDisabled = false;
		public string name;
		public string description;
		public int buttonIndex = -1;

		// INPUT VARIABLES //
		public float angle;
		public float angleRange;

		// ICON SETTINGS //
		public RectTransform iconTransform;
		public Image icon;
		public bool useIconUnique = false;
		public float iconSize = 0.0f;
		public float iconHorizontalPosition = 0.0f, iconVerticalPosition = 0.0f;
		public float iconRotation = 0.0f;
		public bool invertScaleY = false;
		public Vector3 iconNormalScale;

		// TEXT SETTINGS //
		public Text text;

		// TRANSFORM INTERACTION SETTINGS //
		public Vector3 normalPosition, highlightedPosition, pressedPosition, selectedPosition, disabledPosition;

		// BASIC CALLBACK INFORMATION //
		public string key;
		public int id;

		// CALLBACKS //
		public event Action OnRadialButtonInteract;
		public event Action<int> OnRadialButtonInteractWithId;
		public event Action<string> OnRadialButtonInteractWithKey;
		public event Action OnClearButtonInformation;
		public UnityEvent unityEvent;

		
		/// <summary>
		/// Returns true if the angle is within the range of this button.
		/// </summary>
		/// <param name="inputAngle">The current input angle.</param>
		public bool IsInAngle ( float inputAngle )
		{	
			// If the angle is within this buttons range, then return true.
			if( Mathf.Abs( inputAngle - angle ) <= angleRange || Mathf.Abs( ( inputAngle - 360f ) - angle ) <= angleRange || Mathf.Abs( inputAngle - ( angle - 360f ) ) <= angleRange )
				return true;

			// Else return false.
			return false;
		}

		/// <summary>
		/// Invokes the functionality for when the input enters the button.
		/// </summary>
		public void OnEnter ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;

			// If the current selected index is not assigned, or it is different than this button index...
			if( radialMenu.CurrentSelectedButtonIndex < 0 || radialMenu.CurrentSelectedButtonIndex != buttonIndex )
			{
				// If the user want to swap sprites when the radial button is highlighted...
				if( radialMenu.spriteSwap )
				{
					// If the highlighted sprite is assigned, then apply that sprite to the image.
					if( radialMenu.highlightedSprite != null )
						radialImage.sprite = radialMenu.highlightedSprite;
					// Else the highlighted sprite is null, so apply the normal sprite to the image.
					else
						radialImage.sprite = radialMenu.normalSprite;
				}

				// If the user wants to change the color of the radial button, then apply the highlighted color.
				if( radialMenu.colorChange && radialImage.sprite != null )
					radialImage.color = radialMenu.highlightedColor;

				// If the user wants to scale the transform when the radial button is being hovered over...
				if( radialMenu.scaleTransform )
				{
					// Modify the scale and position of the radial button.
					buttonTransform.localScale = Vector3.one * radialMenu.highlightedScaleModifier;
					buttonTransform.localPosition = highlightedPosition;
				}

				// If the user wants to use the icon and it is assigned...
				if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
				{
					// If the user wants to change color, then apply the highlighted color.
					if( radialMenu.iconColorChange )
						icon.color = radialMenu.iconHighlightedColor;

					// If the user wants to scale the transform of the icon, then apply the highlighted scale.
					if( radialMenu.iconScaleTransform )
						iconTransform.localScale = iconNormalScale * radialMenu.iconHighlightedScaleModifier;
				}

				// If the user wants to use text and it is assigned...
				if( radialMenu.useButtonText && text != null )
				{
					// If the user wants to change color, then apply the highlighted color.
					if( radialMenu.textColorChange )
						text.color = radialMenu.textHighlightedColor;
				}
			}
		}

		/// <summary>
		/// Invokes the functionality for when the input exits the button.
		/// </summary>
		public void OnExit ()
		{
			// If this button is disabled, then return...
			if( buttonDisabled )
				return;

			// If the application is not playing OR the current selected index is not this button...
			if( !Application.isPlaying || radialMenu.CurrentSelectedButtonIndex != buttonIndex )
			{
				// If the user wants to swap sprites, then apply the normal sprite to the image.
				if( radialMenu.spriteSwap && radialMenu.normalSprite != null )
					radialImage.sprite = radialMenu.normalSprite;

				// If the user wants to change the color of the image, then apply the normal color.
				if( radialMenu.colorChange && radialImage.sprite != null )
					radialImage.color = radialMenu.normalColor;

				// If the user wants to scale the transform, then apply the normal scale and position.
				if( radialMenu.scaleTransform )
				{
					radialImage.GetComponent<RectTransform>().localScale = Vector3.one;
					radialImage.GetComponent<RectTransform>().localPosition = normalPosition;
				}

				// If the user wants to use the icon and it is assigned...
				if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
				{
					// If the user wants to change color, then apply the normal color.
					if( radialMenu.iconColorChange )
						icon.color = radialMenu.iconNormalColor;

					// If the user wants to scale the transform of the icon, then apply the normal scale.
					if( radialMenu.iconScaleTransform )
						iconTransform.localScale = iconNormalScale;
				}

				// If the user wants to use text and it is assigned...
				if( radialMenu.useButtonText && text != null )
				{
					// If the user wants to change color, then apply the normal color.
					if( radialMenu.textColorChange )
						text.color = radialMenu.textNormalColor;
				}
			}
		}

		/// <summary>
		/// Invokes the functionality for when the input interacts with the button.
		/// </summary>
		public void OnInteract ()
		{
			// If this button is disabled, then return.
			if( buttonDisabled )
				return;
			
			// If the user wants to select the button when interacted with, do that here.
			if( radialMenu.selectButtonOnInteract && radialMenu.CurrentSelectedButtonIndex != buttonIndex )
				OnSelect();

			// If the user has assigned a unity event to call, then call it.
			if( unityEvent != null )
				unityEvent.Invoke();

			// If the user has subscribed to the default callback then invoke it.
			if( OnRadialButtonInteract != null )
				OnRadialButtonInteract();

			// If the user has subscribed to the ID callback then invoke it with the assigned integer ID.
			else if( OnRadialButtonInteractWithId != null )
				OnRadialButtonInteractWithId( id );

			// If the user has subscribed to the string key callback then invoke it with the assigned key.
			else if( OnRadialButtonInteractWithKey != null )
				OnRadialButtonInteractWithKey( key );
			
			// Inform any subscribers that this button has been interacted with.
			if( radialMenu.OnRadialButtonInteract != null )
				radialMenu.OnRadialButtonInteract( buttonIndex );
		}

		/// <summary>
		/// Invokes the functionality for when this button is selected.
		/// </summary>
		public void OnSelect ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;
			
			// If the current selected button index is assigned and it is different than this button...
			if( radialMenu.CurrentSelectedButtonIndex >= 0 && radialMenu.CurrentSelectedButtonIndex != buttonIndex )
				radialMenu.UltimateRadialButtonList[ radialMenu.CurrentSelectedButtonIndex ].OnDeselect();

			// Set the current selected button index to this index.
			radialMenu.CurrentSelectedButtonIndex = buttonIndex;
			
			// If the user wants to swap sprites, then apply the selected sprite to the image.
			if( radialMenu.spriteSwap && radialMenu.selectedSprite != null )
				radialImage.sprite = radialMenu.selectedSprite;

			// If the user wants to change the color of the radial button, then apply the selected color.
			if( radialMenu.colorChange && radialImage.sprite != null )
				radialImage.color = radialMenu.selectedColor;

			// If the user wants to scale the transform, then apply the scale and position.
			if( radialMenu.scaleTransform )
			{
				buttonTransform.localScale = Vector3.one * radialMenu.selectedScaleModifier;
				buttonTransform.localPosition = selectedPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
			{
				// If the user wants to change color, then apply the selected color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconSelectedColor;

				// If the user wants to scale the transform of the icon...
				if( radialMenu.iconScaleTransform )
					iconTransform.localScale = iconNormalScale * radialMenu.iconSelectedScaleModifier;
			}

			// If the user wants to use text and it is assigned...
			if( radialMenu.useButtonText && text != null )
			{
				// If the user wants to change color, then apply the selected color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textSelectedColor;
			}

			// Inform any subscribers that this button has been selected.
			if( radialMenu.OnRadialButtonSelected != null )
				radialMenu.OnRadialButtonSelected( buttonIndex );
		}

		/// <summary>
		/// Invokes the functionality for when this button is not selected anymore.
		/// </summary>
		public void OnDeselect ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;
			
			// If the selected button index is assigned, and it is the same as this button index, then reset the selected index.
			if( radialMenu.CurrentSelectedButtonIndex >= 0 && radialMenu.CurrentSelectedButtonIndex == buttonIndex )
				radialMenu.CurrentSelectedButtonIndex = -1;

			// Call OnExit to reset the button.
			OnExit();
		}

		/// <summary>
		/// Invokes the functionality for when button is disabled.
		/// </summary>
		public void DisableButton ()
		{
			// If the radial image is null, then return.
			if( radialImage == null )
				return;

			// If the selected button index is assigned, and it is the same as this button index, then reset the selected index.
			if( radialMenu.CurrentSelectedButtonIndex >= 0 && radialMenu.CurrentSelectedButtonIndex == buttonIndex )
				radialMenu.CurrentSelectedButtonIndex = -1;

			// Set the disable button to true so that nothing will be calculated on this button.
			buttonDisabled = true;

			// If the user wants to use a disabled sprite then apply the sprite.
			if( radialMenu.spriteSwap && radialMenu.disabledSprite != null )
				radialImage.sprite = radialMenu.disabledSprite;

			// If the use wants to change the color, then apply the color.
			if( radialMenu.colorChange && radialImage.sprite != null )
				radialImage.color = radialMenu.disabledColor;
			
			// If the user is scaling the transform, then reset the scale and position.
			if( radialMenu.scaleTransform )
			{
				radialImage.GetComponent<RectTransform>().localScale = Vector3.one * radialMenu.disabledScaleModifier;
				radialImage.GetComponent<RectTransform>().localPosition = disabledPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
			{
				// If the user wants to change color, then apply the disabled color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconDisabledColor;

				// If the user wants to scale the transform of the icon, then apply the normal scale.
				if( radialMenu.iconScaleTransform )
					iconTransform.localScale = iconNormalScale * radialMenu.iconDisabledScaleModifier;
			}

			// If the user wants to use text, it is assigned, and the user wants to change the color, then apply the color.
			if( radialMenu.useButtonText && text != null && radialMenu.textColorChange )
				text.color = radialMenu.textDisabledColor;
		}

		/// <summary>
		/// Invokes the functionality for when button is enabled.
		/// </summary>
		public void EnableButton ()
		{
			// If the radial image is null, then return.
			if( radialImage == null )
				return;

			// Set the disable button to false so that calculations can continue on this button.
			buttonDisabled = false;

			// Call OnExit to reset the button.
			OnExit();
		}

		/// <summary>
		/// Invokes the functionality for when the input is down on the button.
		/// </summary>
		public void OnInputDown ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;

			// If the user wants to swap sprites and the pressed sprite isn't null, then apply the sprite.
			if( radialMenu.spriteSwap && radialMenu.pressedSprite != null )
				radialImage.sprite = radialMenu.pressedSprite;

			// If the user wants to change the color, then apply the pressed color to the image.
			if( radialMenu.colorChange && radialImage.sprite != null )
				radialImage.color = radialMenu.pressedColor;

			// If the user wants to scale the transform...
			if( radialMenu.scaleTransform )
			{
				// Then apply the scale modifier and position.
				radialImage.GetComponent<RectTransform>().localScale = Vector3.one * radialMenu.pressedScaleModifier;
				radialImage.GetComponent<RectTransform>().localPosition = pressedPosition;
			}

			// If the user wants to use the icon and it is assigned...
			if( radialMenu.useButtonIcon && icon != null && icon.sprite != null )
			{
				// If the user wants to change color, then apply the pressed color.
				if( radialMenu.iconColorChange )
					icon.color = radialMenu.iconPressedColor;

				// If the user wants to scale the transform of the icon, then apply the normal scale multiplied by the pressed mod.
				if( radialMenu.iconScaleTransform )
					iconTransform.localScale = iconNormalScale * radialMenu.iconPressedScaleModifier;
			}

			// If the user wants to use text and it is assigned...
			if( radialMenu.useButtonText && text != null )
			{
				// If the user wants to change color, then apply the pressed color.
				if( radialMenu.textColorChange )
					text.color = radialMenu.textPressedColor;
			}
		}

		/// <summary>
		/// Invokes the functionality for when the input is release on the button.
		/// </summary>
		public void OnInputUp ()
		{
			// If the button is disabled, then return.
			if( buttonDisabled )
				return;
		
			// If this button is still the currently selected button, then return to the OnEnter() function.
			if( buttonIndex == radialMenu.CurrentButtonIndex )
				OnEnter();
			// Else call the OnExit() function because the input has left this button.
			else
				OnExit();
		}
		
		/// <summary>
		/// Subscribes the provided function to the radial button interaction event.
		/// </summary>
		/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
		public void AddCallback ( Action ButtonCallback )
		{
			OnRadialButtonInteract += ButtonCallback;
		}

		/// <summary>
		/// Subscribes the provided function to the radial button interaction event.
		/// </summary>
		/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
		public void AddCallback ( Action<int> ButtonCallback )
		{
			OnRadialButtonInteractWithId += ButtonCallback;
		}

		/// <summary>
		/// Subscribes the provided function to the radial button interaction event.
		/// </summary>
		/// <param name="ButtonCallback">The action callback to call when this new button is being interacted with.</param>
		public void AddCallback ( Action<string> ButtonCallback )
		{
			OnRadialButtonInteractWithKey += ButtonCallback;
		}

		/// <summary>
		/// [Internal] Registers the information to the radial button.
		/// </summary>
		/// <param name="ButtonCallback">The Callback function to call when interacted with.</param>
		/// <param name="buttonInfo">The information to apply to the button.</param>
		public void RegisterButtonInfo ( UltimateRadialButtonInfo buttonInfo )
		{
			// Set registered to true since we are assigning the information.
			registered = true;

			// Assign this radial button to the buttonInfo so that it can reference this button.
			buttonInfo.radialButton = this;

			// Register the button info values.
			id = buttonInfo.id;
			key = buttonInfo.key;
			name = buttonInfo.name;
			description = buttonInfo.description;

			// Subscribe the buttonInfo clear function so that it can be notified when this radial button is cleared.
			OnClearButtonInformation += buttonInfo.OnClearButtonInformation;

			// If the icon image is assigned and the user wants to show the icon...
			if( icon != null && radialMenu.useButtonIcon )
			{
				// If the provided icon sprite is assigned, then assign the sprite and set the color.
				if( buttonInfo.icon != null )
				{
					icon.sprite = buttonInfo.icon;
					icon.color = radialMenu.iconNormalColor;
				}
				// Else just set the color of the icon image to clear.
				else
					icon.color = Color.clear;
			}
			
			// If the text isn't null then assign the radial info text.
			if( text != null && radialMenu.displayNameOnButton )
				text.text = buttonInfo.name;
		}

		/// <summary>
		/// Clears the button information.
		/// </summary>
		public void ClearButtonInformation ()
		{
			// Set registered and disabled to false since the button is being cleared.
			registered = false;
			buttonDisabled = false;

			// Reset the key and id values.
			key = "";
			id = -1;
			name = "";
			description = "";

			// If the user has spriteSwap enabled, then change back the sprite.
			if( radialMenu.spriteSwap )
				radialImage.sprite = radialMenu.normalSprite;

			// Reset the button color.
			if( radialImage.sprite != null )
				radialImage.color = radialMenu.normalColor;

			// Reset the button icon.
			if( icon != null )
			{
				icon.sprite = null;
				icon.color = Color.clear;
			}
			
			// If the text component is assigned, then reset the text.
			if( text != null )
				text.text = "";

			// Notify any button information subscribers that this button has been cleared.
			if( OnClearButtonInformation != null )
				OnClearButtonInformation();

			// Reset callbacks.
			OnRadialButtonInteract = null;
			OnRadialButtonInteractWithId = null;
			OnRadialButtonInteractWithKey = null;
			OnClearButtonInformation = null;
			unityEvent = null;
		}
	}
	public List<UltimateRadialButton> UltimateRadialButtonList = new List<UltimateRadialButton>();
	List<UltimateRadialButton> UltimateRadialButtonPool = new List<UltimateRadialButton>();
	public UltimateRadialMenuInputManager inputManager;

	// SCRIPT REFERENCE //
	static Dictionary<string, UltimateRadialMenu> UltimateRadialMenus = new Dictionary<string, UltimateRadialMenu>();
	public string radialMenuName = string.Empty;

	// ACTION SUBSCRIPTIONS //
	/// <summary>
	/// This callback will be called when a radial button has been entered.
	/// </summary>
	public event Action<int> OnRadialButtonEnter;
	/// <summary>
	/// This callback will be called when a radial button has been exited.
	/// </summary>
	public event Action<int> OnRadialButtonExit;
	/// <summary>
	/// This callback will be called when the input has been pressed on a radial button.
	/// </summary>
	public event Action<int> OnRadialButtonInputDown;
	/// <summary>
	/// This callback will be called when the input has been released on a radial button.
	/// </summary>
	public event Action<int> OnRadialButtonInputUp;
	/// <summary>
	/// This callback will be called when a radial button has been interacted with.
	/// </summary>
	public event Action<int> OnRadialButtonInteract;
	/// <summary>
	/// This callback will be called when a radial button has been selected.
	/// </summary>
	public event Action<int> OnRadialButtonSelected;
	/// <summary>
	/// This callback will be called when the radial menu has lost focus.
	/// </summary>
	public event Action OnRadialMenuLostFocus;
	/// <summary>
	/// This callback will be called when the radial menu has been enabled.
	/// </summary>
	public event Action OnRadialMenuEnabled;
	/// <summary>
	/// This callback will be called when the radial menu has been disabled.
	/// </summary>
	public event Action OnRadialMenuDisabled;
	/// <summary>
	/// This callback will be called when the radial menu's positioning has been updated.
	/// </summary>
	public event Action OnUpdatePositioning;
	/// <summary>
	/// This callback will be called when a radial button is added or subtracted from the Radial Menu during this frame. This is useful for swapping sprites and positioning for a new count.
	/// </summary>
	public event Action<int> OnRadialMenuButtonCountModified;
	

	void Awake ()
	{
		// If the application is not playing, then return.
		if( !Application.isPlaying )
			return;

		// If the name is assigned...
		if( radialMenuName != string.Empty )
		{
			// Check to see if the dictionary already contains this name, and if so, remove the current one.
			if( UltimateRadialMenus.ContainsKey( radialMenuName ) )
				UltimateRadialMenus.Remove( radialMenuName );

			// Register this UltimateRadialMenu into the dictionary.
			UltimateRadialMenus.Add( radialMenuName, GetComponent<UltimateRadialMenu>() );
		}

		// Assign the canvas group component.
		canvasGroup = GetComponent<CanvasGroup>();

		// Reset the stored indexes. 
		CurrentButtonIndex = -1;
		buttonIndexOnInputDown = -1;
		CurrentSelectedButtonIndex = -1;

		// Reset the menu.
		ResetRadialMenu();

		// Store the base transform.
		BaseTransform = GetComponent<RectTransform>();

		// Disable the radial menu immediately if the user wants it disabled.
		if( initialState == InitialState.Disabled )
		{
			RadialMenuActive = false;
			Interactable = false;
			DisableRadialMenuImmediate();
		}
		// Else set the radial menu as active and able to be interacted with.
		else
		{
			RadialMenuActive = true;
			Interactable = true;
		}
	}

	void Start ()
	{
		// If the game is running, then return.
		if( !Application.isPlaying )
			return;

		// If there is an Input Manager script on this gameObject, then the input must be unique, so add this radial menu to it's list.
		if( GetComponent<UltimateRadialMenuInputManager>() )
			GetComponent<UltimateRadialMenuInputManager>().AddRadialMenuToList( this, ref inputManager );
		// Else, find the master Input Manager and add this radial menu to that list.
		else
		{
			// If there is no input manager, then add a input manager component to the event system.
			if( FindObjectOfType<EventSystem>() && !FindObjectOfType<EventSystem>().gameObject.GetComponent<UltimateRadialMenuInputManager>() )
			{
				Debug.LogWarning( "Ultimate Radial Menu\nThere was no Ultimate Radial Menu Input Manager on the EventSystem in your scene. Adding a default Ultimate Radial Menu Input Manager to avoid errors, but you should ensure that you have an Ultimate Radial Menu Input Manager on your EventSystem so that you can customize the settings." );

				if( FindObjectOfType<EventSystem>() )
					FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();
			}

			// Update the radial menu list.
			UltimateRadialMenuInputManager.Instance.AddRadialMenuToList( this, ref inputManager );
		}

		// If the parent canvas is null...
		if( ParentCanvas == null )
		{
			// Update the ParentCanvas component.
			UpdateParentCanvas();

			// If it is still null, then warn the user and return to avoid errors.
			if( ParentCanvas == null )
			{
				Debug.LogError( "Ultimate Radial Menu\nThis component is not with a Canvas object. Disabling this component to avoid any errors." );
				enabled = false;
				return;
			}
		}

		// Update the positioning of the Ultimate Radial Menu.
		UpdatePositioning();
		
		// If the parent canvas does not have a screen size updater, then add it.
		if( !ParentCanvas.GetComponent<UltimateRadialMenuScreenSizeUpdater>() )
			ParentCanvas.gameObject.AddComponent<UltimateRadialMenuScreenSizeUpdater>();
	}

	#if UNITY_EDITOR
	void Update ()
	{
		// If the application is not playing (in edit mode of Unity) update the positioning.
		if( !Application.isPlaying )
			UpdatePositioning();
	}

	void Reset ()
	{
		// If this game object does not have a RectTransform component, then replace the normal Transform component with a RectTranform.
		if( !GetComponent<RectTransform>() )
			gameObject.AddComponent<RectTransform>();
	}
	#endif

	/// <summary>
	/// This function is called by Unity when the parent of this transform changes.
	/// </summary>
	void OnTransformParentChanged ()
	{
		UpdateParentCanvas();
	}

	/// <summary>
	/// Updates the parent canvas if it has changed.
	/// </summary>
	public void UpdateParentCanvas ()
	{
		// Store the parent of this object.
		Transform parent = transform.parent;

		// If the parent is null, then just return.
		if( parent == null )
			return;

		// While the parent is assigned...
		while( parent != null )
		{
			// If the parent object has a Canvas component, then assign the ParentCanvas and transform.
			if( parent.transform.GetComponent<UnityEngine.Canvas>() )
			{
				ParentCanvas = parent.transform.GetComponent<UnityEngine.Canvas>();
				canvasRectTrans = ParentCanvas.GetComponent<RectTransform>();
				return;
			}

			// If the parent does not have a canvas, then store it's parent to loop again.
			parent = parent.transform.parent;
		}
	}

	/// <summary>
	/// Processes the input and calculates the information for use within the Ultimate Radial Menu.
	/// </summary>
	/// <param name="input">The current input values.</param>
	/// <param name="distance">The distance of the input from the center of the radial menu.</param>
	/// <param name="inputDown">The state of the input being pressed this frame.</param>
	/// <param name="inputUp">The state of the input being released this frame.</param>
	public void ProcessInput ( Vector2 input, float distance, bool inputDown, bool inputUp )
	{
		// If the radial menu is inactive then return.
		if( !RadialMenuActive || !Interactable )
			return;
		
		// Set the InputInRange bool to false by default.
		InputInRange = false;

		// If the menu is not in world space, then cast the input to be relative to this transform. This is needed for if the radial menu is rotated like in a carousel type menu.
		if( !IsWorldSpaceRadialMenu )
			input = BaseTransform.InverseTransformPoint( BasePosition + ( Vector3 )input );
		
		// Calculate the angle of the input and convert it to degrees.
		float angle = Mathf.Atan2( input.y, input.x ) * Mathf.Rad2Deg;

		// If the angle is negative, then add 360 to make it positive.
		if( angle < 0 )
			angle += 360;
		
		// Store the current angle so that other scripts can get it.
		GetCurrentInputAngle = angle;

		// Loop through all of the radial menu buttons...
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
		{
			// If the distance of the input exceeds the boundaries of the radial menu...
			if( distance < CalculatedMinRange || distance > CalculatedMaxRange )
			{
				// If the input was in range the last frame...
				if( inputInRangeLastFrame )
				{
					// Reset the radial menu.
					ResetRadialMenu();
					
					// Notify any subscriptions that the radial menu has lost focus.
					if( OnRadialMenuLostFocus != null )
						OnRadialMenuLostFocus();
				}

				// Break the loop.
				break;
			}

			// If there was a current button with input on it, and the input is still within the angle of that button, then set InputInRange to true and break the loop.
			if( CurrentButtonIndex >= 0 && UltimateRadialButtonList[ CurrentButtonIndex ].IsInAngle( angle ) )
			{
				InputInRange = true;
				break;
			}

			// If the angle is within the range of the current radial menu button...
			if( UltimateRadialButtonList[ i ].IsInAngle( angle ) )
			{
				// Set the InputInRange to true.
				InputInRange = true;
				
				// If the current index is greater than -1, then exit the current button.
				if( CurrentButtonIndex >= 0 && CurrentButtonIndex < UltimateRadialButtonList.Count )
				{
					buttonIndexOnInputDown = -1;	
					UltimateRadialButtonList[ CurrentButtonIndex ].OnExit();

					// Inform and subscribers that this button has been exited.
					if( OnRadialButtonExit != null )
						OnRadialButtonExit( CurrentButtonIndex );
				}

				// Assign the current button index to this index.
				CurrentButtonIndex = i;

				// Call the OnEnter function on the current button.
				UltimateRadialButtonList[ i ].OnEnter();

				// Inform any subscribers that this button has been entered.
				if( OnRadialButtonEnter != null )
					OnRadialButtonEnter( i );
				
				// If the user wants to display the name of the button and the text is assigned, then apply the name.
				if( displayButtonName && nameText != null )
					nameText.text = !UltimateRadialButtonList[ i ].buttonDisabled ? UltimateRadialButtonList[ i ].name : "";

				// If the user wants to display the description of the button and the text is assigned, then display the description.
				if( displayButtonDescription && descriptionText != null )
					descriptionText.text = !UltimateRadialButtonList[ i ].buttonDisabled ? UltimateRadialButtonList[ i ].description : "";
				
				// Break the loop.
				break;
			}

			// If this loop has reached the end of the button list and no buttons have been found worthy...
			if( i == UltimateRadialButtonList.Count - 1 )
			{
				// Reset the radial menu.
				ResetRadialMenu();

				// Inform any subscribers that the radial menu has lost focus.
				if( OnRadialMenuLostFocus != null )
					OnRadialMenuLostFocus();
			}
		}

		// If the last frame caught input, but now the radial button list is zero...
		if( inputInRangeLastFrame && UltimateRadialButtonList.Count == 0 )
		{
			// Inform any subscribers that the radial menu has lost focus.
			if( OnRadialMenuLostFocus != null )
				OnRadialMenuLostFocus();

			// Reset the radial menu.
			ResetRadialMenu();
		}

		// If the input is down on this frame, the input is in range, and the current index is within range...
		if( CurrentButtonIndex >= 0 && !UltimateRadialButtonList[ CurrentButtonIndex ].buttonDisabled && inputDown && InputInRange )
		{
			// Call the OnInputDown() function on the current button.
			if( CurrentSelectedButtonIndex != CurrentButtonIndex )
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInputDown();

			// If the invoke action is set to being when the button is down...
			if( inputManager.invokeAction == UltimateRadialMenuInputManager.InvokeAction.OnButtonDown )
			{
				// Call the OnInteract() function on the current button.
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInteract();
				
				// If the input manager wants to disable the radial menu when interacted with, then disable the menu.
				if( inputManager.disableOnInteract && !IsWorldSpaceRadialMenu )
					DisableRadialMenu();
			}

			// Set the button index to the buttonIndexOnInputDown so that the button up can be calculated.
			buttonIndexOnInputDown = CurrentButtonIndex;

			// Inform and subscribers that this button has received down input.
			if( OnRadialButtonInputDown != null )
				OnRadialButtonInputDown( CurrentButtonIndex );
		}

		// If the input is up on this frame, the input is in range, and the current index is within range...
		if( CurrentButtonIndex >= 0 && !UltimateRadialButtonList[ CurrentButtonIndex ].buttonDisabled && inputUp && InputInRange )
		{
			// Call the OnInputUp() function on the current button.
			if( CurrentSelectedButtonIndex != CurrentButtonIndex )
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInputUp();
			
			// If the invoke action is set to being when the button has been clicked, and the current button index is the same as when the buttonIndexOnInputDown was calculated...
			if( inputManager.invokeAction == UltimateRadialMenuInputManager.InvokeAction.OnButtonClick && CurrentButtonIndex == buttonIndexOnInputDown )
			{
				// Call the OnInteract() function on the current button.
				UltimateRadialButtonList[ CurrentButtonIndex ].OnInteract();
				
				// If the input manager wants to disable the radial menu when interacted with, then disable the menu.
				if( inputManager.disableOnInteract && !IsWorldSpaceRadialMenu )
					DisableRadialMenu();
			}

			// Reset the buttonIndexOnInputDown.
			buttonIndexOnInputDown = -1;

			// Inform and subscribers that this button has received up input.
			if( OnRadialButtonInputUp != null )
				OnRadialButtonInputUp( CurrentButtonIndex );
		}

		// Store the InputInRange value for the next calculation.
		inputInRangeLastFrame = InputInRange;
	}

	/// <summary>
	/// Resets the Ultimate Radial Menu Buttons and all the enabled options to their default state.
	/// </summary>
	void ResetRadialMenu ()
	{
		// If the current index greater than or equal to zero, then reset the current button.
		if( CurrentButtonIndex >= 0 && CurrentButtonIndex < UltimateRadialButtonList.Count )
			UltimateRadialButtonList[ CurrentButtonIndex ].OnExit();

		// Set the current button index to -1, resetting it.
		CurrentButtonIndex = -1;
		buttonIndexOnInputDown = -1;

		// If the user is wanting to display the current selection on the radial menu, reset the text.
		if( displayButtonName && nameText != null )
			nameText.text = "";

		// If the user is wanting to display the description of the selected button, then reset the text here.
		if( displayButtonDescription && descriptionText != null )
			descriptionText.text = "";
	}

	/// <summary>
	/// Fades the canvas group component over time.
	/// </summary>
	IEnumerator FadeRadialMenu ()
	{
		// Set InTransition to true so that other functions can know this is running.
		InTransition = true;

		// Set Interactable to false so that the user cannot interact while the menu is transitioning.
		Interactable = false;

		// Calculate the speed of the fade.
		float speed = 1.0f / toggleInDuration;

		// Store the starting alpha value so that the transition will be smooth.
		float startingAlpha = canvasGroup.alpha;
		for( float t = 0.0f; t < 1.0f && RadialMenuActive; t += Time.unscaledDeltaTime * speed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( speed ) )
				break;

			// Lerp the alpha by the current alpha to 1.
			canvasGroup.alpha = Mathf.Lerp( startingAlpha, 1.0f, t );
			yield return null;
		}

		// If the radial menu is still active, apply the final alpha value.
		if( RadialMenuActive )
			canvasGroup.alpha = 1.0f;

		// Set Interactable to true now since the menu has finished it's transition.
		Interactable = true;

		// Hold here while the radial menu is active.
		while( RadialMenuActive )
			yield return null;

		// Set Interactable to false since the menu is transitioning again.
		Interactable = false;

		// Store the current alpha value so that the transition will be smooth.
		speed = 1.0f / toggleOutDuration;
		startingAlpha = canvasGroup.alpha;
		for( float t = 0.0f; t < 1.0f && !RadialMenuActive; t += Time.unscaledDeltaTime * speed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( speed ) )
				break;

			// Lerp the alpha from the current to 0.
			canvasGroup.alpha = Mathf.Lerp( startingAlpha, 0.0f, t );
			yield return null;
		}

		// If the radial menu is not active still, then apply 0 as the final alpha.
		if( !RadialMenuActive )
			canvasGroup.alpha = 0.0f;

		// Notify any subscribers that the radial menu is now disabled.
		if( OnRadialMenuDisabled != null )
			OnRadialMenuDisabled();

		// Set InTransition to false.
		InTransition = false;
	}

	/// <summary>
	/// Scales the transform of the radial menu over time.
	/// </summary>
	IEnumerator ScaleRadialMenu ()
	{
		// Set InTransition to true so that other functions can know this is running.
		InTransition = true;

		// Set Interactable to false so that the user cannot interact with the menu while it is transitioning.
		Interactable = false;

		// Calculate the speed of the fade.
		float speed = 1.0f / toggleInDuration;

		// Store the starting scale so that the transition will be smooth.
		Vector3 startingScale = BaseTransform.localScale;
		for( float t = 0.0f; t < 1.0f && RadialMenuActive; t += Time.unscaledDeltaTime * speed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( speed ) )
				break;

			// Lerp the scale from the starting scale to a Vector3.one.
			BaseTransform.localScale = Vector3.Lerp( startingScale, Vector3.one, t );
			yield return null;
		}

		// If the radial menu is still active, apply the final scale. 
		if( RadialMenuActive )
			BaseTransform.localScale = Vector3.one;

		// Set Interactable to true now since the menu has finished it's transition.
		Interactable = true;

		// Loop here while the radial menu is active.
		while( RadialMenuActive )
			yield return null;

		// Set Interactable to false since the menu is transitioning again.
		Interactable = false;

		speed = 1.0f / toggleOutDuration;
		// Store the current scale so that the transition will be smooth.
		startingScale = BaseTransform.localScale;
		for( float t = 0.0f; t < 1.0f && !RadialMenuActive; t += Time.unscaledDeltaTime * speed )
		{
			// If the speed is NaN, then break the coroutine.
			if( float.IsInfinity( speed ) )
				break;

			// Lerp the scale from the current to 0.
			BaseTransform.localScale = Vector3.Lerp( startingScale, Vector3.zero, t );
			yield return null;
		}

		// If the radial menu is still inactive, apply the final scale. 
		if( !RadialMenuActive )
			BaseTransform.localScale = Vector3.zero;

		// Notify any subscribers that the radial menu is now disabled.
		if( OnRadialMenuDisabled != null )
			OnRadialMenuDisabled();

		// Set InTransition to false so other functions know.
		InTransition = false;
	}

	/// <summary>
	/// Returns the ratio of the targeted sprite.
	/// </summary>
	/// <param name="sprite">The sprite to calculate the ratio of.</param>
	Vector2 GetImageAspectRatio ( Sprite sprite )
	{
		Vector2 ratio = Vector2.one;
		
		// Store the raw values of the sprites ratio so that a smaller value can be configured.
		Vector2 rawRatio = new Vector2( sprite.rect.width, sprite.rect.height );

		// Temporary float to store the largest side of the sprite.
		float maxValue = rawRatio.x > rawRatio.y ? rawRatio.x : rawRatio.y;

		// Now configure the ratio based on the above information.
		ratio.x = rawRatio.x / maxValue;
		ratio.y = rawRatio.y / maxValue;

		return ratio;
	}

	/// <summary>
	/// Creates a button at the specified index.
	/// </summary>
	/// <param name="buttonIndex">The targeted button index to create the button at.</param>
	void CreateRadialButtonAtIndex ( int buttonIndex )
	{
		// If the button index is less than zero, then make is zero to avoid errors.
		if( buttonIndex < 0 )
			buttonIndex = 0;

		// If the button index is higher than the button list count, then assign it to the end of the list.
		if( buttonIndex > UltimateRadialButtonList.Count )
			buttonIndex = UltimateRadialButtonList.Count;

		// If the radial menu style is assigned and the count is greater than the max button count for this style, then warn the user.
		if( radialMenuStyle != null && UltimateRadialButtonList.Count >= radialMenuStyle.maxButtonCount )
			Debug.LogWarning( "Ultimate Radial Menu\nThe current radial menu button count is out of range for this style. The buttons may look strange because there is no corresponding button sprite to use with this count." );

		// If there is a radial button in the pool list, then insert the first radial button from the pool list.
		if( UltimateRadialButtonPool.Count > 0 )
			UltimateRadialButtonList.Insert( buttonIndex, GetRadialButtonFromPool() );
		// Else there is no pooled buttons, so create a new one.
		else
		{
			// Insert a new radial button at the targeted index.
			UltimateRadialButtonList.Insert( buttonIndex, new UltimateRadialButton() );

			// Assign the radial menu variable.
			UltimateRadialButtonList[ buttonIndex ].radialMenu = this;

			// Create a new base game object and add a RectTransform component.
			GameObject newGameObject = new GameObject();
			newGameObject.AddComponent<RectTransform>();
			newGameObject.AddComponent<CanvasRenderer>();
			newGameObject.AddComponent<Image>();

			// Create the button game object, set the parent, and change the name.
			GameObject radialButton = Instantiate( newGameObject, Vector3.zero, Quaternion.identity );
			radialButton.transform.SetParent( BaseTransform );
			radialButton.gameObject.name = "Radial Menu Button";
			radialButton.transform.SetAsLastSibling();

			// Store the RectTransform component and modify it.
			UltimateRadialButtonList[ buttonIndex ].buttonTransform = radialButton.GetComponent<RectTransform>();
			UltimateRadialButtonList[ buttonIndex ].buttonTransform.anchorMin = new Vector2( 0.5f, 0.5f );
			UltimateRadialButtonList[ buttonIndex ].buttonTransform.anchorMax = new Vector2( 0.5f, 0.5f );
			UltimateRadialButtonList[ buttonIndex ].buttonTransform.pivot = new Vector2( 0.5f, 0.5f );
			UltimateRadialButtonList[ buttonIndex ].buttonTransform.localScale = Vector3.one;

			// Store the image component and update the sprite and color.
			UltimateRadialButtonList[ buttonIndex ].radialImage = radialButton.GetComponent<Image>();
			UltimateRadialButtonList[ buttonIndex ].radialImage.sprite = normalSprite;
			if( UltimateRadialButtonList[ buttonIndex ].radialImage.sprite != null )
				UltimateRadialButtonList[ buttonIndex ].radialImage.color = normalColor;
			else
				UltimateRadialButtonList[ buttonIndex ].radialImage.color = Color.clear;

			// If the user wants to display a icon on the button...
			if( useButtonIcon )
			{
				// Create the button icon game object. Then set the parent as the baseButton's transform, and change the name.
				GameObject buttonIcon = Instantiate( newGameObject, Vector3.zero, Quaternion.identity );
				buttonIcon.transform.SetParent( UltimateRadialButtonList[ buttonIndex ].buttonTransform );
				buttonIcon.gameObject.name = "Icon " + buttonIndex.ToString( "00" );

				// Store the RectTransform component and set it to fill up the parent transform.
				UltimateRadialButtonList[ buttonIndex ].iconTransform = buttonIcon.GetComponent<RectTransform>();
				UltimateRadialButtonList[ buttonIndex ].buttonTransform.anchorMin = new Vector2( 0.5f, 0.5f );
				UltimateRadialButtonList[ buttonIndex ].buttonTransform.anchorMax = new Vector2( 0.5f, 0.5f );
				UltimateRadialButtonList[ buttonIndex ].buttonTransform.pivot = new Vector2( 0.5f, 0.5f );
				UltimateRadialButtonList[ buttonIndex ].iconTransform.localScale = Vector3.one;

				// Store the buttonIcon image component and clear the sprite and update the color to clear.
				UltimateRadialButtonList[ buttonIndex ].icon = buttonIcon.GetComponent<Image>();
				UltimateRadialButtonList[ buttonIndex ].icon.sprite = null;
				UltimateRadialButtonList[ buttonIndex ].icon.color = Color.clear;
			}

			// If the user wants to use text on the button...
			if( useButtonText )
			{
				// Create a new game object for the text, add a RectTransform and a CanvasRenderer component.
				GameObject newTextObject = new GameObject();
				newTextObject.AddComponent<RectTransform>();
				newTextObject.AddComponent<CanvasRenderer>();

				// Create the cooldown text object, set the parent and name.
				GameObject buttonText = Instantiate( newTextObject, Vector3.zero, Quaternion.identity );
				buttonText.transform.SetParent( UltimateRadialButtonList[ buttonIndex ].buttonTransform );
				buttonText.gameObject.name = "Text " + buttonIndex.ToString( "00" );

				// Store the text component and modify the settings.
				UltimateRadialButtonList[ buttonIndex ].text = buttonText.AddComponent<Text>();
				UltimateRadialButtonList[ buttonIndex ].text.text = "";
				UltimateRadialButtonList[ buttonIndex ].text.alignment = TextAnchor.MiddleCenter;
				UltimateRadialButtonList[ buttonIndex ].text.resizeTextForBestFit = true;
				UltimateRadialButtonList[ buttonIndex ].text.resizeTextMinSize = 0;
				UltimateRadialButtonList[ buttonIndex ].text.resizeTextMaxSize = 300;
				if( buttonTextFont != null )
					UltimateRadialButtonList[ buttonIndex ].text.font = buttonTextFont;
				UltimateRadialButtonList[ buttonIndex ].text.color = textNormalColor;
				UltimateRadialButtonList[ buttonIndex ].text.rectTransform.localScale = Vector3.one;

				// If the user wants a text outline on the button text...
				if( buttonTextOutline )
				{
					// Add a outline component and update the color.
					UnityEngine.UI.Outline textOutline = buttonText.AddComponent<UnityEngine.UI.Outline>();
					textOutline.effectColor = buttonTextOutlineColor;
				}

				// Destroy the temporary text object that was created.
				Destroy( newTextObject );
			}

			// Destroy the temporary game object.
			Destroy( newGameObject );
		}

		// If the selected button index is assigned...
		if( CurrentSelectedButtonIndex >= 0 )
		{
			// Else if the button index was lower that the selected button index, then modify the index to fit the new index of the button.
			if( buttonIndex <= CurrentSelectedButtonIndex )
				CurrentSelectedButtonIndex += 1;
		}

		// Calculate the new information since the count has been modified.
		RadialMenuButtonCountModified();
	}

	/// <summary>
	/// Finds the radial button index according to the provided integer index. This function will "fix" the index to make sure that it does what the user wants it to do.
	/// </summary>
	/// <param name="buttonIndex">The button index to modify.</param>
	void FindRadialButtonIndex ( ref int buttonIndex )
	{
		// If the button index is unassigned, then find the next available button.
		if( buttonIndex < 0 )
		{
			// Loop through each button.
			for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
			{
				// If the button in not currently registered to anything, then assign this index.
				if( !UltimateRadialButtonList[ i ].Registered && !UltimateRadialButtonList[ i ].buttonDisabled )
				{
					buttonIndex = i;
					return;
				}
			}
			
			// If we have reached this part of code without finding any available buttons, then create a new button.
			CreateRadialButtonAtIndex( 1000 );

			// Set the index to the new button.
			buttonIndex = UltimateRadialButtonList.Count - 1;
			return;
		}

		// If the button index is greater than the count...
		if( buttonIndex > UltimateRadialButtonList.Count )
		{
			// Create a radial button at the end of the list.
			CreateRadialButtonAtIndex( 1000 );

			// Update the button index to the last button in the list.
			buttonIndex = UltimateRadialButtonList.Count - 1;
			return;
		}

		// If the targeted button is registered or if is it disabled then insert a button at this index.
		if( UltimateRadialButtonList[ buttonIndex ].Registered || UltimateRadialButtonList[ buttonIndex ].buttonDisabled )
			CreateRadialButtonAtIndex( buttonIndex );
	}
	
	/// <summary>
	/// Returns the first radial button in the pool and removes it from the list.
	/// </summary>
	UltimateRadialButton GetRadialButtonFromPool ()
	{
		// Grab the first pooled button, enable it, and remove it from the pool.
		UltimateRadialButton radialButton = UltimateRadialButtonPool[ 0 ];
		radialButton.buttonTransform.gameObject.SetActive( true );
		UltimateRadialButtonPool.Remove( radialButton );

		// Return the radial button.
		return radialButton;
	}

	/// <summary>
	/// Sends the radial button to the pool and removes it from the list of active buttons.
	/// </summary>
	/// <param name="buttonIndex">The targeted button index to send to the pool.</param>
	void SendRadialButtonToPool ( int buttonIndex )
	{
		// Add the targeted button to the pool list.
		UltimateRadialButtonPool.Add( UltimateRadialButtonList[ buttonIndex ] );
		
		// Set the game object to not active.
		UltimateRadialButtonList[ buttonIndex ].buttonTransform.gameObject.SetActive( false );

		// Remove this button from the list.
		UltimateRadialButtonList.RemoveAt( buttonIndex );
	}

	/// <summary>
	/// This updates the menuButtonCount and updates the needed information.
	/// </summary>
	void RadialMenuButtonCountModified ()
	{
		// Update the menuButtonCount value.
		menuButtonCount = UltimateRadialButtonList.Count;
		
		// If the user has set a style to use...
		if( radialMenuStyle != null )
		{
			// Loop through each style and check for the corresponding button count.
			for( int i = 0; i < radialMenuStyle.RadialMenuStyles.Count; i++ )
			{
				// If this style button count is the same as the radial button count...
				if( radialMenuStyle.RadialMenuStyles[ i ].buttonCount == menuButtonCount )
				{
					// Then assign this as the current style and break this loop.
					currentStyleIndex = i;
					break;
				}
			}
			
			// Apply the new style normal sprite for the buttons.
			normalSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].normalSprite;

			// If the user is wanting to use the Sprite Swap feature, then apply the sprites from this style.
			if( spriteSwap )
			{
				highlightedSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].highlightedSprite == null ? normalSprite : radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].highlightedSprite;
				pressedSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].pressedSprite == null ? normalSprite : radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].pressedSprite;
				selectedSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].selectedSprite == null ? normalSprite : radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].selectedSprite;
				disabledSprite = radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].disabledSprite == null ? normalSprite : radialMenuStyle.RadialMenuStyles[ currentStyleIndex ].disabledSprite;
			}

			// Loop through each radial button and update the image sprite to the new normal sprite.
			for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
			{
				if( UltimateRadialButtonList[ i ].buttonDisabled && spriteSwap )
					UltimateRadialButtonList[ i ].radialImage.sprite = disabledSprite;
				else if( CurrentSelectedButtonIndex >= 0 && CurrentSelectedButtonIndex == i && spriteSwap )
					UltimateRadialButtonList[ i ].radialImage.sprite = selectedSprite;
				else
					UltimateRadialButtonList[ i ].radialImage.sprite = normalSprite;
			}
		}

		// Inform any subscribers that the count has been modified.
		if( OnRadialMenuButtonCountModified != null )
			OnRadialMenuButtonCountModified( menuButtonCount );

		// Update the positioning.
		UpdatePositioning();
	}

	// -------------------------------------------------- < PUBLIC FUNCTIONS FOR THE USER > -------------------------------------------------- //
	/// <summary>
	/// Updates the positioning of the radial menu according to the user's options.
	/// </summary>
	public void UpdatePositioning ()
	{
		// If the parent canvas is null, then try to get the parent canvas component.
		if( ParentCanvas == null )
			UpdateParentCanvas();
		
		// If it is still null, then log a error and return.
		if( ParentCanvas == null )
		{
			Debug.LogError( "Ultimate Radial Menu\nThere is no parent canvas object. Please make sure that the Ultimate Radial Menu is placed within a canvas." );
			return;
		}

		// Set the current reference size for scaling.
		float referenceSize = scalingAxis == ScalingAxis.Height ? canvasRectTrans.sizeDelta.y : canvasRectTrans.sizeDelta.x;
		
		// Configure the target size for the graphic.
		float textureSize = referenceSize * ( menuSize / 10 );

		// If baseTrans is null, store this object's RectTrans so that it can be positioned.
		if( BaseTransform == null )
			BaseTransform = GetComponent<RectTransform>();
		
		// Apply the scale of 1 for calculations.
		BaseTransform.localScale = Vector3.one;

		// If the pivot is not 0.5, then set the pivot.
		if( BaseTransform.pivot != Vector2.one / 2 )
			BaseTransform.pivot = Vector2.one / 2;

		// First, fix the positioning to be a value between -0.5f and 0.5f.
		Vector2 fixedPositioning = new Vector2( horizontalPosition - 50, verticalPosition - 50 ) / 100;
		
		// Apply the positioning to the baseTransform.
		BaseTransform.localPosition = ( Vector3 )( canvasRectTrans.sizeDelta * fixedPositioning ) - ( Vector3 )( ( Vector2.one * textureSize ) * fixedPositioning );

		// Apply the texture size to the baseTransform.
		BaseTransform.sizeDelta = new Vector2( textureSize, textureSize );

		// Set the local rotation to zero.
		BaseTransform.localRotation = Quaternion.identity;
		
		// Store the default position of the radial menu.
		defaultPosition = BaseTransform.position;
		
		// Calculate the minimum range.
		CalculatedMinRange = ( BaseTransform.sizeDelta.x / 2 ) * minRange;
		
		// If the user wants to have an infinite max range then apply that, otherwise calculate the max range by the baseTransform's size delta.
		if( infiniteMaxRange )
			CalculatedMaxRange = Mathf.Infinity;
		else
			CalculatedMaxRange = ( BaseTransform.sizeDelta.x / 2 ) * maxRange;

		// If the render mode is not set to screen space overlay...
		if( ParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
		{
			// Set IsWorldSpaceRadialMenu to true so that calculations can be made for world space.
			IsWorldSpaceRadialMenu = true;

			// If the base gameObject does not have a box collider, then add one.
			if( !BaseTransform.GetComponent<BoxCollider>() )
				BaseTransform.gameObject.AddComponent<BoxCollider>();

			// Set the collider to trigger so that it doesn't have an effect on any physics.
			BaseTransform.GetComponent<BoxCollider>().isTrigger = true;

			// Apply the size of the box collider.
			BaseTransform.GetComponent<BoxCollider>().size = new Vector3( BaseTransform.sizeDelta.x * maxRange, BaseTransform.sizeDelta.y * maxRange, 0.001f );

			// Modify the position by the depth set by the user.
			BaseTransform.localPosition += new Vector3( 0, 0, depthPosition );
		}
		// Else the render mode is set to screen space.
		else
		{
			// Set IsWorldSpaceRadialMenu to false for calculations.
			IsWorldSpaceRadialMenu = false;

			// If there is a box collider on this object, then remove the component.
			if( BaseTransform.GetComponent<BoxCollider>() )
				DestroyImmediate( BaseTransform.GetComponent<BoxCollider>() );
		}

		// -------------------------- < CENTER TEXT POSITIONING > -------------------------- //
		// If the user wants to display the radial button name, and the text is assigned...
		if( displayButtonName && nameText != null )
		{
			// Configure text position and size.
			Vector2 textPosition = Vector2.zero;
			textPosition.x += BaseTransform.sizeDelta.x * ( ( nameTextHorizontalPosition - 50 ) / 100 );
			textPosition.y += BaseTransform.sizeDelta.y * ( ( nameTextVerticalPosition - 50 ) / 100 );
			nameText.rectTransform.sizeDelta = new Vector2( BaseTransform.sizeDelta.x * nameTextSize, BaseTransform.sizeDelta.x * nameTextSize ) * new Vector2( nameTextRatioX, nameTextRatioY );
			nameText.rectTransform.localPosition = textPosition;
			nameText.rectTransform.localRotation = Quaternion.identity;
		}

		// If the user wants to display the description of the button...
		if( displayButtonDescription && descriptionText != null )
		{
			// Configure text position and size.
			Vector2 textPosition = Vector2.zero;
			textPosition.x += BaseTransform.sizeDelta.x * ( ( descriptionTextHorizontalPosition - 50 ) / 100 );
			textPosition.y += BaseTransform.sizeDelta.y * ( ( descriptionTextVerticalPosition - 50 ) / 100 );
			descriptionText.rectTransform.sizeDelta = new Vector2( BaseTransform.sizeDelta.x * descriptionTextSize, BaseTransform.sizeDelta.x * descriptionTextSize ) * new Vector2( descriptionTextRatioX, descriptionTextRatioY );
			descriptionText.rectTransform.localPosition = textPosition;
			descriptionText.rectTransform.localRotation = Quaternion.identity;
		}

		// ------------------------- < RADIAL BUTTON POSITIONING > ------------------------- //
		// Configure the angle per button.
		float angle = GetAnglePerButton;

		// Convert the angle into radians. Here we are applying the angle as negative since we want the buttons to go clockwise.
		float angleInRadians = -angle * Mathf.Deg2Rad;
		
		// Configure the center angle offset according to the users options.
		float centerAngleOffset = 0.0f;

		// Switch for the angle offset setting.
		switch( angleOffset )
		{
			default:
			case AngleOffset.Centered:
			{
				centerAngleOffset = GetAnglePerButton / 2;
			}
			break;
			case AngleOffset.OffCenter:
			{
				centerAngleOffset = 0.0f;
			}
			break;
			case AngleOffset.OnlyEven:
			{
				if( menuButtonCount % 2 == 0 )
					centerAngleOffset = GetAnglePerButton / 2;
				else
					centerAngleOffset = 0.0f;
			}
			break;
			case AngleOffset.OnlyOdd:
			{
				if( menuButtonCount % 2 != 0 )
					centerAngleOffset = GetAnglePerButton / 2;
				else
					centerAngleOffset = 0.0f;
			}
			break;
		}

		// Configure how much to offset the rotation of the button by.
		float rotationOffset = -startingAngle + centerAngleOffset - ( GetAnglePerButton / 2 );

		float startingRotation = ( 90 * Mathf.Deg2Rad ) + ( -startingAngle * Mathf.Deg2Rad );
		
		// Store the buttons size.
		Vector2 buttonImageSize = ( BaseTransform.sizeDelta * menuButtonSize );

		// If the normal sprite is assigned then multiply the button size by the ratio of the sprite.
		if( normalSprite != null )
			buttonImageSize *= GetImageAspectRatio( normalSprite );

		// Configure the button radius. ( half of the base size * by the radius ) minus ( half of the button's Y size ).
		float buttonRadius = ( ( BaseTransform.sizeDelta.x / 2 ) * radialMenuButtonRadius ) - ( buttonImageSize.y / 2 );

		// Loop through all of the radial buttons.
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
		{
			UltimateRadialButtonList[ i ].buttonIndex = i;

			// If the radial image is null then try to find it.
			if( UltimateRadialButtonList[ i ].radialImage == null )
				UltimateRadialButtonList[ i ].radialImage = UltimateRadialButtonList[ i ].buttonTransform.GetComponent<Image>();

			// Apply the size to the button transform.
			UltimateRadialButtonList[ i ].buttonTransform.sizeDelta = buttonImageSize;

			// Configure a new position for the button.
			Vector3 normalPosition = Vector3.zero;

			// This code may seem like a bunch of voodoo magic, ( but that's what math is, am I right!? ) but essentially it is just adding together all of the angle options that were set and multiplying it by the button radius.
			normalPosition.x += ( Mathf.Cos( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * buttonRadius );
			normalPosition.y += ( Mathf.Sin( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * buttonRadius );

			// Apply the new position to the transform, as well as a default scale of one.
			UltimateRadialButtonList[ i ].buttonTransform.localPosition = normalPosition;
			UltimateRadialButtonList[ i ].buttonTransform.localScale = scaleTransform && UltimateRadialButtonList[ i ].buttonDisabled ? Vector3.one * disabledScaleModifier : Vector3.one;

			// If the user wants to scale the transform when interacted with...
			if( scaleTransform )
			{
				// Store the new position as the normal position to return to.
				UltimateRadialButtonList[ i ].normalPosition = normalPosition;

				// If the highlighted position modifier is assigned, then calculate the position.
				if( positionModifier != 0 )
				{
					// Configure the active position for the button.
					Vector3 activePosition = Vector3.zero;

					// Again, voodoo math, but the difference is that it is multiplying the radius modifier by the position modifier that the user has set.
					activePosition.x += ( Mathf.Cos( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * positionModifier ) ) );
					activePosition.y += ( Mathf.Sin( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * positionModifier ) ) );
					
					// Store the active position as the active position of the radial button.
					UltimateRadialButtonList[ i ].highlightedPosition = activePosition;
				}
				// Else assign the normal position.
				else
					UltimateRadialButtonList[ i ].highlightedPosition = normalPosition;

				// If the pressed position modifier is assigned, then calculate the position.
				if( pressedPositionModifier != 0 )
				{
					// Configure the pressed position for the button.
					Vector3 pressedPosition = Vector3.zero;

					// Again, voodoo math, but the difference is that it is multiplying the radius modifier by the pressed position modifier that the user has set.
					pressedPosition.x += ( Mathf.Cos( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * selectedPositionModifier ) ) );
					pressedPosition.y += ( Mathf.Sin( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * selectedPositionModifier ) ) );
					
					// Store the pressed position as the pressed position of the radial button.
					UltimateRadialButtonList[ i ].pressedPosition = pressedPosition;
				}
				// Else assign the normal position.
				else
					UltimateRadialButtonList[ i ].pressedPosition = normalPosition;

				if( selectedPositionModifier != 0 )
				{
					// Configure the selected position for the button.
					Vector3 selectedPosition = Vector3.zero;

					// Again, voodoo math, but the difference is that it is multiplying the radius modifier by the selected position modifier that the user has set.
					selectedPosition.x += ( Mathf.Cos( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * selectedPositionModifier ) ) );
					selectedPosition.y += ( Mathf.Sin( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * selectedPositionModifier ) ) );
					
					// Store the selected position as the selected position of the radial button.
					UltimateRadialButtonList[ i ].selectedPosition = selectedPosition;
				}
				// Else assign the normal position.
				else
					UltimateRadialButtonList[ i ].selectedPosition = normalPosition;

				if( disabledPositionModifier != 0 )
				{
					// Configure the disabled position for the button.
					Vector3 disabledPosition = Vector3.zero;

					// Again, voodoo math, but the difference is that it is multiplying the radius modifier by the disabled position modifier that the user has set.
					disabledPosition.x += ( Mathf.Cos( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * disabledPositionModifier ) ) );
					disabledPosition.y += ( Mathf.Sin( ( ( angleInRadians * i ) ) + startingRotation + ( centerAngleOffset * Mathf.Deg2Rad ) + ( angleInRadians / 2 ) ) * ( buttonRadius + ( buttonRadius * disabledPositionModifier ) ) );
					
					// Store the disabled position as the disabled position of the radial button.
					UltimateRadialButtonList[ i ].disabledPosition = disabledPosition;

					if( UltimateRadialButtonList[ i ].buttonDisabled )
						UltimateRadialButtonList[ i ].buttonTransform.localPosition = disabledPosition;
				}
				// Else assign the normal position.
				else
					UltimateRadialButtonList[ i ].disabledPosition = normalPosition;
			}

			// Store the angle of the radial button. This starts at 90 degrees to start the menu straight up. After that just add/subtract the angle information.
			UltimateRadialButtonList[ i ].angle = 90 + -startingAngle + centerAngleOffset + ( -angle * i ) - ( angle / 2 );
			
			// Configure the angle range for calculations.
			UltimateRadialButtonList[ i ].angleRange = ( angle / 2 ) * buttonInputAngle;

			// If the value is less than -360, then increase by 360 to try to get a positive number. This is needed for the Starting Angle variable if the user sets it to a high value.
			if( UltimateRadialButtonList[ i ].angle <= -360 )
				UltimateRadialButtonList[ i ].angle += 360;

			// If the calculated angle is negative, then add 360 to try and make the value more readable and easy to work with.
			if( UltimateRadialButtonList[ i ].angle < 0 )
				UltimateRadialButtonList[ i ].angle += 360;
			
			// Configure the new rotation to apply to the button.
			Vector3 newRotation = Vector3.zero;

			// If the user wants to follow the orbital rotation of the menu, then calculate the rotation plus the rotation offset.
			if( followOrbitalRotation )
				newRotation = new Vector3( 0, 0, ( -angle * i ) + rotationOffset );

			// Apply the rotation to the button transform.
			UltimateRadialButtonList[ i ].buttonTransform.localRotation = Quaternion.Euler( newRotation );

			// -------------------------- < ICON POSITIONING > -------------------------- //
			if( useButtonIcon && UltimateRadialButtonList[ i ].icon != null )
			{
				// Store the positioning information so that it can modified if need be.
				float horizontalPos = iconHorizontalPosition;
				float verticalPos = iconVerticalPosition;
				float sizeMod = iconSize;
				float rotationMod = iconRotation;

				// If the user wants to use this icon with unique positioning...
				if( UltimateRadialButtonList[ i ].useIconUnique )
				{
					// Modify the positioning information with the unique information.
					horizontalPos = UltimateRadialButtonList[ i ].iconHorizontalPosition;
					verticalPos = UltimateRadialButtonList[ i ].iconVerticalPosition;
					sizeMod = UltimateRadialButtonList[ i ].iconSize;
					rotationMod = UltimateRadialButtonList[ i ].iconRotation;
				}

				// Configure the position for the icon.
				Vector2 iconPosition = Vector3.zero;
				iconPosition.x += ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x * ( horizontalPos / 100 ) ) - ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x / 2 );
				iconPosition.y += ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y * ( verticalPos / 100 ) ) - ( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y / 2 );

				// Apply the size and position to the icon.
				UltimateRadialButtonList[ i ].icon.rectTransform.sizeDelta = new Vector2( BaseTransform.sizeDelta.x * sizeMod, BaseTransform.sizeDelta.x * sizeMod ) * ( UltimateRadialButtonList[ i ].icon.sprite == null ? Vector2.one : GetImageAspectRatio( UltimateRadialButtonList[ i ].icon.sprite ) );
				UltimateRadialButtonList[ i ].icon.rectTransform.localPosition = iconPosition;

				// If the transform is unassigned, then assign it.
				if( UltimateRadialButtonList[ i ].iconTransform == null )
					UltimateRadialButtonList[ i ].iconTransform = UltimateRadialButtonList[ i ].icon.rectTransform;

				// Store the normal scale.
				UltimateRadialButtonList[ i ].iconNormalScale = UltimateRadialButtonList[ i ].invertScaleY ? new Vector3( 1, -1, 1 ) : new Vector3( 1, 1, 1 );
				UltimateRadialButtonList[ i ].iconTransform.localScale = UltimateRadialButtonList[ i ].buttonDisabled ? UltimateRadialButtonList[ i ].iconNormalScale * iconDisabledScaleModifier : UltimateRadialButtonList[ i ].iconNormalScale;

				// If the user wants to use local rotation then increase the rotation mod by the current button's rotation.
				if( iconLocalRotation )
				{
					// Store the image rotation.
					float imageRotation = UltimateRadialButtonList[ i ].radialImage.rectTransform.localRotation.eulerAngles.z;

					// If the rotation is less than zero then add 360 to get a positive number.
					if( imageRotation < 0 )
						imageRotation += 360;

					// If the rotation is more than 90 degrees and less than 270, then increase the rotation by 180 to flip the icon.
					if( imageRotation > 90 && imageRotation < 270 )
						rotationMod += 180;
				}
				// Else the user wants world space rotation so store the rotation modifier as the buttons rotation.
				else
					rotationMod = -UltimateRadialButtonList[ i ].buttonTransform.localRotation.eulerAngles.z + ( UltimateRadialButtonList[ i ].useIconUnique ? UltimateRadialButtonList[ i ].iconRotation : -iconRotation );

				// Apply the rotation to the icon.
				UltimateRadialButtonList[ i ].icon.rectTransform.localRotation = Quaternion.Euler( new Vector3( 0, 0, rotationMod ) );
			}

			// -------------------------- < TEXT POSITIONING > -------------------------- //
			if( UltimateRadialButtonList[ i ].text != null )
			{
				// Apply the size to the text transform.
				UltimateRadialButtonList[ i ].text.rectTransform.sizeDelta = new Vector2( BaseTransform.sizeDelta.x * textSize, BaseTransform.sizeDelta.x * textSize ) * new Vector2( textAreaRatioX, textAreaRatioY );

				// If the user wants to position the text in local position to the button...
				if( textLocalPosition )
				{
					// Store a vector2 for the text position.
					Vector2 textPosition = Vector2.zero;

					// Since the user might want to increase the are at which they can position the text, this Vector2 will be a larger area to position.
					Vector2 modifiedRefSizeForText = new Vector2( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x, UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y ) * 1.25f;

					if( relativeToIcon && UltimateRadialButtonList[ i ].iconTransform != null )
						modifiedRefSizeForText = new Vector2( UltimateRadialButtonList[ i ].iconTransform.sizeDelta.x, UltimateRadialButtonList[ i ].iconTransform.sizeDelta.y ) * 1.25f;

					textPosition.x += ( modifiedRefSizeForText.x * ( textHorizontalPosition / 100 ) ) - ( modifiedRefSizeForText.x / 2 );
					textPosition.y += ( modifiedRefSizeForText.y * ( textVerticalPosition / 100 ) ) - ( modifiedRefSizeForText.y / 2 );
					
					// Apply the local position to the text.
					UltimateRadialButtonList[ i ].text.rectTransform.localPosition = textPosition;

					// If the user wants the text to have a local rotation with the button...
					if( textLocalRotation )
					{
						// Store the image rotation.
						float imageRotation = UltimateRadialButtonList[ i ].radialImage.rectTransform.localRotation.eulerAngles.z;

						// If the rotation is less than zero then add 360 to get a positive number.
						if( imageRotation < 0 )
							imageRotation += 360;

						// If the rotation is more than 90 degrees and less than 270, then increase the rotation by 180 to flip the text so it is readable.
						if( imageRotation > 90 && imageRotation < 270 )
							UltimateRadialButtonList[ i ].text.rectTransform.localRotation = Quaternion.Euler( 0, 0, 180 );
						// Else just set the local rotation to zero.
						else
							UltimateRadialButtonList[ i ].text.rectTransform.localRotation = Quaternion.identity;
					}
					// Else the user does not want the rotation to be local, so apply the inverse rotation of the button.
					else
						UltimateRadialButtonList[ i ].text.rectTransform.localRotation = Quaternion.Euler( new Vector3( 0, 0, -UltimateRadialButtonList[ i ].buttonTransform.localRotation.eulerAngles.z ) );
				}
				// Else the text positioning is set to global...
				else
				{
					// Store the position of this button in world space. Everything will be calculated off of this.
					Vector3 buttonWorldPosition = Vector3.zero;
					
					// Modify that position by the options of the user.
					Vector2 modifiedRefSizeForText = ( new Vector2( UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.x, UltimateRadialButtonList[ i ].buttonTransform.sizeDelta.y ) ) * 1.25f;

					if( relativeToIcon && UltimateRadialButtonList[ i ].iconTransform != null )
						modifiedRefSizeForText = ( new Vector2( UltimateRadialButtonList[ i ].iconTransform.sizeDelta.x, UltimateRadialButtonList[ i ].iconTransform.sizeDelta.y ) ) * 1.25f;

					buttonWorldPosition.x += ( modifiedRefSizeForText.x * ( textHorizontalPosition / 100 ) ) - ( modifiedRefSizeForText.x / 2 );
					buttonWorldPosition.y += ( modifiedRefSizeForText.y * ( textVerticalPosition / 100 ) ) - ( modifiedRefSizeForText.y / 2 );

					// If this canvas is in world space...
					if( IsWorldSpaceRadialMenu )
					{
						// Convert the position from local to world space.
						buttonWorldPosition = BaseTransform.transform.TransformPoint( buttonWorldPosition );

						// Modify the calculated position by subtracting the base transforms position.
						buttonWorldPosition -= BaseTransform.position;
					}

					if( relativeToIcon && UltimateRadialButtonList[ i ].iconTransform != null )
						UltimateRadialButtonList[ i ].text.rectTransform.position = UltimateRadialButtonList[ i ].iconTransform.position + buttonWorldPosition;
					// Apply the position of the text to being the button position plus the calculated world position.
					else
						UltimateRadialButtonList[ i ].text.rectTransform.position = UltimateRadialButtonList[ i ].buttonTransform.position + buttonWorldPosition;

					// Set the rotation of the text to being the inverse rotation of the button.
					UltimateRadialButtonList[ i ].text.rectTransform.localRotation = Quaternion.Euler( 0, 0, -UltimateRadialButtonList[ i ].buttonTransform.localRotation.eulerAngles.z );
				}
			}

			// If the game is running and the user wants to scale transform, and if this button is currently selected then apply the position and scale.
			if( Application.isPlaying && scaleTransform && CurrentSelectedButtonIndex >= 0 && CurrentSelectedButtonIndex == i )
			{
				UltimateRadialButtonList[ i ].buttonTransform.localScale = Vector3.one * selectedScaleModifier;
				UltimateRadialButtonList[ i ].buttonTransform.localPosition = UltimateRadialButtonList[ i ].selectedPosition;
			}
		}

		// Reset the current button index since the menu has likely shifted in some way. This will allow the ProcessInput function to recalculate where the buttons are.
		CurrentButtonIndex = -1;
		
		// Apply the starting scale.
		BaseTransform.localScale = radialMenuToggle == RadialMenuToggle.Scale && !RadialMenuActive && Application.isPlaying ? Vector3.zero : Vector3.one;

		// Inform any subscribers that the update positioning function has run.
		if( OnUpdatePositioning != null )
			OnUpdatePositioning();
	}

	/// <summary>
	/// Register the provided information to the Ultimate Radial Menu.
	/// </summary>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no parameter is provided, the information will be registered to the first available button.</param>
	public void RegisterToRadialMenu ( Action ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// Find the actual index of the radial button depending on what the user passed as the buttonIndex parameter.
		FindRadialButtonIndex( ref buttonIndex );

		// Register the button information.
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	/// <summary>
	/// Register the provided information to the Ultimate Radial Menu.
	/// </summary>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no parameter is provided, the information will be registered to the first available button.</param>
	public void RegisterToRadialMenu ( Action<int> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// Find the actual index of the radial button depending on what the user passed as the buttonIndex parameter.
		FindRadialButtonIndex( ref buttonIndex );

		// Register the button information.
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	/// <summary>
	/// Register the provided information to the Ultimate Radial Menu.
	/// </summary>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no parameter is provided, the information will be registered to the first available button.</param>
	public void RegisterToRadialMenu ( Action<string> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// Find the actual index of the radial button depending on what the user passed as the buttonIndex parameter.
		FindRadialButtonIndex( ref buttonIndex );

		// Register the button information.
		UltimateRadialButtonList[ buttonIndex ].RegisterButtonInfo( buttonInfo );

		// Subscribe the ButtonCallback function to the OnRadialButtonInteract event.
		UltimateRadialButtonList[ buttonIndex ].AddCallback( ButtonCallback );
	}

	/// <summary>
	/// Enables the Ultimate Radial Menu so that it can be interacted with.
	/// </summary>
	public void EnableRadialMenu ()
	{
		// If the radial menu is already active, then return.
		if( RadialMenuActive )
			return;

		// Set the state to active.
		RadialMenuActive = true;

		// Depending on the options set by the user, start the correct coroutine.
		switch( radialMenuToggle )
		{
			default:
			case RadialMenuToggle.FadeAlpha:
			{
				StartCoroutine( FadeRadialMenu() );
			}
			break;
			case RadialMenuToggle.Scale:
			{
				StartCoroutine( ScaleRadialMenu() );
			}
			break;
		}

		// Notify any subscribers that the radial menu is now enabled.
		if( OnRadialMenuEnabled != null )
			OnRadialMenuEnabled();
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu so that it can not be interacted with.
	/// </summary>
	public void DisableRadialMenu ()
	{
		// If the radial menu is already disabled, then return.
		if( !RadialMenuActive )
			return;

		// Set the state to inactive.
		RadialMenuActive = false;

		// Reset the radial menu so that it is ready for the next time it is enabled.
		ResetRadialMenu();

		// If the transitioning coroutine is not currently running...
		if( !InTransition )
		{
			// Start the correct coroutine according to the users options.
			switch( radialMenuToggle )
			{
				default:
				case RadialMenuToggle.FadeAlpha:
				{
					StartCoroutine( FadeRadialMenu() );
				}
				break;
				case RadialMenuToggle.Scale:
				{
					StartCoroutine( ScaleRadialMenu() );
				}
				break;
			}
		}

		// Set the input in range to false, just so that if the input manager was disabled, then other scripts can know that the input is no longer in range.
		InputInRange = false;
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu immediately so that it can not be interacted with.
	/// </summary>
	public void DisableRadialMenuImmediate ()
	{
		// Set the state to inactive.
		RadialMenuActive = false;

		// Reset the radial menu so that it is ready.
		ResetRadialMenu();

		// According to the users options, apply the disabled state of the radial menu.
		switch( radialMenuToggle )
		{
			default:
			case RadialMenuToggle.FadeAlpha:
			{
				canvasGroup.alpha = 0.0f;
			}
			break;
			case RadialMenuToggle.Scale:
			{
				BaseTransform.localScale = Vector3.zero;
			}
			break;
		}

		// Notify any subscribers that the radial menu has been disabled.
		if( OnRadialMenuDisabled != null )
			OnRadialMenuDisabled();
	}

	/// <summary>
	/// Creates an empty radial button at the end of the radial menu.
	/// </summary>
	public void CreateEmptyRadialButton ()
	{
		// Call the created function with the index of 1000 so it's out of range.
		CreateRadialButtonAtIndex( 1000 );
	}

	/// <summary>
	/// Removes all of the radial buttons from the radial menu. If a value is provided for the buttonCount parameter, the radial menu will ensure that it does not remove more than that count of buttons.
	/// </summary>
	/// <param name="buttonCount">[Optional Parameter] Determines how many buttons the radial menu will leave on the menu.</param>
	public void RemoveAllRadialButtons ( int buttonCount = 0 )
	{
		// If the user has a style assigned, and the button count is less than the minimum button count for the style, set the button count to the minimum.
		if( radialMenuStyle != null && buttonCount < radialMenuStyle.minButtonCount )
			buttonCount = radialMenuStyle.minButtonCount;

		// Loop through the radial button list...
		for( int i = UltimateRadialButtonList.Count - 1; i >= 0; i-- )
		{
			// Clear button information.
			UltimateRadialButtonList[ i ].ClearButtonInformation();

			// If the index is greater than the minimum button count, then send this button index to the pool.
			if( i >= buttonCount )
				SendRadialButtonToPool( i );
		}

		// Since the button count has changed, call the RadialMenuButtonCountModified function.
		RadialMenuButtonCountModified();

		// Reset the current selected index.
		CurrentSelectedButtonIndex = -1;
	}

	/// <summary>
	/// Removes the radial button at the targeted index.
	/// </summary>
	/// <param name="buttonIndex">The index to remove the radial button at.</param>
	public void RemoveRadialButton ( int buttonIndex )
	{
		// If the list has no buttons, then just return because there is nothing to remove.
		if( UltimateRadialButtonList.Count <= 0 )
			return;

		// If the button index is greater than the count, then just set it to the max index.
		if( buttonIndex > UltimateRadialButtonList.Count )
			buttonIndex = UltimateRadialButtonList.Count - 1;
		
		// Clear the button information so that any button information will be notified that this button has been destroyed.
		UltimateRadialButtonList[ buttonIndex ].ClearButtonInformation();
		
		// If there is a style assigned and the targeted button count is less than the minimum count, then just set the button count to the minimum.
		if( radialMenuStyle != null && menuButtonCount - 1 < radialMenuStyle.minButtonCount )
			menuButtonCount = radialMenuStyle.minButtonCount;
		// Else the radial button is unassigned or the target button count is within range...
		else
		{
			// Send this button to the pool.
			SendRadialButtonToPool( buttonIndex );

			// If the selected button index is assigned...
			if( CurrentSelectedButtonIndex >= 0 )
			{
				// If the button index that was removed was the selected button, then reset the button index.
				if( buttonIndex == CurrentSelectedButtonIndex )
					CurrentSelectedButtonIndex = -10;
				// Else if the button index was lower that the selected button index, then modify the index to fit the new index of the button.
				else if( buttonIndex < CurrentSelectedButtonIndex )
					CurrentSelectedButtonIndex -= 1;
			}

			// Since the button count has changed, call the RadialMenuButtonCountModified function.
			RadialMenuButtonCountModified();
		}
	}
	
	/// <summary>
	/// Clears the registered button information.
	/// </summary>
	public void ClearRadialButtonInformations ()
	{
		// Loop through the list of radial buttons and clear the button information.
		for( int i = 0; i < UltimateRadialButtonList.Count; i++ )
			UltimateRadialButtonList[ i ].ClearButtonInformation();

		// Reset the radial menu.
		ResetRadialMenu();
		CurrentSelectedButtonIndex = -1;
	}

	/// <summary>
	/// Updates the radial menu's position to the new position either in the world or on the screen.
	/// </summary>
	/// <param name="position">The new position either on the screen, or in the world.</param>
	/// <param name="local">[OPTIONAL] Determines whether or not to apply the provided position to local space or not. Defaults to world space.</param>
	public void SetPosition ( Vector3 position, bool local = false )
	{
		// If the base transform is assigned...
		if( BaseTransform != null )
		{
			// If this menu is world space...
			if( IsWorldSpaceRadialMenu )
			{
				// If the parent canvas is unassigned...
				if( ParentCanvas == null )
				{
					// Try to find the parent canvas.
					UpdateParentCanvas();

					// If the parent canvas is still null, then notify the user and return.
					if( ParentCanvas == null )
					{
						Debug.LogError( "Ultimate Radial Menu\nThere is no parent canvas object. Please make sure that the Ultimate Radial Menu is placed within a canvas." );
						return;
					}
				}

				// Set the position of the parent canvas to the position that the user sent.
				ParentCanvas.GetComponent<RectTransform>().position = position;
			}
			// Else apply the position to the base transform.
			else
			{
				if( local )
					BaseTransform.localPosition = position;
				else
					BaseTransform.position = position;
			}
		}
	}

	/// <summary>
	/// Resets the position of the radial menu back to it's original position.
	/// </summary>
	public void ResetPosition ()
	{
		if( BaseTransform != null )
			BaseTransform.position = defaultPosition;
	}

	/// <summary>
	/// Sets the parent of the Canvas to the provided Transform. This is only suitable for World Space radial menus.
	/// </summary>
	/// <param name="parent">The new Transform to make the Canvas a child of.</param>
	/// <param name="localPosition">The local position of the radial menu.</param>
	/// <param name="localRotation">The local rotation of the radial menu.</param>
	public void SetParent ( Transform parent, Vector3 localPosition, Quaternion localRotation )
	{
		// If the parent canvas is unassigned...
		if( ParentCanvas == null )
		{
			// Try to find the parent canvas.
			UpdateParentCanvas();

			// If the parent canvas is still null, then notify the user and return.
			if( ParentCanvas == null )
			{
				Debug.LogError( "Ultimate Radial Menu\nThere is no parent canvas object. Please make sure that the Ultimate Radial Menu is placed within a canvas." );
				return;
			}
		}

		// Set the parent transform of the canvas.
		ParentCanvas.transform.SetParent( parent );

		// Set the local position and rotation.
		ParentCanvas.GetComponent<RectTransform>().localRotation = localRotation;
		ParentCanvas.GetComponent<RectTransform>().localPosition = localPosition;
	}
	// ------------------------------------------------ < END PUBLIC FUNCTIONS FOR THE USER > ------------------------------------------------ //

	/// <summary>
	/// Confirms the existence of the radial menu that has been registered with the targeted name.
	/// </summary>
	/// <param name="radialMenuName">The string name that the radial menu has been registered with.</param>
	static bool ConfirmUltimateRadialMenu ( string radialMenuName )
	{
		// If the static radial menu dictionary does not contain the targeted radial menu key, then inform the user and return false.
		if( !UltimateRadialMenus.ContainsKey( radialMenuName ) )
		{
			Debug.LogWarning( "Ultimate Radial Menu\nThere is no Ultimate Radial Menu registered with the name: " + radialMenuName + " in the scene." );
			return false;
		}
		return true;
	}

	// ----------------------------------------------------- < PUBLIC STATIC FUNCTIONS > ----------------------------------------------------- //
	/// <summary>
	/// Returns the radial menu that has been registered with the targeted radial menu name.
	/// </summary>
	/// <param name="radialMenuName">The string name that the radial menu has been registered with.</param>
	public static UltimateRadialMenu GetUltimateRadialMenu ( string radialMenuName )
	{
		// If the radial menu does not exist then return null.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return null;

		return UltimateRadialMenus[ radialMenuName ];
	}

	/// <summary>
	/// Registered the provided information to the targeted Ultimate Radial Menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no parameter is provided, the information will be registered to the first available button.</param>
	public static void RegisterToRadialMenu ( string radialMenuName, Action ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterToRadialMenu( ButtonCallback, buttonInfo, buttonIndex );
	}

	/// <summary>
	/// Registered the provided information to the targeted Ultimate Radial Menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no parameter is provided, the information will be registered to the first available button.</param>
	public static void RegisterToRadialMenu ( string radialMenuName, Action<int> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterToRadialMenu( ButtonCallback, buttonInfo, buttonIndex );
	}

	/// <summary>
	/// Registered the provided information to the targeted Ultimate Radial Menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="ButtonCallback">The function that will be called with the button is interacted with.</param>
	/// <param name="buttonInfo">The provided button information to apply to the radial button.</param>
	/// <param name="buttonIndex">[OPTIONAL] This parameter is optional and will determine where to register this information. If no parameter is provided, the information will be registered to the first available button.</param>
	public static void RegisterToRadialMenu ( string radialMenuName, Action<string> ButtonCallback, UltimateRadialButtonInfo buttonInfo, int buttonIndex = -1 )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return.
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RegisterToRadialMenu( ButtonCallback, buttonInfo, buttonIndex );
	}
	
	/// <summary>
	/// Enables the targeted Ultimate Radial Menu so that it can be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void EnableRadialMenu ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].EnableRadialMenu();
	}

	/// <summary>
	/// Disables the targeted Ultimate Radial Menu so that it can not be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void DisableRadialMenu ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].DisableRadialMenu();
	}

	/// <summary>
	/// Disables the Ultimate Radial Menu immediately so that it can not be interacted with.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void DisableRadialMenuImmediate ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].DisableRadialMenuImmediate();
	}

	/// <summary>
	/// Creates an empty radial button at the end of the targeted radial menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void CreateEmptyRadialButton ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].CreateEmptyRadialButton();
	}

	/// <summary>
	/// Deletes all of the radial menu buttons from the radial menu.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void RemoveAllRadialButtons ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RemoveAllRadialButtons();
	}

	/// <summary>
	/// Removes the radial button at the targeted index.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="buttonIndex">The index to remove the radial button at.</param>
	public static void RemoveRadialButton ( string radialMenuName, int buttonIndex )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].RemoveRadialButton( buttonIndex );
	}

	/// <summary>
	/// Clears the registered button information.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void ClearRadialButtonInformations ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].ClearRadialButtonInformations();
	}

	/// <summary>
	/// Updates the targeted radial menu's position to the new position on the screen.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="position">The new position on the screen or in the world.</param>
	/// <param name="local">[OPTIONAL] Determines whether or not to apply the provided position to local space or not. Defaults to world space.</param>
	public static void SetPosition ( string radialMenuName, Vector3 position, bool local = false )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		// Call the SetPosition function on the targeted radial menu.
		UltimateRadialMenus[ radialMenuName ].SetPosition( position, local );
	}

	/// <summary>
	/// Resets the position of the radial menu back to it's original position.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	public static void ResetPosition ( string radialMenuName )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].ResetPosition();
	}

	/// <summary>
	/// Sets the parent of the Canvas to the provided Transform. This is only suitable for World Space radial menus.
	/// </summary>
	/// <param name="radialMenuName">The string name that the targeted Ultimate Radial Menu has been registered with.</param>
	/// <param name="parent">The new Transform to make the Canvas a child of.</param>
	/// <param name="localPosition">The local position of the radial menu.</param>
	/// <param name="localRotation">The local rotation of the radial menu.</param>
	public static void SetParent ( string radialMenuName, Transform parent, Vector3 localPosition, Quaternion localRotation )
	{
		// If there is not a radial menu that has been registered with the targeted radialMenuName, then return. 
		if( !ConfirmUltimateRadialMenu( radialMenuName ) )
			return;

		UltimateRadialMenus[ radialMenuName ].SetParent( parent, localPosition, localRotation );
	}
	// --------------------------------------------------- < END PUBLIC STATIC FUNCTIONS > --------------------------------------------------- //
}