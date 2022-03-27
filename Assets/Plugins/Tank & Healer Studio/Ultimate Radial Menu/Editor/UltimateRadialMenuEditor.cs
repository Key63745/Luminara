/* UltimateRadialMenuEditor.cs */
/* Written by Kaz Crowe */
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[CustomEditor( typeof( UltimateRadialMenu ) )]
public class UltimateRadialMenuEditor : Editor
{
	UltimateRadialMenu targ;

	// GENERATE RADIAL MENU OPTIONS //
	SerializedProperty menuButtonCount;

	// RADIAL MENU POSITIONING //
	SerializedProperty scalingAxis, menuSize;
	SerializedProperty horizontalPosition, verticalPosition, depthPosition;
	SerializedProperty menuButtonSize, radialMenuButtonRadius;
	SerializedProperty angleOffset, followOrbitalRotation;
	SerializedProperty startingAngle;
	// Canvas Options //
	UnityEngine.Canvas parentCanvas;
	RectTransform parentCanvasRectTrans;
	float parentCanvasScale = 1.0f;
	Vector3 parentCanvasPosition = Vector3.zero;
	Vector3 parentCanvasRotation = Vector3.zero;
	Vector2 parentCanvasSizeDelta = Vector2.zero;
	// Input Settings //
	SerializedProperty minRange, maxRange;
	SerializedProperty infiniteMaxRange, buttonInputAngle;

	// RADIAL MENU SETTINGS //
	SerializedProperty radialMenuStyle, normalSprite, normalColor;
	// Menu Toggle //
	SerializedProperty initialState, radialMenuToggle, toggleInDuration, toggleOutDuration;
	// Text Options //
	Color nameTextColor = Color.white, nameTextOutlineColor = Color.white;
	SerializedProperty displayButtonName, nameText, nameFont, nameOutline;
	SerializedProperty nameTextRatioX, nameTextRatioY, nameTextSize, nameTextHorizontalPosition, nameTextVerticalPosition;
	Color descriptionTextColor = Color.white, descriptionTextOutlineColor = Color.white;
	SerializedProperty displayButtonDescription, descriptionText, descriptionFont, descriptionOutline;
	SerializedProperty descriptionTextRatioX, descriptionTextRatioY, descriptionTextSize, descriptionTextHorizontalPosition, descriptionTextVerticalPosition;
	// Button Icon //
	Sprite iconPlaceholderSprite;
	SerializedProperty useButtonIcon, iconNormalColor;
	SerializedProperty iconSize, iconHorizontalPosition, iconVerticalPosition, iconRotation, iconLocalRotation;
	// Button Text //
	SerializedProperty useButtonText, textNormalColor;
	SerializedProperty textAreaRatioX, textAreaRatioY, textSize, textHorizontalPosition, textVerticalPosition;
	SerializedProperty textLocalPosition, textLocalRotation, displayNameOnButton, buttonTextFont, buttonTextOutline, buttonTextOutlineColor;

	// BUTTON INTERACTION //
	SerializedProperty spriteSwap, colorChange,  scaleTransform;
	SerializedProperty iconColorChange, iconScaleTransform, textColorChange;
	// Highlighted //
	SerializedProperty highlightedSprite, highlightedColor, highlightedScaleModifier, positionModifier;
	SerializedProperty iconHighlightedColor, iconHighlightedScaleModifier, textHighlightedColor;
	// Pressed //
	SerializedProperty pressedSprite, pressedColor, pressedScaleModifier, pressedPositionModifier;
	SerializedProperty iconPressedColor, iconPressedScaleModifier, textPressedColor;
	// Selected //
	SerializedProperty selectedSprite, selectedColor, selectedScaleModifier, selectedPositionModifier, selectButtonOnInteract;
	SerializedProperty iconSelectedColor, iconSelectedScaleModifier, textSelectedColor;
	// Disabled //
	SerializedProperty disabledSprite, disabledColor, disabledScaleModifier, disabledPositionModifier;
	SerializedProperty iconDisabledColor, iconDisabledScaleModifier, textDisabledColor;

	// MENU BUTTON CUSTOMIZATION //
	static int radialNameListIndex = 0;
	List<string> buttonNames = new List<string>();
	List<SerializedProperty> buttonTransform;
	List<SerializedProperty> buttonName, buttonKey, buttonId, description;
	List<SerializedProperty> icon, text;
	List<Sprite> iconSprites;
	List<string> buttonText;
	List<SerializedProperty> rmbIconSize, rmbIconRotation;
	List<SerializedProperty> rmbIconHorizontalPosition, rmbIconVerticalPosition;
	List<SerializedProperty> useIconUnique, invertScaleY;
	List<SerializedProperty> buttonDisabled, unityEvent;

	// SCRIPT REFERENCE //
	bool RadialMenuNameAssigned, RadialMenuNameDuplicate, RadialMenuNameUnassigned;
	SerializedProperty radialMenuName;
	class ExampleCode
	{
		public string optionName = "";
		public string optionDescription = "";
		public string basicCode = "";
	}
	ExampleCode[] ExampleCodes = new ExampleCode[]
	{
		new ExampleCode() { optionName = "RegisterToRadialMenu", optionDescription = "Registers the provided information to the targeted radial menu.", basicCode = "UltimateRadialMenu.RegisterToRadialMenu( \"{0}\", YourCallbackFunction, newRadialButtonInfo );" },
		new ExampleCode() { optionName = "EnableRadialMenu", optionDescription = "Enables the radial menu visually.", basicCode = "UltimateRadialMenu.EnableRadialMenu( \"{0}\" );" },
		new ExampleCode() { optionName = "DisableRadialMenu", optionDescription = "Disables the radial menu visually.", basicCode = "UltimateRadialMenu.DisableRadialMenu( \"{0}\" );" },
		new ExampleCode() { optionName = "RemoveAllRadialButtons", optionDescription = "Removes all of the radial buttons on the menu.", basicCode = "UltimateRadialMenu.RemoveAllRadialButtons( \"{0}\" );" },
		new ExampleCode() { optionName = "RemoveRadialButton", optionDescription = "Removes the radial button at the targeted index.", basicCode = "UltimateRadialMenu.RemoveRadialButton( \"{0}\", {1} );" },
		new ExampleCode() { optionName = "ClearRadialButtonInformations", optionDescription = "Clears all of the registered information on the radial menu.", basicCode = "UltimateRadialMenu.ClearRadialButtonInformations( \"{0}\" );" },
		new ExampleCode() { optionName = "GetUltimateRadialMenu", optionDescription = "Returns the Ultimate Radial Menu component that is registered with the target name.", basicCode = "UltimateRadialMenu.GetUltimateRadialMenu( \"{0}\" );" },
	};
	List<string> exampleCodeOptions = new List<string>();
	int exampleCodeIndex = 0;

	// DEVELOPMENT MODE //
	bool showDefaultInspector = false;
	
	// SCENE GUI //
	class DisplaySceneGizmo
	{
		public int frames = maxFrames;
		public bool hover = false;

		public bool HighlightGizmo
		{
			get
			{
				return hover || frames < maxFrames;
			}
		}

		public void PropertyUpdated ()
		{
			frames = 0;
		}
	}
	DisplaySceneGizmo DisplayMinRange = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayMaxRange = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayInputAngle = new DisplaySceneGizmo();
	const int maxFrames = 200;
	// Gizmo Colors //
	Color colorDefault = Color.black;
	Color colorValueChanged = Color.cyan;
	Color colorButtonSelected = Color.yellow;
	Color colorButtonUnselected = Color.white;
	Color colorTextBox = Color.yellow;

	// EDITOR STYLES //
	GUIStyle collapsableSectionStyle = new GUIStyle();

	// MISC //
	bool prefabRootError = false;
	

	void OnEnable ()
	{
		StoreReferences();
		
		Undo.undoRedoPerformed += UndoRedoCallback;

		if( EditorPrefs.HasKey( "URM_ColorHexSetup" ) )
		{
			ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorDefaultHex" ), out colorDefault );
			ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorValueChangedHex" ), out colorValueChanged );
			ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorButtonSelectedHex" ), out colorButtonSelected );
			ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorButtonUnselectedHex" ), out colorButtonUnselected );
			ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorTextBoxHex" ), out colorTextBox );
		}

		prefabRootError = false;
		if( PrefabUtility.GetPrefabAssetType( targ.gameObject ) != PrefabAssetType.NotAPrefab )
		{
			if( PrefabUtility.GetOutermostPrefabInstanceRoot( targ.gameObject ) != targ.gameObject )
			{
				if( PrefabUtility.GetOutermostPrefabInstanceRoot( targ.gameObject ) != null )
					prefabRootError = true;
			}
			else if( PrefabUtility.GetOutermostPrefabInstanceRoot( targ.gameObject ) == targ.gameObject )
				PrefabUtility.UnpackPrefabInstance( targ.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction );
		}
	}

	void OnDisable ()
	{
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}
	
	void UndoRedoCallback ()
	{
		StoreReferences();
	}

	void StoreReferences ()
	{
		targ = ( UltimateRadialMenu )target;

		if( targ == null )
			return;
		
		CheckForParentCanvas();
		
		if( radialNameListIndex >= targ.UltimateRadialButtonList.Count )
			radialNameListIndex = 0;
		
		// GENERATE RADIAL MENU OPTIONS //
		menuButtonCount = serializedObject.FindProperty( "menuButtonCount" );
		followOrbitalRotation = serializedObject.FindProperty( "followOrbitalRotation" );
		startingAngle = serializedObject.FindProperty( "startingAngle" );

		// RADIAL MENU POSITIONING //
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		menuSize = serializedObject.FindProperty( "menuSize" );
		horizontalPosition = serializedObject.FindProperty( "horizontalPosition" );
		verticalPosition = serializedObject.FindProperty( "verticalPosition" );
		depthPosition = serializedObject.FindProperty( "depthPosition" );
		menuButtonSize = serializedObject.FindProperty( "menuButtonSize" );
		radialMenuButtonRadius = serializedObject.FindProperty( "radialMenuButtonRadius" );
		angleOffset = serializedObject.FindProperty( "angleOffset" );
		// Canvas Options //
		if( parentCanvas != null )
		{
			parentCanvasRectTrans = parentCanvas.GetComponent<RectTransform>();
			parentCanvasScale = parentCanvasRectTrans.localScale.x;
			parentCanvasPosition = parentCanvasRectTrans.position;
			parentCanvasRotation = parentCanvasRectTrans.eulerAngles;
			parentCanvasSizeDelta = parentCanvasRectTrans.sizeDelta;
		}
		// Input Settings //
		minRange = serializedObject.FindProperty( "minRange" );
		maxRange = serializedObject.FindProperty( "maxRange" );
		infiniteMaxRange = serializedObject.FindProperty( "infiniteMaxRange" );
		buttonInputAngle = serializedObject.FindProperty( "buttonInputAngle" );

		// RADIAL MENU OPTIONS //
		radialMenuStyle = serializedObject.FindProperty( "radialMenuStyle" );
		normalSprite = serializedObject.FindProperty( "normalSprite" );
		normalColor = serializedObject.FindProperty( "normalColor" );
		// Menu Toggle //
		initialState = serializedObject.FindProperty( "initialState" );
		radialMenuToggle = serializedObject.FindProperty( "radialMenuToggle" );
		toggleInDuration = serializedObject.FindProperty( "toggleInDuration" );
		toggleOutDuration = serializedObject.FindProperty( "toggleOutDuration" );
		// Menu Text //
		displayButtonName = serializedObject.FindProperty( "displayButtonName" );
		nameText = serializedObject.FindProperty( "nameText" );
		if( targ.nameText != null )
			nameTextColor = targ.nameText.color;
		nameFont = serializedObject.FindProperty( "nameFont" );
		if( targ.nameFont == null )
		{
			serializedObject.FindProperty( "nameFont" ).objectReferenceValue = Resources.GetBuiltinResource<Font>( "Arial.ttf" );
			serializedObject.ApplyModifiedProperties();
		}
		nameOutline = serializedObject.FindProperty( "nameOutline" );
		if( targ.nameText != null && targ.nameText.GetComponent<UnityEngine.UI.Outline>() )
			nameTextOutlineColor = targ.nameText.GetComponent<UnityEngine.UI.Outline>().effectColor;
		nameTextRatioX = serializedObject.FindProperty( "nameTextRatioX" );
		nameTextRatioY = serializedObject.FindProperty( "nameTextRatioY" );
		nameTextSize = serializedObject.FindProperty( "nameTextSize" );
		nameTextHorizontalPosition = serializedObject.FindProperty( "nameTextHorizontalPosition" );
		nameTextVerticalPosition = serializedObject.FindProperty( "nameTextVerticalPosition" );
		displayButtonDescription = serializedObject.FindProperty( "displayButtonDescription" );
		descriptionText = serializedObject.FindProperty( "descriptionText" );
		if( targ.descriptionText != null )
			descriptionTextColor = targ.descriptionText.color;
		descriptionFont = serializedObject.FindProperty( "descriptionFont" );
		if( targ.descriptionFont == null )
		{
			serializedObject.FindProperty( "descriptionFont" ).objectReferenceValue = Resources.GetBuiltinResource<Font>( "Arial.ttf" );
			serializedObject.ApplyModifiedProperties();
		}
		descriptionOutline = serializedObject.FindProperty( "descriptionOutline" );
		if( targ.descriptionText != null && targ.descriptionText.GetComponent<UnityEngine.UI.Outline>() )
			descriptionTextOutlineColor = targ.descriptionText.GetComponent<UnityEngine.UI.Outline>().effectColor;
		descriptionTextRatioX = serializedObject.FindProperty( "descriptionTextRatioX" );
		descriptionTextRatioY = serializedObject.FindProperty( "descriptionTextRatioY" );
		descriptionTextSize = serializedObject.FindProperty( "descriptionTextSize" );
		descriptionTextHorizontalPosition = serializedObject.FindProperty( "descriptionTextHorizontalPosition" );
		descriptionTextVerticalPosition = serializedObject.FindProperty( "descriptionTextVerticalPosition" );
		// Button Icon //
		useButtonIcon = serializedObject.FindProperty( "useButtonIcon" );
		iconSize = serializedObject.FindProperty( "iconSize" );
		iconHorizontalPosition = serializedObject.FindProperty( "iconHorizontalPosition" );
		iconVerticalPosition = serializedObject.FindProperty( "iconVerticalPosition" );
		iconRotation = serializedObject.FindProperty( "iconRotation" );
		iconLocalRotation = serializedObject.FindProperty( "iconLocalRotation" );
		iconNormalColor = serializedObject.FindProperty( "iconNormalColor" );
		// Button Text //
		useButtonText = serializedObject.FindProperty( "useButtonText" );
		textLocalRotation = serializedObject.FindProperty( "textLocalRotation" );
		buttonTextFont = serializedObject.FindProperty( "buttonTextFont" );
		if( targ.buttonTextFont == null )
		{
			serializedObject.FindProperty( "buttonTextFont" ).objectReferenceValue = Resources.GetBuiltinResource<Font>( "Arial.ttf" );
			serializedObject.ApplyModifiedProperties();
		}
		buttonTextOutline = serializedObject.FindProperty( "buttonTextOutline" );
		buttonTextOutlineColor = serializedObject.FindProperty( "buttonTextOutlineColor" );
		textNormalColor = serializedObject.FindProperty( "textNormalColor" );
		textSize = serializedObject.FindProperty( "textSize" );
		textHorizontalPosition = serializedObject.FindProperty( "textHorizontalPosition" );
		textVerticalPosition = serializedObject.FindProperty( "textVerticalPosition" );
		textAreaRatioX = serializedObject.FindProperty( "textAreaRatioX" );
		textAreaRatioY = serializedObject.FindProperty( "textAreaRatioY" );
		textLocalPosition = serializedObject.FindProperty( "textLocalPosition" );
		displayNameOnButton = serializedObject.FindProperty( "displayNameOnButton" );

		// BUTTON INTERACTION //
		spriteSwap = serializedObject.FindProperty( "spriteSwap" );
		colorChange = serializedObject.FindProperty( "colorChange" );
		scaleTransform = serializedObject.FindProperty( "scaleTransform" );
		iconColorChange = serializedObject.FindProperty( "iconColorChange" );
		iconScaleTransform = serializedObject.FindProperty( "iconScaleTransform" );
		textColorChange = serializedObject.FindProperty( "textColorChange" );
		// Highlighted //
		highlightedSprite = serializedObject.FindProperty( "highlightedSprite" );
		highlightedColor = serializedObject.FindProperty( "highlightedColor" );
		highlightedScaleModifier = serializedObject.FindProperty( "highlightedScaleModifier" );
		positionModifier = serializedObject.FindProperty( "positionModifier" );
		iconHighlightedColor = serializedObject.FindProperty( "iconHighlightedColor" );
		iconHighlightedScaleModifier = serializedObject.FindProperty( "iconHighlightedScaleModifier" );
		textHighlightedColor = serializedObject.FindProperty( "textHighlightedColor" );
		// Pressed //
		pressedSprite = serializedObject.FindProperty( "pressedSprite" );
		pressedColor = serializedObject.FindProperty( "pressedColor" );
		pressedScaleModifier = serializedObject.FindProperty( "pressedScaleModifier" );
		pressedPositionModifier = serializedObject.FindProperty( "pressedPositionModifier" );
		iconPressedColor = serializedObject.FindProperty( "iconPressedColor" );
		iconPressedScaleModifier = serializedObject.FindProperty( "iconPressedScaleModifier" );
		textPressedColor = serializedObject.FindProperty( "textPressedColor" );
		// Selected //
		selectedSprite = serializedObject.FindProperty( "selectedSprite" );
		selectedColor = serializedObject.FindProperty( "selectedColor" );
		selectedScaleModifier = serializedObject.FindProperty( "selectedScaleModifier" );
		selectedPositionModifier = serializedObject.FindProperty( "selectedPositionModifier" );
		selectButtonOnInteract = serializedObject.FindProperty( "selectButtonOnInteract" );
		iconSelectedColor = serializedObject.FindProperty( "iconSelectedColor" );
		iconSelectedScaleModifier = serializedObject.FindProperty( "iconSelectedScaleModifier" );
		textSelectedColor = serializedObject.FindProperty( "textSelectedColor" );
		// Disabled //
		disabledSprite = serializedObject.FindProperty( "disabledSprite" );
		disabledColor = serializedObject.FindProperty( "disabledColor" );
		disabledScaleModifier = serializedObject.FindProperty( "disabledScaleModifier" );
		disabledPositionModifier = serializedObject.FindProperty( "disabledPositionModifier" );
		iconDisabledColor = serializedObject.FindProperty( "iconDisabledColor" );
		iconDisabledScaleModifier = serializedObject.FindProperty( "iconDisabledScaleModifier" );
		textDisabledColor = serializedObject.FindProperty( "textDisabledColor" );

		// RADIAL BUTTON LIST //
		buttonTransform = new List<SerializedProperty>();
		buttonName = new List<SerializedProperty>();
		buttonKey = new List<SerializedProperty>();
		buttonId = new List<SerializedProperty>();
		description = new List<SerializedProperty>();
		icon = new List<SerializedProperty>();
		iconSprites = new List<Sprite>();
		buttonText = new List<string>();
		rmbIconSize = new List<SerializedProperty>();
		rmbIconHorizontalPosition = new List<SerializedProperty>();
		rmbIconVerticalPosition = new List<SerializedProperty>();
		rmbIconRotation = new List<SerializedProperty>();
		useIconUnique = new List<SerializedProperty>();
		invertScaleY = new List<SerializedProperty>();
		text = new List<SerializedProperty>();
		buttonDisabled = new List<SerializedProperty>();
		unityEvent = new List<SerializedProperty>();
		buttonNames = new List<string>();

		for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
		{
			buttonNames.Add( "Radial Button " + i.ToString( "00" ) );
			if( targ.UltimateRadialButtonList[ i ].buttonTransform.name.Contains( "Radial" ) )
				targ.UltimateRadialButtonList[ i ].buttonTransform.name = "Radial Button " + i.ToString( "00" );

			if( i > 0 )
				targ.UltimateRadialButtonList[ i ].buttonTransform.SetSiblingIndex( targ.UltimateRadialButtonList[ i - 1 ].buttonTransform.GetSiblingIndex() + 1 );

			buttonTransform.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonTransform", i ) ) );
			buttonName.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].name", i ) ) );
			buttonKey.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].key", i ) ) );
			buttonId.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].id", i ) ) );
			description.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].description", i ) ) );

			icon.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].icon", i ) ) );
			iconSprites.Add( targ.UltimateRadialButtonList[ i ].icon != null ? targ.UltimateRadialButtonList[ i ].icon.sprite : null );
			buttonText.Add( targ.UltimateRadialButtonList[ i ].text != null ? targ.UltimateRadialButtonList[ i ].text.text : string.Empty );
			rmbIconSize.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconSize", i ) ) );
			rmbIconHorizontalPosition.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconHorizontalPosition", i ) ) );
			useIconUnique.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useIconUnique", i ) ) );
			invertScaleY.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].invertScaleY", i ) ) );
			rmbIconVerticalPosition.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconVerticalPosition", i ) ) );
			rmbIconRotation.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconRotation", i ) ) );

			text.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].text", i ) ) );
			buttonDisabled.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonDisabled", i ) ) );
			unityEvent.Add( serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].unityEvent", i ) ) );

			if( targ.useButtonIcon && targ.UltimateRadialButtonList[ i ].icon == null && icon[ i ] != null )
			{
				Image[] possibleImages = targ.UltimateRadialButtonList[ i ].buttonTransform.gameObject.GetComponentsInChildren<Image>();
				for( int n = 0; n < possibleImages.Length; n++ )
				{
					if( possibleImages[ n ] == targ.UltimateRadialButtonList[ i ].radialImage )
						continue;

					if( possibleImages[ n ].name != "Icon" )
						continue;

					icon[ i ].objectReferenceValue = possibleImages[ n ];
					serializedObject.ApplyModifiedProperties();
				}
			}
			if( targ.useButtonText && targ.UltimateRadialButtonList[ i ].text == null && text[ i ] != null )
			{
				Text childText = targ.UltimateRadialButtonList[ i ].buttonTransform.gameObject.GetComponentInChildren<Text>();

				if( childText != null )
				{
					text[ i ].objectReferenceValue = childText;
					serializedObject.ApplyModifiedProperties();
				}
			}
		}

		// SCRIPT REFERENCE //
		RadialMenuNameDuplicate = DuplicateRadialMenuName();
		RadialMenuNameUnassigned = targ.radialMenuName == string.Empty;
		RadialMenuNameAssigned = RadialMenuNameDuplicate == false && targ.radialMenuName != string.Empty;
		radialMenuName = serializedObject.FindProperty( "radialMenuName" );
		exampleCodeOptions = new List<string>();

		for( int i = 0; i < ExampleCodes.Length; i++ )
			exampleCodeOptions.Add( ExampleCodes[ i ].optionName );

		if( FindObjectOfType<EventSystem>() && !FindObjectOfType<EventSystem>().GetComponent<UltimateRadialMenuInputManager>() )
			FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();
	}

	bool DisplayHeaderDropdown ( string headerName, string editorPref )
	{
		EditorGUILayout.Space();

		GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11 };
		GUILayout.BeginHorizontal();
		GUILayout.Space( -10 );
		EditorPrefs.SetBool( editorPref, GUILayout.Toggle( EditorPrefs.GetBool( editorPref ), ( EditorPrefs.GetBool( editorPref ) ? "▼" : "►" ) + headerName, toolbarStyle ) );
		GUILayout.EndHorizontal();

		if( EditorPrefs.GetBool( editorPref ) )
		{
			EditorGUILayout.Space();
			return true;
		}
		return false;
	}

	bool DisplayCollapsibleBoxSection ( string sectionTitle, string editorPref )
	{
		if( EditorPrefs.GetBool( editorPref ) )
			collapsableSectionStyle.fontStyle = FontStyle.Bold;

		if( GUILayout.Button( sectionTitle, collapsableSectionStyle ) )
			EditorPrefs.SetBool( editorPref, !EditorPrefs.GetBool( editorPref ) );

		if( EditorPrefs.GetBool( editorPref ) )
			collapsableSectionStyle.fontStyle = FontStyle.Normal;

		return EditorPrefs.GetBool( editorPref );
	}

	bool DisplayCollapsibleBoxSection ( string sectionTitle, string editorPref, SerializedProperty enabledProp, ref bool valueChanged )
	{
		if( EditorPrefs.GetBool( editorPref ) && enabledProp.boolValue )
			collapsableSectionStyle.fontStyle = FontStyle.Bold;

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();
		enabledProp.boolValue = EditorGUILayout.Toggle( enabledProp.boolValue, GUILayout.Width( 25 ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			if( enabledProp.boolValue )
				EditorPrefs.SetBool( editorPref, true );
			else
				EditorPrefs.SetBool( editorPref, false );

			valueChanged = true;
		}

		GUILayout.Space( -25 );

		EditorGUI.BeginDisabledGroup( !enabledProp.boolValue );
		if( GUILayout.Button( sectionTitle, collapsableSectionStyle ) )
			EditorPrefs.SetBool( editorPref, !EditorPrefs.GetBool( editorPref ) );
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.EndHorizontal();

		if( EditorPrefs.GetBool( editorPref ) )
			collapsableSectionStyle.fontStyle = FontStyle.Normal;

		return EditorPrefs.GetBool( editorPref ) && enabledProp.boolValue;
	}

	void CheckPropertyHover ( DisplaySceneGizmo displaySceneGizmo )
	{
		displaySceneGizmo.hover = false;
		var rect = GUILayoutUtility.GetLastRect();
		if( Event.current.type == EventType.Repaint && rect.Contains( Event.current.mousePosition ) )
			displaySceneGizmo.hover = true;
	}

	void GUILayoutAfterIndentSpace ()
	{
		GUILayout.Space( 2 );
	}

	public override void OnInspectorGUI ()
	{
		if( Application.isPlaying && targ.UltimateRadialButtonList.Count == 0 )
		{
			EditorGUILayout.HelpBox( "The Radial Menu Button List has been cleared and there are no radial buttons present.", MessageType.Error );
			return;
		}

		serializedObject.Update();

		collapsableSectionStyle = new GUIStyle( EditorStyles.label ) { alignment = TextAnchor.MiddleCenter, onActive = new GUIStyleState() { textColor = Color.black } };
		collapsableSectionStyle.active.textColor = collapsableSectionStyle.normal.textColor;

		bool valueChanged = false;

		if( targ.UltimateRadialButtonList.Count == 0 )
		{
			EditorGUILayout.BeginVertical( "Box" );

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( menuButtonCount, new GUIContent( "Menu Button Count", "The amount of menu buttons in this radial menu." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( menuButtonCount.intValue < 2 )
					menuButtonCount.intValue = 2;

				serializedObject.ApplyModifiedProperties();
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( radialMenuStyle, new GUIContent( "Radial Menu Style", "The style to be used for the radial buttons as the menu changes the count of buttons." ) );
			if( targ.radialMenuStyle != null )
			{
				GUIStyle noteStyle = new GUIStyle( EditorStyles.miniLabel ) { alignment = TextAnchor.MiddleLeft, richText = true, wordWrap = true };
				EditorGUILayout.LabelField( "<color=red>*</color> Button sprite driven from assigned style.", noteStyle );
			}
			else
				EditorGUILayout.PropertyField( normalSprite, new GUIContent( "Normal Sprite", "The sprite to be applied to each radial button." ) );

			EditorGUILayout.PropertyField( followOrbitalRotation, new GUIContent( "Follow Orbital Rotation", "Determines whether or not the buttons should follow the rotation of the menu." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( GUILayout.Button( "Generate" ) )
			{
				if( targ.radialMenuStyle != null )
					menuButtonSize.floatValue = 1.0f;

				useButtonIcon.boolValue = false;
				useButtonText.boolValue = false;
				serializedObject.ApplyModifiedProperties();

				UpdateRadialButtonStyle();

				if( targ.normalSprite == null )
				{
					if( EditorUtility.DisplayDialog( "Ultimate Radial Menu", "You are about to create a radial menu with no assigned sprite.", "Continue", "Cancel" ) )
						GenerateRadialImages();
				}
				else
					GenerateRadialImages();
			}

			EditorGUILayout.EndVertical();
			Repaint();
			return;
		}
		
		if( prefabRootError )
		{
			EditorGUILayout.HelpBox( "The Ultimate Radial Menu is not the root of this prefab and therefore cannot be unpacked properly.\n\nThis can cause some strange behavior, as well as not being able to remove buttons in the editor that are part of the prefab.\n\nThis is caused because of Unity's new prefab manager. Please remove the Ultimate Radial Menu from the prefab in order to edit it.", MessageType.Error );
		}

		if( DisplayHeaderDropdown( "Radial Menu Positioning", "URM_RadialMenuPositioning" ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( scalingAxis, new GUIContent( "Scaling Axis", "Determines whether the Ultimate Radial Menu is sized according to Screen Height or Screen Width." ) );
			EditorGUILayout.Slider( menuSize, 0.0f, 10.0f, new GUIContent( "Menu Size", "Determines the overall size of the radial menu." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider( horizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the radial menu." ) );
			EditorGUILayout.Slider( verticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the radial menu." ) );
			if( targ.IsWorldSpaceRadialMenu )
				EditorGUILayout.PropertyField( depthPosition, new GUIContent( "Depth Position", "The depth of the radial menu." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider( radialMenuButtonRadius, 0.0f, 1.5f, new GUIContent( "Button Radius", "The distance that the buttons will be from the center of the menu." ) );
			EditorGUILayout.Slider( menuButtonSize, 0.0f, 1.0f, new GUIContent( "Button Size", "The size of the radial buttons." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( angleOffset, new GUIContent( "Angle Offset", "Determines how the first button should be positioned at the top of the menu." ) );
			EditorGUILayout.PropertyField( followOrbitalRotation, new GUIContent( "Orbital Rotation", "Determines whether or not the buttons should follow the rotation of the menu." ) );
			// EDIT: NEW
			EditorGUILayout.Slider( startingAngle, 0.0f, 360.0f, new GUIContent( "Starting Angle", "The angle for the first radial button in the list." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace )
			{
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Canvas Options", "URM_CanvasOptions" ) )
				{
					EditorGUI.BeginChangeCheck();
					parentCanvasScale = EditorGUILayout.Slider( new GUIContent( "Canvas Scale", "The scale of the canvas rect transform." ), parentCanvasScale, 0.0f, 1.0f );
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject( parentCanvas.GetComponent<RectTransform>(), "Change Canvas Scale" );
						parentCanvas.GetComponent<RectTransform>().localScale = Vector3.one * parentCanvasScale;
					}

					EditorGUI.BeginChangeCheck();
					parentCanvasPosition = EditorGUILayout.Vector3Field( new GUIContent( "Canvas Position", "The position of the canvas rect transform." ), parentCanvasPosition );
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject( parentCanvas.GetComponent<RectTransform>(), "Change Canvas Position" );
						parentCanvas.GetComponent<RectTransform>().position = parentCanvasPosition;
					}

					EditorGUI.BeginChangeCheck();
					parentCanvasSizeDelta = EditorGUILayout.Vector2Field( new GUIContent( "Canvas Size Delta", "The size delta of the canvas rect transform." ), parentCanvasSizeDelta );
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject( parentCanvas.GetComponent<RectTransform>(), "Change Canvas Size Delta" );
						parentCanvas.GetComponent<RectTransform>().sizeDelta = parentCanvasSizeDelta;
					}

					EditorGUI.BeginChangeCheck();
					parentCanvasRotation = EditorGUILayout.Vector3Field( new GUIContent( "Canvas Rotation", "The rotation of the canvas rect transform." ), parentCanvasRotation );
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject( parentCanvas.GetComponent<RectTransform>(), "Change Canvas Rotation" );
						parentCanvas.GetComponent<RectTransform>().rotation = Quaternion.Euler( parentCanvasRotation );
					}
				}
				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Input Settings", "URM_Input Settings" ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( minRange, 0.0f, targ.maxRange, new GUIContent( "Minimum Range", "The minimum range that will affect the radial menu." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					DisplayMinRange.frames = 0;
					serializedObject.ApplyModifiedProperties();
				}
				CheckPropertyHover( DisplayMinRange );

				EditorGUI.BeginDisabledGroup( targ.infiniteMaxRange );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( maxRange, targ.minRange, 1.5f, new GUIContent( "Maximum Range", "The maximum range that will affect the radial menu." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					DisplayMaxRange.frames = 0;
					serializedObject.ApplyModifiedProperties();
				}
				CheckPropertyHover( DisplayMaxRange );
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( infiniteMaxRange, new GUIContent( "Infinite Max Range", "Determines whether or not the maximum range should be calculated as infinite." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( buttonInputAngle, 0.0f, 1.0f, new GUIContent( "Input Angle", "Determines how much of the angle should be used for input." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					DisplayInputAngle.frames = 0;
					serializedObject.ApplyModifiedProperties();
				}
				CheckPropertyHover( DisplayInputAngle );

				if( GUILayout.Button( "Select Input Manager", EditorStyles.miniButton ) )
				{
					Selection.activeGameObject = FindObjectOfType<UltimateRadialMenuInputManager>().gameObject;
					EditorGUIUtility.PingObject( Selection.activeGameObject );
				}
				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
		}

		if( DisplayHeaderDropdown( "Radial Menu Options", "URM_RadialMenuOptions" ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( radialMenuStyle, new GUIContent( "Radial Button Style", "The style to be used for the radial buttons as the menu changes the count of buttons." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				UpdateRadialButtonStyle();
			}

			if( targ.radialMenuStyle != null )
			{
				GUIStyle noteStyle = new GUIStyle( EditorStyles.miniLabel ) { alignment = TextAnchor.MiddleLeft, richText = true, wordWrap = true };
				EditorGUILayout.LabelField( "<color=red>*</color> This style determines button sprites.", noteStyle );
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( normalSprite, new GUIContent( "Radial Button Sprite", "The default sprite to apply to the radial button image." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					if( normalSprite.objectReferenceValue == null )
					{
						spriteSwap.boolValue = false;
						colorChange.boolValue = false;
					}
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Radial Button Sprite" );
						if( targ.normalSprite != null )
						{
							if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.spriteSwap )
								targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.normalSprite;

							if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.colorChange )
								targ.UltimateRadialButtonList[ i ].radialImage.color = normalColor.colorValue;
						}
						else
						{
							if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.spriteSwap )
								targ.UltimateRadialButtonList[ i ].radialImage.sprite = null;

							if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.colorChange )
								targ.UltimateRadialButtonList[ i ].radialImage.color = Color.clear;
						}
						
						// This is added just in case the user has not broken the prefab, at least we can keep the sprites up to date.
						if( prefabRootError )
							PrefabUtility.RecordPrefabInstancePropertyModifications( targ.UltimateRadialButtonList[ i ].radialImage );
					}
				}
			}

			EditorGUI.BeginDisabledGroup( targ.normalSprite == null );
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( normalColor, new GUIContent( "Radial Button Color", "The default color to apply to the radial button image." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				if( targ.normalSprite != null )
				{
					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].radialImage.sprite == null )
							continue;

						if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.colorChange )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Radial Button Color" );
							targ.UltimateRadialButtonList[ i ].radialImage.color = targ.normalColor;
						}
					}
				}
			}
			EditorGUI.EndDisabledGroup();
			
			// MENU TOGGLE SETTINGS //
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Menu Toggle", "URM_MenuToggleSettings" ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( initialState, new GUIContent( "Initial State", "The initial state of the radial menu, either enabled (visible) or disabled (invisible)." ) );
				EditorGUILayout.PropertyField( radialMenuToggle, new GUIContent( "Radial Menu Toggle", "Determines how the radial menu will toggle it's state, either by fading the alpha of a Canvas Group component or scaling the transform." ) );
				EditorGUILayout.PropertyField( toggleInDuration, new GUIContent( targ.radialMenuToggle == UltimateRadialMenu.RadialMenuToggle.FadeAlpha ? "Fade In Duration" : "Scale In Duration", "The time in seconds to enable the radial menu." ) );
				EditorGUILayout.PropertyField( toggleOutDuration, new GUIContent( targ.radialMenuToggle == UltimateRadialMenu.RadialMenuToggle.FadeAlpha ? "Fade Out Duration" : "Scale Out Duration", "The time in seconds to disable the radial menu." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
			// END MENU TOGGLE SETTINGS //

			// MENU TEXT //
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Menu Text", "URM_MenuText" ) )
			{
				EditorGUI.BeginChangeCheck();
				displayButtonName.boolValue = EditorGUILayout.ToggleLeft( new GUIContent( "Display Name", "Determines if the radial menu should have a text component that will display the name of the currently selected button." ), displayButtonName.boolValue );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					if( targ.nameText != null )
						targ.nameText.gameObject.SetActive( displayButtonName.boolValue );
				}

				if( targ.displayButtonName )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( nameText, new GUIContent( "Name Text", "The text component to be used for the name." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( nameFont, new GUIContent( "Font", "The font to use on the name text." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						if( targ.nameText != null )
						{
							Undo.RecordObject( targ.nameText, "Update Name Font" );
							targ.nameText.font = targ.nameFont;
						}
					}

					if( targ.displayButtonName && targ.nameText == null )
					{
						EditorGUILayout.HelpBox( "There is no text component assigned.", MessageType.Warning );
						if( GUILayout.Button( "Generate Name Text", EditorStyles.miniButton ) )
						{
							nameOutline.boolValue = false;

							GameObject newText = new GameObject();
							RectTransform textTransform = newText.AddComponent<RectTransform>();
							newText.AddComponent<CanvasRenderer>();
							Text textComponent = newText.AddComponent<Text>();
							newText.name = "Name Text";

							newText.transform.SetParent( targ.transform );
							textTransform.position = targ.GetComponent<RectTransform>().position;
							textTransform.pivot = new Vector2( 0.5f, 0.5f );
							textTransform.localScale = Vector3.one;
							textComponent.text = "Name Text";
							textComponent.resizeTextForBestFit = true;
							textComponent.resizeTextMinSize = 0;
							textComponent.resizeTextMaxSize = 300;
							textComponent.alignment = TextAnchor.MiddleCenter;
							textComponent.color = nameTextColor;
							if( targ.nameFont != null )
								textComponent.font = targ.nameFont;

							nameText.objectReferenceValue = newText;
							serializedObject.ApplyModifiedProperties();

							Undo.RegisterCreatedObjectUndo( newText, "Create Text Object" );
						}
					}
					else if( targ.nameText != null )
					{
						EditorGUI.BeginChangeCheck();
						nameTextColor = EditorGUILayout.ColorField( "Text Color", nameTextColor );
						if( EditorGUI.EndChangeCheck() )
						{
							if( targ.nameText != null )
							{
								Undo.RecordObject( targ.nameText, "Update Name Text Color" );
								targ.nameText.enabled = false;
								targ.nameText.color = nameTextColor;
								targ.nameText.enabled = true;
							}
						}

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( nameOutline, new GUIContent( "Text Outline", "Determines if the text should have an outline or not." ) );
						if( EditorGUI.EndChangeCheck() )
						{
							serializedObject.ApplyModifiedProperties();

							if( targ.nameText != null )
							{
								if( targ.nameOutline && !targ.nameText.gameObject.GetComponent<UnityEngine.UI.Outline>() )
								{
									Undo.RecordObject( targ.nameText.gameObject, "Update Text Outline" );
									targ.nameText.gameObject.AddComponent<UnityEngine.UI.Outline>();

									nameTextOutlineColor = targ.nameText.gameObject.GetComponent<UnityEngine.UI.Outline>().effectColor;
								}

								if( targ.nameText.gameObject.GetComponent<UnityEngine.UI.Outline>() )
								{
									Undo.RecordObject( targ.nameText.gameObject.GetComponent<UnityEngine.UI.Outline>(), "Update Text Outline" );
									targ.nameText.gameObject.GetComponent<UnityEngine.UI.Outline>().enabled = targ.nameOutline;
								}
							}
						}

						if( targ.nameOutline )
						{
							EditorGUI.indentLevel++;
							EditorGUI.BeginChangeCheck();
							nameTextOutlineColor = EditorGUILayout.ColorField( "Outline Color", nameTextOutlineColor );
							if( EditorGUI.EndChangeCheck() )
							{
								if( targ.nameText != null && targ.nameText.GetComponent<UnityEngine.UI.Outline>() )
								{
									Undo.RecordObject( targ.nameText.GetComponent<UnityEngine.UI.Outline>(), "Update Text Outline" );
									targ.nameText.GetComponent<UnityEngine.UI.Outline>().enabled = false;
									targ.nameText.GetComponent<UnityEngine.UI.Outline>().effectColor = nameTextOutlineColor;
									targ.nameText.GetComponent<UnityEngine.UI.Outline>().enabled = true;
								}
							}
							GUILayoutAfterIndentSpace();
							EditorGUI.indentLevel--;
						}

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( nameTextRatioX, 0.0f, 1.0f, new GUIContent( "X Ratio", "The horizontal ratio of the text transform." ) );
						EditorGUILayout.Slider( nameTextRatioY, 0.0f, 1.0f, new GUIContent( "Y Ratio", "The vertical ratio of the text transform." ) );
						EditorGUILayout.Slider( nameTextSize, 0.0f, 1.0f, new GUIContent( "Overall Size", "The overall size of the text transform." ) );
						EditorGUILayout.Slider( nameTextHorizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the text transform." ) );
						EditorGUILayout.Slider( nameTextVerticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the text transform." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}

					EditorGUILayout.Space();
				}
				// --------- END NAME TEXT --------- //
				
				// --------- DESCRIPTION TEXT --------- //
				EditorGUI.BeginChangeCheck();
				displayButtonDescription.boolValue = EditorGUILayout.ToggleLeft( new GUIContent( "Display Description", "Determines if the radial menu should have a text component that will display the description of the currently selected button." ), displayButtonDescription.boolValue );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					if( targ.descriptionText != null )
						targ.descriptionText.gameObject.SetActive( displayButtonDescription.boolValue );
				}

				if( targ.displayButtonDescription )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( descriptionText, new GUIContent( "Description Text", "The text component to be used for the button description." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( descriptionFont, new GUIContent( "Font", "The font to use on the description text." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						if( targ.descriptionText != null )
						{
							Undo.RecordObject( targ.descriptionText, "Update Description Font" );
							targ.descriptionText.font = targ.descriptionFont;
						}
					}

					if( targ.displayButtonDescription && targ.descriptionText == null )
					{
						EditorGUILayout.HelpBox( "There is no text component assigned.", MessageType.Warning );
						if( GUILayout.Button( "Generate Description Text", EditorStyles.miniButton ) )
						{
							descriptionOutline.boolValue = false;

							GameObject newText = new GameObject();
							RectTransform textTransform = newText.AddComponent<RectTransform>();
							newText.AddComponent<CanvasRenderer>();
							Text textComponent = newText.AddComponent<Text>();
							newText.name = "Description Text";

							newText.transform.SetParent( targ.transform );
							textTransform.position = targ.GetComponent<RectTransform>().position;
							textTransform.pivot = new Vector2( 0.5f, 0.5f );
							textTransform.localScale = Vector3.one;
							textComponent.text = "Description Text";
							textComponent.resizeTextForBestFit = true;
							textComponent.resizeTextMinSize = 0;
							textComponent.resizeTextMaxSize = 300;
							textComponent.alignment = TextAnchor.UpperCenter;
							textComponent.color = descriptionTextColor;
							if( targ.descriptionFont != null )
								textComponent.font = targ.descriptionFont;

							descriptionText.objectReferenceValue = newText;
							serializedObject.ApplyModifiedProperties();

							Undo.RegisterCreatedObjectUndo( newText, "Create Description Text Object" );
						}
					}
					else if( targ.descriptionText != null )
					{
						EditorGUI.BeginChangeCheck();
						descriptionTextColor = EditorGUILayout.ColorField( "Text Color", descriptionTextColor );
						if( EditorGUI.EndChangeCheck() )
						{
							if( targ.descriptionText != null )
							{
								Undo.RecordObject( targ.descriptionText, "Update Description Text Color" );
								targ.descriptionText.enabled = false;
								targ.descriptionText.color = descriptionTextColor;
								targ.descriptionText.enabled = true;
							}
						}

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( descriptionOutline, new GUIContent( "Text Outline", "Determines if the text should have an outline or not." ) );
						if( EditorGUI.EndChangeCheck() )
						{
							serializedObject.ApplyModifiedProperties();

							if( targ.descriptionText != null )
							{
								if( targ.descriptionOutline && !targ.descriptionText.gameObject.GetComponent<UnityEngine.UI.Outline>() )
								{
									Undo.RecordObject( targ.descriptionText.gameObject, "Update Text Outline" );
									targ.descriptionText.gameObject.AddComponent<UnityEngine.UI.Outline>();

									descriptionTextOutlineColor = targ.descriptionText.gameObject.GetComponent<UnityEngine.UI.Outline>().effectColor;
								}

								if( targ.descriptionText.gameObject.GetComponent<UnityEngine.UI.Outline>() )
								{
									Undo.RecordObject( targ.descriptionText.gameObject.GetComponent<UnityEngine.UI.Outline>(), "Update Text Outline" );
									targ.descriptionText.gameObject.GetComponent<UnityEngine.UI.Outline>().enabled = targ.descriptionOutline;
								}
							}
						}

						if( targ.descriptionOutline )
						{
							EditorGUI.indentLevel++;
							EditorGUI.BeginChangeCheck();
							descriptionTextOutlineColor = EditorGUILayout.ColorField( "Outline Color", descriptionTextOutlineColor );
							if( EditorGUI.EndChangeCheck() )
							{
								if( targ.descriptionText != null && targ.descriptionText.GetComponent<UnityEngine.UI.Outline>() )
								{
									Undo.RecordObject( targ.descriptionText.GetComponent<UnityEngine.UI.Outline>(), "Update Text Outline" );
									targ.descriptionText.GetComponent<UnityEngine.UI.Outline>().enabled = false;
									targ.descriptionText.GetComponent<UnityEngine.UI.Outline>().effectColor = descriptionTextOutlineColor;
									targ.descriptionText.GetComponent<UnityEngine.UI.Outline>().enabled = true;
								}
							}
							GUILayoutAfterIndentSpace();
							EditorGUI.indentLevel--;
						}

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( descriptionTextRatioX, 0.0f, 1.0f, new GUIContent( "X Ratio", "The horizontal ratio of the text transform." ) );
						EditorGUILayout.Slider( descriptionTextRatioY, 0.0f, 1.0f, new GUIContent( "Y Ratio", "The vertical ratio of the text transform." ) );
						EditorGUILayout.Slider( descriptionTextSize, 0.0f, 1.0f, new GUIContent( "Overall Size", "The overall size of the text transform." ) );
						EditorGUILayout.Slider( descriptionTextHorizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the text transform." ) );
						EditorGUILayout.Slider( descriptionTextVerticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the text transform." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}
				}
				// --------- END DESCRIPTION TEXT --------- //
				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
			// END MENU TEXT //

			// BUTTON ICON //
			valueChanged = false;
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Button Icon", "URM_ButtonIcon", useButtonIcon, ref valueChanged ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( iconNormalColor, new GUIContent( "Icon Color", "The color of the icon image." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].buttonDisabled || targ.UltimateRadialButtonList[ i ].icon == null || targ.UltimateRadialButtonList[ i ].icon.sprite == null )
							continue;

						Undo.RecordObject( targ.UltimateRadialButtonList[ i ].icon, "Update Radial Button Icon Color" );
						targ.UltimateRadialButtonList[ i ].icon.color = targ.iconNormalColor;
					}
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( iconSize, 0.0f, 1.0f, new GUIContent( "Icon Size", "The size of the icon image transform." ) );
				EditorGUILayout.Slider( iconHorizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position in relation to the radial button transform." ) );
				EditorGUILayout.Slider( iconVerticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position in relation to the radial button transform." ) );
				EditorGUILayout.PropertyField( iconRotation, new GUIContent( "Rotation Offset", "The rotation offset to apply to the icon transform." ) );
				if( targ.followOrbitalRotation )
					EditorGUILayout.PropertyField( iconLocalRotation, new GUIContent( "Local Rotation", "Determines if the icon transform will use local or global rotation." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}
			EditorGUILayout.EndVertical();
			if( valueChanged )
			{
				for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
				{
					if( targ.useButtonIcon )
					{
						if( targ.UltimateRadialButtonList[ i ].icon == null )
						{
							GameObject newIcon = new GameObject();
							newIcon.AddComponent<CanvasRenderer>();
							RectTransform iconTransform = newIcon.AddComponent<RectTransform>();
							Image iconImage = newIcon.AddComponent<Image>();

							newIcon.transform.SetParent( targ.UltimateRadialButtonList[ i ].buttonTransform );
							newIcon.name = "Icon";

							iconTransform.pivot = new Vector2( 0.5f, 0.5f );
							iconTransform.localScale = Vector3.one;

							icon[ i ].objectReferenceValue = iconImage;
							serializedObject.ApplyModifiedProperties();

							Undo.RegisterCreatedObjectUndo( newIcon, "Create Icon Objects" );

							if( iconPlaceholderSprite != null )
							{
								iconImage.sprite = iconPlaceholderSprite;
								iconImage.color = targ.iconNormalColor;
							}
							else
								iconImage.color = Color.clear;
						}
						else
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].icon.gameObject, "Enable Button Icon" );
							targ.UltimateRadialButtonList[ i ].icon.gameObject.SetActive( true );
						}
					}
					else
					{
						if( targ.UltimateRadialButtonList[ i ].icon != null )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].icon.gameObject, "Disable Button Icon" );
							targ.UltimateRadialButtonList[ i ].icon.gameObject.SetActive( false );
						}
					}
				}
			}
			// END BUTTON ICON //

			// BUTTON TEXT //
			valueChanged = false;
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Button Text", "URM_ButtonText", useButtonText, ref valueChanged ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( textNormalColor, new GUIContent( "Text Color", "The color to apply to the text component." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int n = 0; n < targ.UltimateRadialButtonList.Count; n++ )
					{
						if( targ.UltimateRadialButtonList[ n ].buttonDisabled || targ.UltimateRadialButtonList[ n ].text == null )
							continue;

						Undo.RecordObject( targ.UltimateRadialButtonList[ n ].text, "Update Button Text Color" );
						targ.UltimateRadialButtonList[ n ].text.color = textNormalColor.colorValue;
					}
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( buttonTextFont, new GUIContent( "Text Font", "The font to apply to the button text." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int n = 0; n < targ.UltimateRadialButtonList.Count; n++ )
					{
						Undo.RecordObject( targ.UltimateRadialButtonList[ n ].text, "Update Button Text Font" );
						targ.UltimateRadialButtonList[ n ].text.font = targ.buttonTextFont;
					}
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( buttonTextOutline, new GUIContent( "Text Outline", "Determines if the text should have an outline or not." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].text != null )
						{
							if( targ.buttonTextOutline && !targ.UltimateRadialButtonList[ i ].text.gameObject.GetComponent<UnityEngine.UI.Outline>() )
							{
								Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text.gameObject, "Update Text Outline" );
								targ.UltimateRadialButtonList[ i ].text.gameObject.AddComponent<UnityEngine.UI.Outline>();

								buttonTextOutlineColor.colorValue = targ.UltimateRadialButtonList[ i ].text.gameObject.GetComponent<UnityEngine.UI.Outline>().effectColor;
								serializedObject.ApplyModifiedProperties();
							}

							if( targ.UltimateRadialButtonList[ i ].text.gameObject.GetComponent<UnityEngine.UI.Outline>() )
							{
								Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text.gameObject.GetComponent<UnityEngine.UI.Outline>(), "Update Text Outline" );
								targ.UltimateRadialButtonList[ i ].text.gameObject.GetComponent<UnityEngine.UI.Outline>().enabled = targ.buttonTextOutline;
							}
						}
					}
				}

				if( targ.buttonTextOutline )
				{
					EditorGUI.indentLevel++;
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( buttonTextOutlineColor, new GUIContent( "Outline Color" ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
						{
							if( targ.UltimateRadialButtonList[ i ].text != null && targ.UltimateRadialButtonList[ i ].text.GetComponent<UnityEngine.UI.Outline>() )
							{
								Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text.GetComponent<UnityEngine.UI.Outline>(), "Update Text Outline" );
								targ.UltimateRadialButtonList[ i ].text.GetComponent<UnityEngine.UI.Outline>().enabled = false;
								targ.UltimateRadialButtonList[ i ].text.GetComponent<UnityEngine.UI.Outline>().effectColor = targ.buttonTextOutlineColor;
								targ.UltimateRadialButtonList[ i ].text.GetComponent<UnityEngine.UI.Outline>().enabled = true;
							}
						}
					}
					GUILayoutAfterIndentSpace();
					EditorGUI.indentLevel--;
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( displayNameOnButton, new GUIContent( "Display Name", "Determines if the name of the button should be applied to the text or not." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].text == null )
							continue;

						if( displayNameOnButton.boolValue && buttonName[ i ].stringValue != string.Empty )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text, "Update Button Text Value" );
							targ.UltimateRadialButtonList[ i ].text.text = buttonName[ i ].stringValue;
						}
						else
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text, "Update Button Text Value" );
							targ.UltimateRadialButtonList[ i ].text.text = "Text";
						}
					}
				}

				EditorGUILayout.Space();

				EditorGUILayout.LabelField( "Positioning", EditorStyles.boldLabel );

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( textAreaRatioX, 0.0f, 1.0f, new GUIContent( "Ratio X", "The horizontal ratio of the text transform." ) );
				EditorGUILayout.Slider( textAreaRatioY, 0.0f, 1.0f, new GUIContent( "Ratio Y", "The vertical ratio of the text transform." ) );
				EditorGUILayout.Slider( textSize, 0.0f, 0.5f, new GUIContent( "Text Size", "The overall size of the text transform." ) );
				EditorGUILayout.PropertyField( textLocalPosition, new GUIContent( "Local Position", "Determines if the text will position itself according to the local position and rotation of the button." ) );
				if( targ.textLocalPosition )
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( textLocalRotation, new GUIContent( "Local Rotation", "Determines if the text should follow the local rotation of the button or not." ) );
					GUILayoutAfterIndentSpace();
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.Slider( textHorizontalPosition, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the text transform." ) );
				EditorGUILayout.Slider( textVerticalPosition, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The horizontal position of the text transform." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "relativeToIcon" ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}
			EditorGUILayout.EndVertical();
			if( valueChanged )
			{
				for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
				{
					if( targ.useButtonText )
					{
						if( targ.UltimateRadialButtonList[ i ].text == null )
						{
							GameObject newText = new GameObject();
							RectTransform textTransform = newText.AddComponent<RectTransform>();
							newText.AddComponent<CanvasRenderer>();
							Text textComponent = newText.AddComponent<Text>();
							newText.name = "Text";

							newText.transform.SetParent( targ.UltimateRadialButtonList[ i ].buttonTransform );
							newText.transform.SetAsLastSibling();

							textTransform.position = targ.UltimateRadialButtonList[ radialNameListIndex ].buttonTransform.position;
							textTransform.localScale = Vector3.one;
							textTransform.pivot = new Vector2( 0.5f, 0.5f );

							textComponent.text = "Text";
							textComponent.resizeTextForBestFit = true;
							textComponent.resizeTextMinSize = 0;
							textComponent.resizeTextMaxSize = 300;
							textComponent.alignment = TextAnchor.MiddleCenter;
							textComponent.color = targ.textNormalColor;

							if( targ.buttonTextFont != null )
								textComponent.font = targ.buttonTextFont;

							if( targ.buttonTextOutline && !newText.gameObject.GetComponent<UnityEngine.UI.Outline>() )
								newText.gameObject.AddComponent<UnityEngine.UI.Outline>();

							text[ i ].objectReferenceValue = newText;
							serializedObject.ApplyModifiedProperties();
							
							Undo.RegisterCreatedObjectUndo( newText, "Create Text Objects" );
						}
						else
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text.gameObject, "Enable Button Text" );
							targ.UltimateRadialButtonList[ i ].text.gameObject.SetActive( true );
						}
					}
					else
					{
						if( targ.UltimateRadialButtonList[ i ].text != null )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text.gameObject, "Disable Button Text" );
							targ.UltimateRadialButtonList[ i ].text.gameObject.SetActive( false );
						}
					}
				}
			}
			// END BUTTON TEXT //
		}

		if( DisplayHeaderDropdown( "Button Interaction", "URM_ButtonInteraction" ) )
		{
			EditorGUI.BeginDisabledGroup( targ.normalSprite == null );
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( spriteSwap, new GUIContent( "Sprite Swap", "Determines whether or not the radial buttons will swap sprites when being interacted with." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
				{
					Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Button Disabled Sprite" );
					if( targ.spriteSwap && targ.UltimateRadialButtonList[ i ].buttonDisabled && targ.disabledSprite != null )
						targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.disabledSprite;
					else
						targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.normalSprite;
				}

				UpdateRadialButtonStyle();
			}

			if( targ.spriteSwap && targ.radialMenuStyle != null )
			{
				GUIStyle noteStyle = new GUIStyle( EditorStyles.miniLabel ) { alignment = TextAnchor.MiddleCenter, richText = true, wordWrap = true };
				EditorGUILayout.LabelField( "<color=red>*</color> The button interaction sprites are determined by the assigned style. To modify the different sprites please edit the Radial Button Style object.", noteStyle );
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( colorChange, new GUIContent( "Color Change", "Determines whether or not the radial buttons will change color when being interacted with." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
				{
					Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Button Color" );
					if( targ.colorChange && targ.UltimateRadialButtonList[ i ].buttonDisabled )
						targ.UltimateRadialButtonList[ i ].radialImage.color = targ.disabledColor;
					else
						targ.UltimateRadialButtonList[ i ].radialImage.color = targ.normalColor;
				}
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( scaleTransform, new GUIContent( "Scale Transform", "Determines whether or not the radial buttons will scale when being interacted with." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( targ.useButtonIcon )
			{
				EditorGUILayout.Space();

				EditorGUILayout.LabelField( "Button Icon", EditorStyles.boldLabel );

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( iconColorChange, new GUIContent( "Color Change", "Determines whether or not the icon will change color when being interacted with." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].icon == null || targ.UltimateRadialButtonList[ i ].icon.sprite == null )
							continue;

						Undo.RecordObject( targ.UltimateRadialButtonList[ i ].icon, "Update Radial Button Icon Color" );
						
						if( targ.iconColorChange && targ.UltimateRadialButtonList[ i ].buttonDisabled )
							targ.UltimateRadialButtonList[ i ].icon.color = targ.iconDisabledColor;
						else
							targ.UltimateRadialButtonList[ i ].icon.color = targ.iconNormalColor;
					}
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( iconScaleTransform, new GUIContent( "Scale Transform", "Determines whether or not the icon will scale when being interacted with." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}

			if( targ.useButtonText )
			{
				EditorGUILayout.Space();

				EditorGUILayout.LabelField( "Button Text", EditorStyles.boldLabel );

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( textColorChange, new GUIContent( "Color Change", "Determines whether or not the text will change color when being interacted with." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].text == null )
							continue;

						Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text, "Update Radial Button Text Color" );
						if( targ.UltimateRadialButtonList[ i ].buttonDisabled )
							targ.UltimateRadialButtonList[ i ].text.color = targ.textDisabledColor;
						else
							targ.UltimateRadialButtonList[ i ].text.color = targ.textNormalColor;
					}
				}
			}

			// BUTTON INTERACTION SETTINGS //
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Normal", "URM_ButtonNormal" ) )
			{
				if( targ.radialMenuStyle == null )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( normalSprite, new GUIContent( "Button Sprite", "The default sprite to apply to the radial button image." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						if( normalSprite.objectReferenceValue == null )
						{
							spriteSwap.boolValue = false;
							colorChange.boolValue = false;
						}
						serializedObject.ApplyModifiedProperties();
						

						for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Radial Button Sprite" );
							
							if( targ.normalSprite != null )
							{
								if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.spriteSwap )
									targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.normalSprite;

								if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.colorChange )
									targ.UltimateRadialButtonList[ i ].radialImage.color = normalColor.colorValue;
							}
							else
							{
								if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.spriteSwap )
									targ.UltimateRadialButtonList[ i ].radialImage.sprite = null;

								if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.colorChange )
									targ.UltimateRadialButtonList[ i ].radialImage.color = Color.clear;
							}
						}
					}
				}

				EditorGUI.BeginDisabledGroup( targ.normalSprite == null );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( normalColor, new GUIContent( "Button Color", "The default color to apply to the radial button image." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].radialImage.sprite == null )
							continue;

						if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.colorChange )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Radial Button Color" );
							targ.UltimateRadialButtonList[ i ].radialImage.color = targ.normalColor;
						}
					}
				}
				EditorGUI.EndDisabledGroup();

				if( targ.useButtonIcon && targ.iconColorChange )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Icon", EditorStyles.boldLabel );
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( iconNormalColor, new GUIContent( "Normal Color", "The color of the icon image." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
						{
							if( targ.UltimateRadialButtonList[ i ].buttonDisabled || targ.UltimateRadialButtonList[ i ].icon == null || targ.UltimateRadialButtonList[ i ].icon.sprite == null )
								continue;

							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].icon, "Update Radial Button Icon Color" );
							targ.UltimateRadialButtonList[ i ].icon.color = targ.iconNormalColor;
						}
					}
				}

				if( targ.useButtonText && targ.textColorChange )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Text", EditorStyles.boldLabel );
					EditorGUILayout.PropertyField( textHighlightedColor, new GUIContent( "Text Color", "The color to be applied to the text when highlighted." ) );
				}

				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Highlighted", "URM_ButtonHighlighted" ) )
			{
				EditorGUI.BeginChangeCheck();

				if( targ.radialMenuStyle == null )
				{
					EditorGUI.BeginDisabledGroup( !targ.spriteSwap );
					EditorGUILayout.PropertyField( highlightedSprite, new GUIContent( "Button Sprite", "The sprite to be applied to the radial button when highlighted." ) );
					EditorGUI.EndDisabledGroup();
				}

				EditorGUI.BeginDisabledGroup( !targ.colorChange );
				EditorGUILayout.PropertyField( highlightedColor, new GUIContent( "Button Color", "The color to be applied to the radial button when highlighted." ) );
				EditorGUI.EndDisabledGroup();
				
				EditorGUI.BeginDisabledGroup( !targ.scaleTransform );
				EditorGUILayout.Slider( highlightedScaleModifier, 0.5f, 1.5f, new GUIContent( "Button Scale", "The scale modifier to be applied to the radial button transform when highlighted." ) );
				EditorGUILayout.Slider( positionModifier, -0.2f, 0.2f, new GUIContent( "Position Modifier", "The position modifier for how much the radial button will expand from it's default position." ) );
				EditorGUI.EndDisabledGroup();

				if( targ.useButtonIcon && ( targ.iconColorChange || targ.iconScaleTransform ) )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Icon", EditorStyles.boldLabel );
					if( targ.iconColorChange )
						EditorGUILayout.PropertyField( iconHighlightedColor, new GUIContent( "Icon Color", "The color to be applied to the icon when highlighted." ) );

					if( targ.iconScaleTransform )
						EditorGUILayout.Slider( iconHighlightedScaleModifier, 0.5f, 1.5f, new GUIContent( "Icon Scale", "The scale modifier to be applied to the icon transform when highlighted." ) );
				}

				if( targ.useButtonText && targ.textColorChange )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Text", EditorStyles.boldLabel );
					EditorGUILayout.PropertyField( textHighlightedColor, new GUIContent( "Text Color", "The color to be applied to the text when highlighted." ) );
				}

				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				
				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Pressed", "URM_ButtonPressed" ) )
			{
				EditorGUI.BeginChangeCheck();

				if( targ.radialMenuStyle == null )
				{
					EditorGUI.BeginDisabledGroup( !targ.spriteSwap );
					EditorGUILayout.PropertyField( pressedSprite, new GUIContent( "Button Sprite", "The sprite to be applied to the radial button when pressed." ) );
					EditorGUI.EndDisabledGroup();
				}

				EditorGUI.BeginDisabledGroup( !targ.colorChange );
				EditorGUILayout.PropertyField( pressedColor, new GUIContent( "Button Color", "The color to be applied to the radial button when pressed." ) );
				EditorGUI.EndDisabledGroup();
				
				EditorGUI.BeginDisabledGroup( !targ.scaleTransform );
				EditorGUILayout.Slider( pressedScaleModifier, 0.5f, 1.5f, new GUIContent( "Button Scale", "The scale modifier to be applied to the radial button transform when pressed." ) );
				EditorGUILayout.Slider( pressedPositionModifier, -0.2f, 0.2f, new GUIContent( "Position Modifier", "The position modifier for how much the radial button will expand from it's default position." ) );
				EditorGUI.EndDisabledGroup();

				if( targ.useButtonIcon && ( targ.iconColorChange || targ.iconScaleTransform ) )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Icon", EditorStyles.boldLabel );
					if( targ.iconColorChange )
						EditorGUILayout.PropertyField( iconPressedColor, new GUIContent( "Icon Color", "The color to be applied to the icon when pressed." ) );
					
					if( targ.iconScaleTransform )
						EditorGUILayout.Slider( iconPressedScaleModifier, 0.5f, 1.5f, new GUIContent( "Icon Scale", "The scale modifier to be applied to the icon transform when pressed." ) );
				}

				if( targ.useButtonText && targ.textColorChange )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Text", EditorStyles.boldLabel );
					EditorGUILayout.PropertyField( textPressedColor, new GUIContent( "Text Color", "The color to be applied to the text when pressed." ) );
				}

				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				
				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Selected", "URM_ButtonSelected" ) )
			{
				EditorGUI.BeginChangeCheck();

				if( targ.radialMenuStyle == null )
				{
					EditorGUI.BeginDisabledGroup( !targ.spriteSwap );
					EditorGUILayout.PropertyField( selectedSprite, new GUIContent( "Button Sprite", "The sprite to be applied to the radial button when selected." ) );
					EditorGUI.EndDisabledGroup();
				}

				EditorGUI.BeginDisabledGroup( !targ.colorChange );
				EditorGUILayout.PropertyField( selectedColor, new GUIContent( "Button Color", "The color to be applied to the radial button when selected." ) );
				EditorGUI.EndDisabledGroup();
				
				EditorGUI.BeginDisabledGroup( !targ.scaleTransform );
				EditorGUILayout.Slider( selectedScaleModifier, 0.5f, 1.5f, new GUIContent( "Button Scale", "The scale modifier to be applied to the radial button transform when selected." ) );
				EditorGUILayout.Slider( selectedPositionModifier, -0.2f, 0.2f, new GUIContent( "Position Modifier", "The position modifier for how much the radial button will expand from it's default position." ) );
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginChangeCheck();
				selectButtonOnInteract.boolValue = EditorGUILayout.ToggleLeft( new GUIContent( "Select On Interact", "Determines if the radial menu should show the last interacted button as selected." ), selectButtonOnInteract.boolValue );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				if( targ.useButtonIcon && ( targ.iconColorChange || targ.iconScaleTransform ) )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Icon", EditorStyles.boldLabel );

					if( targ.iconColorChange )
						EditorGUILayout.PropertyField( iconSelectedColor, new GUIContent( "Icon Color", "The color to be applied to the icon when selected." ) );
					
					if( targ.iconScaleTransform )
						EditorGUILayout.Slider( iconSelectedScaleModifier, 0.5f, 1.5f, new GUIContent( "Icon Scale", "The scale modifier to be applied to the icon transform when selected." ) );
				}

				if( targ.useButtonText && targ.textColorChange )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Text", EditorStyles.boldLabel );
					EditorGUILayout.PropertyField( textSelectedColor, new GUIContent( "Text Color", "The color to be applied to the text when selected." ) );
				}

				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				
				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Disabled", "URM_ButtonDisabled" ) )
			{
				if( targ.radialMenuStyle == null )
				{
					EditorGUI.BeginDisabledGroup( !targ.spriteSwap );
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( disabledSprite, new GUIContent( "Button Sprite", "The sprite to be applied to the radial button when disabled." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
						{
							if( targ.UltimateRadialButtonList[ i ].buttonDisabled && targ.disabledSprite != null )
							{
								Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Button Disabled Sprite" );
								targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.disabledSprite;
							}
						}
					}
					EditorGUI.EndDisabledGroup();
				}

				EditorGUI.BeginDisabledGroup( !targ.colorChange );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( disabledColor, new GUIContent( "Button Color", "The color to be applied to the radial button when disabled." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					{
						if( targ.UltimateRadialButtonList[ i ].buttonDisabled )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Radial Button Color" );
							targ.UltimateRadialButtonList[ i ].radialImage.color = targ.disabledColor;
						}
					}
				}
				EditorGUI.EndDisabledGroup();
				
				EditorGUI.BeginDisabledGroup( !targ.scaleTransform );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( disabledScaleModifier, 0.5f, 1.5f, new GUIContent( "Button Scale", "The scale modifier to be applied to the radial button transform when disabled." ) );
				EditorGUILayout.Slider( disabledPositionModifier, -0.2f, 0.2f, new GUIContent( "Position Modifier", "The position modifier for how much the radial button will expand from it's default position." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				EditorGUI.EndDisabledGroup();

				if( targ.useButtonIcon && ( targ.iconColorChange || targ.iconScaleTransform ) )
				{
					EditorGUILayout.Space();

					EditorGUILayout.LabelField( "Button Icon", EditorStyles.boldLabel );
					if( targ.iconColorChange )
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( iconDisabledColor, new GUIContent( "Icon Color", "The color to be applied to the icon when disabled." ) );
						if( EditorGUI.EndChangeCheck() )
						{
							serializedObject.ApplyModifiedProperties();

							for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
							{
								if( targ.UltimateRadialButtonList[ i ].buttonDisabled && targ.useButtonIcon && targ.UltimateRadialButtonList[ i ].icon != null && targ.UltimateRadialButtonList[ i ].icon.sprite != null )
								{
									Undo.RecordObject( targ.UltimateRadialButtonList[ i ].icon, "Update Button Disabled Color" );
									targ.UltimateRadialButtonList[ i ].icon.color = iconDisabledColor.colorValue;
								}
							}
						}
					}

					if( targ.iconScaleTransform )
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( iconDisabledScaleModifier, 0.5f, 1.5f, new GUIContent( "Icon Scale", "The scale modifier to be applied to the icon transform when disabled." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}
				}

				if( targ.useButtonText && targ.textColorChange )
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField( "Button Text", EditorStyles.boldLabel );
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( textDisabledColor, new GUIContent( "Text Color", "The color to be applied to the icon when disabled." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
						{
							if( targ.UltimateRadialButtonList[ i ].buttonDisabled && targ.UltimateRadialButtonList[ i ].text != null )
							{
								Undo.RecordObject( targ.UltimateRadialButtonList[ i ].text, "Update Text Disabled Color" );
								targ.UltimateRadialButtonList[ i ].text.color = textDisabledColor.colorValue;
							}
						}
					}
				}
				
				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
		}

		if( DisplayHeaderDropdown( "Radial Button List", "URM_RadialButtonList" ) )
		{
			if( Application.isPlaying )
				EditorGUILayout.HelpBox( "Radial Button List cannot be edited during play mode.", MessageType.Info );
			else
			{
				EditorGUILayout.BeginVertical( "Box" );
				GUILayout.Space( 1 );
				
				GUIStyle headerStyle = new GUIStyle( GUI.skin.label )
				{
					fontStyle = FontStyle.Bold,
					alignment = TextAnchor.MiddleCenter,
					wordWrap = true
				};

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "◄", headerStyle, GUILayout.Width( 25 ) ) )
				{
					radialNameListIndex = radialNameListIndex == 0 ? targ.UltimateRadialButtonList.Count - 1 : radialNameListIndex - 1;
					GUI.FocusControl( "" );
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.LabelField( buttonNames.Count == 0 ? "" : ( buttonNames[ radialNameListIndex ] ), headerStyle );
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( "►", headerStyle, GUILayout.Width( 25 ) ) )
				{
					radialNameListIndex = radialNameListIndex == targ.UltimateRadialButtonList.Count - 1 ? 0 : radialNameListIndex + 1;
					GUI.FocusControl( "" );
				}
				EditorGUILayout.EndHorizontal();
				if( EditorGUI.EndChangeCheck() )
					EditorGUIUtility.PingObject( targ.UltimateRadialButtonList[ radialNameListIndex ].buttonTransform );

				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup( Application.isPlaying );
				if( GUILayout.Button( "Insert", EditorStyles.miniButtonLeft ) )
					AddNewRadialButton( radialNameListIndex + 1 );
				EditorGUI.EndDisabledGroup();
				EditorGUI.BeginDisabledGroup( targ.UltimateRadialButtonList.Count == 1 );
				if( GUILayout.Button( "Remove", EditorStyles.miniButtonRight ) )
				{
					if( EditorUtility.DisplayDialog( "Ultimate Radial Menu", "Warning!\n\nAre you sure that you want to delete this radial button?", "Yes", "No" ) )
						RemoveRadialButton( radialNameListIndex );
				}
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
				// END RADIAL BUTTON TOOLBAR //

				EditorGUILayout.Space();

				if( radialNameListIndex < 0 || radialNameListIndex > targ.UltimateRadialButtonList.Count - 1 )
					radialNameListIndex = 0;

				if( buttonDisabled[ radialNameListIndex ] == null )
					StoreReferences();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( buttonDisabled[ radialNameListIndex ], new GUIContent( "Disable Button", "Determines if this button should be disabled or not." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					if( buttonDisabled[ radialNameListIndex ].boolValue == true )
						targ.UltimateRadialButtonList[ radialNameListIndex ].DisableButton();
					else
						targ.UltimateRadialButtonList[ radialNameListIndex ].EnableButton();
				}

				EditorGUI.BeginDisabledGroup( buttonDisabled[ radialNameListIndex ].boolValue );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( buttonName[ radialNameListIndex ], new GUIContent( "Name", "The name to be displayed on the radial button." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					if( targ.UltimateRadialButtonList[ radialNameListIndex ].text != null )
					{
						if( displayNameOnButton.boolValue && targ.UltimateRadialButtonList[ radialNameListIndex ].name != string.Empty )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ radialNameListIndex ].text, "Update Radial Button Name" );
							targ.UltimateRadialButtonList[ radialNameListIndex ].text.text = buttonName[ radialNameListIndex ].stringValue;
						}
						else
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ radialNameListIndex ].text, "Update Button Text Value" );
							targ.UltimateRadialButtonList[ radialNameListIndex ].text.text = "Text";
						}
					}
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( buttonKey[ radialNameListIndex ], new GUIContent( "Key", "The string key to send when the radial button is interacted with." ) );
				EditorGUILayout.PropertyField( buttonId[ radialNameListIndex ], new GUIContent( "ID", "The integer ID to send when the radial button is interacted with." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				if( description[ radialNameListIndex ].stringValue == string.Empty && Event.current.type == EventType.Repaint )
				{
					GUIStyle style = new GUIStyle( GUI.skin.textField );
					style.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 0.75f );
					style.wordWrap = true;
					EditorGUILayout.TextField( GUIContent.none, "Description", style );
				}
				else
				{
					Event mEvent = Event.current;

					if( mEvent.type == EventType.KeyDown && mEvent.keyCode == KeyCode.Return )
					{
						GUI.SetNextControlName( "DescriptionField" );
						if( GUI.GetNameOfFocusedControl() == "DescriptionField" )
							GUI.FocusControl( "" );
					}

					GUIStyle style = new GUIStyle( GUI.skin.textField ) { wordWrap = true };

					EditorGUI.BeginChangeCheck();
					description[ radialNameListIndex ].stringValue = EditorGUILayout.TextArea( description[ radialNameListIndex ].stringValue, style );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}

				EditorGUILayout.Space();

				// ------------------------------------------- ICON SETTINGS ------------------------------------------- //
				if( targ.useButtonIcon )
				{
					EditorGUILayout.LabelField( "Icon Settings", EditorStyles.boldLabel );

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( icon[ radialNameListIndex ], new GUIContent( "Icon Image", "The image component associated with this radial button." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						if( icon[ radialNameListIndex ].objectReferenceValue != null )
							serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconTransform", radialNameListIndex ) ).objectReferenceValue = targ.UltimateRadialButtonList[ radialNameListIndex ].icon.rectTransform;
						else
							serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconTransform", radialNameListIndex ) ).objectReferenceValue = null;

						serializedObject.ApplyModifiedProperties();
					}

					if( targ.UltimateRadialButtonList[ radialNameListIndex ].icon != null )
					{
						EditorGUI.BeginChangeCheck();
						iconSprites[ radialNameListIndex ] = ( Sprite )EditorGUILayout.ObjectField( targ.UltimateRadialButtonList[ radialNameListIndex ].icon.sprite, typeof( Sprite ), true );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ radialNameListIndex ].icon, "Update Radial Button Icon" );

							targ.UltimateRadialButtonList[ radialNameListIndex ].icon.enabled = false;
							targ.UltimateRadialButtonList[ radialNameListIndex ].icon.sprite = iconSprites[ radialNameListIndex ];
							targ.UltimateRadialButtonList[ radialNameListIndex ].icon.enabled = true;

							if( iconSprites[ radialNameListIndex ] != null )
								targ.UltimateRadialButtonList[ radialNameListIndex ].icon.color = targ.iconNormalColor;
							else
								targ.UltimateRadialButtonList[ radialNameListIndex ].icon.color = Color.clear;
						}
					}
					if( iconLocalRotation.boolValue )
					{
						EditorGUI.BeginChangeCheck();
						invertScaleY[ radialNameListIndex ].boolValue = GUILayout.Toggle( invertScaleY[ radialNameListIndex ].boolValue, "Invert Y Scale", EditorStyles.miniButton );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( useIconUnique[ radialNameListIndex ], new GUIContent( "Unique Positioning", "Determines if the icon should use positioning different from the prefab radial button or not." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						if( useIconUnique[ radialNameListIndex ].boolValue == true )
						{
							if( rmbIconSize[ radialNameListIndex ].floatValue == 0.0f )
								rmbIconSize[ radialNameListIndex ].floatValue = iconSize.floatValue;
							if( rmbIconHorizontalPosition[ radialNameListIndex ].floatValue == 0.0f )
								rmbIconHorizontalPosition[ radialNameListIndex ].floatValue = iconHorizontalPosition.floatValue;
							if( rmbIconVerticalPosition[ radialNameListIndex ].floatValue == 0.0f )
								rmbIconVerticalPosition[ radialNameListIndex ].floatValue = iconVerticalPosition.floatValue;
							if( rmbIconRotation[ radialNameListIndex ].floatValue == 0.0f )
								rmbIconRotation[ radialNameListIndex ].floatValue = iconRotation.floatValue;
						}
						serializedObject.ApplyModifiedProperties();
					}
					if( useIconUnique[ radialNameListIndex ].boolValue )
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( rmbIconSize[ radialNameListIndex ], 0.0f, 1.0f, new GUIContent( "Icon Size", "The size of the icon image transform." ) );
						EditorGUILayout.Slider( rmbIconHorizontalPosition[ radialNameListIndex ], 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position in relation to the radial button transform." ) );
						EditorGUILayout.Slider( rmbIconVerticalPosition[ radialNameListIndex ], 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position in relation to the radial button transform." ) );
						EditorGUILayout.PropertyField( rmbIconRotation[ radialNameListIndex ], new GUIContent( "Rotation Offset", "The rotation offset to apply to the icon transform." ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}
					EditorGUILayout.Space();
				}

				// ------------------------------------------- TEXT SETTINGS ------------------------------------------- //
				if( targ.useButtonText )
				{
					EditorGUILayout.LabelField( "Text Settings", EditorStyles.boldLabel );

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( text[ radialNameListIndex ], new GUIContent( "Button Text", "The text component to use for this radial button." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					if( !targ.displayNameOnButton )
					{
						EditorGUI.BeginChangeCheck();
						buttonText[ radialNameListIndex ] = EditorGUILayout.TextField( buttonText[ radialNameListIndex ] );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( targ.UltimateRadialButtonList[ radialNameListIndex ].text, "Update Radial Button Text" );

							targ.UltimateRadialButtonList[ radialNameListIndex ].text.enabled = false;
							targ.UltimateRadialButtonList[ radialNameListIndex ].text.text = buttonText[ radialNameListIndex ];
							targ.UltimateRadialButtonList[ radialNameListIndex ].text.enabled = true;
						}
					}

					EditorGUILayout.Space();
				}

				// UNITY EVENTS //
				EditorGUILayout.BeginHorizontal();
				GUIStyle unityEventLabelStyle = new GUIStyle( GUI.skin.label );

				if( targ.UltimateRadialButtonList[ radialNameListIndex ].unityEvent.GetPersistentEventCount() > 0 )
					unityEventLabelStyle.fontStyle = FontStyle.Bold;

				EditorGUILayout.LabelField( "Unity Events", unityEventLabelStyle );
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( EditorPrefs.GetBool( "URM_RadialButtonUnityEvents" ) ? "-" : "+", EditorStyles.miniButton, GUILayout.Width( 17 ) ) )
					EditorPrefs.SetBool( "URM_RadialButtonUnityEvents", !EditorPrefs.GetBool( "URM_RadialButtonUnityEvents" ) );
				EditorGUILayout.EndHorizontal();
				if( EditorPrefs.GetBool( "URM_RadialButtonUnityEvents" ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( unityEvent[ radialNameListIndex ] );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUI.EndDisabledGroup();

				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();

				if( GUILayout.Button( "Clear Radial Buttons", EditorStyles.miniButton ) )
				{
					if( EditorUtility.DisplayDialog( "Ultimate Radial Menu", "Warning!\n\nAre you sure that you want to delete all of the radial buttons?", "Yes", "No" ) )
					{
						DeleteRadialImages();
						StoreReferences();
						Repaint();
						return;
					}
				}
			}
		}
		
		if( DisplayHeaderDropdown( "Script Reference", "UUI_ScriptReference" ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( radialMenuName, new GUIContent( "Radial Menu Name", "The name to be used for reference from scripts." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				RadialMenuNameDuplicate = DuplicateRadialMenuName();
				RadialMenuNameUnassigned = targ.radialMenuName == string.Empty ? true : false;
				RadialMenuNameAssigned = RadialMenuNameDuplicate == false && targ.radialMenuName != string.Empty ? true : false;
			}

			if( RadialMenuNameUnassigned )
				EditorGUILayout.HelpBox( "Please make sure to assign a name so that this radial menu can be referenced from your scripts.", MessageType.Warning );
			else if( RadialMenuNameDuplicate )
				EditorGUILayout.HelpBox( "This name has already been used in your scene. Please make sure to make the Radial Menu Name unique.", MessageType.Error );
			else if( RadialMenuNameAssigned )
			{
				EditorGUILayout.BeginVertical( "Box" );
				GUILayout.Space( 1 );
				EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );

				exampleCodeIndex = EditorGUILayout.Popup( "Function", exampleCodeIndex, exampleCodeOptions.ToArray() );

				EditorGUILayout.LabelField( "Function Description", EditorStyles.boldLabel );
				GUIStyle wordWrappedLabel = new GUIStyle( GUI.skin.label ) { wordWrap = true };
				EditorGUILayout.LabelField( ExampleCodes[ exampleCodeIndex ].optionDescription, wordWrappedLabel );

				EditorGUILayout.LabelField( "Example Code", EditorStyles.boldLabel );
				GUIStyle wordWrappedTextArea = new GUIStyle( GUI.skin.textArea ) { wordWrap = true };
				EditorGUILayout.TextArea( string.Format( ExampleCodes[ exampleCodeIndex ].basicCode, radialMenuName.stringValue, exampleCodeIndex == 1 ? "\"" + targ.UltimateRadialButtonList[ radialNameListIndex ].name + "\"" : radialNameListIndex.ToString() ), wordWrappedTextArea );

				if( exampleCodeIndex <= 3 )
				{
					EditorGUILayout.LabelField( "Needed Variable", EditorStyles.boldLabel );
					EditorGUILayout.TextArea( "UltimateRadialButtonInfo buttonInfo;", wordWrappedTextArea );
				}

				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}

			if( GUILayout.Button( "Open Documentation" ) )
				UltimateRadialMenuReadmeEditor.OpenReadmeDocumentation();
		}

		if( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) )
		{
			EditorGUILayout.Space();
			GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11, richText = true };
			GUILayout.BeginHorizontal();
			GUILayout.Space( -10 );
			showDefaultInspector = GUILayout.Toggle( showDefaultInspector, ( showDefaultInspector ? "▼" : "►" ) + "<color=#ff0000ff>Development Inspector</color>", toolbarStyle );
			GUILayout.EndHorizontal();
			if( showDefaultInspector )
			{
				EditorGUILayout.Space();

				base.OnInspectorGUI();
			}
		}

		EditorGUILayout.Space();

		Repaint();
	}
	
	void CheckForParentCanvas ()
	{
		if( Selection.activeGameObject == null )
			return;

		// Store the current parent.
		Transform parent = Selection.activeGameObject.transform.parent;

		// Loop through parents as long as there is one.
		while( parent != null )
		{
			// If there is a Canvas component, return that gameObject.
			if( parent.transform.GetComponent<UnityEngine.Canvas>() && parent.transform.GetComponent<UnityEngine.Canvas>().enabled == true )
			{
				parentCanvas = parent.transform.GetComponent<UnityEngine.Canvas>();
				return;
			}

			// Else, shift to the next parent.
			parent = parent.transform.parent;
		}
		if( parent == null && !AssetDatabase.Contains( Selection.activeGameObject ) )
		{
			if( EditorUtility.DisplayDialog( "Ultimate Radial Menu", "Where are you wanting to use this Ultimate Radial Menu?", "World Space", "Screen Space" ) )
				RequestCanvas( Selection.activeGameObject, false );
			else
				RequestCanvas( Selection.activeGameObject );
		}
	}

	bool DuplicateRadialMenuName ()
	{
		UltimateRadialMenu[] ultimateRadialMenus = FindObjectsOfType<UltimateRadialMenu>();

		for( int i = 0; i < ultimateRadialMenus.Length; i++ )
		{
			if( ultimateRadialMenus[ i ].radialMenuName == string.Empty )
				continue;

			if( ultimateRadialMenus[ i ] != targ && ultimateRadialMenus[ i ].radialMenuName == targ.radialMenuName )
				return true;
		}
		return false;
	}

	void AddNewRadialButton ( int index )
	{
		serializedObject.FindProperty( "UltimateRadialButtonList" ).InsertArrayElementAtIndex( index );
		serializedObject.ApplyModifiedProperties();

		GameObject newGameObject  = new GameObject();
		newGameObject.AddComponent<RectTransform>();
		newGameObject.AddComponent<CanvasRenderer>();
		newGameObject.AddComponent<Image>();

		if( targ.normalSprite != null )
		{
			newGameObject.GetComponent<Image>().sprite = targ.normalSprite;
			newGameObject.GetComponent<Image>().color = targ.normalColor;
		}
		else
			newGameObject.GetComponent<Image>().color = Color.clear;

		GameObject image = Instantiate( newGameObject.gameObject, Vector3.zero, Quaternion.identity );
		image.transform.SetParent( targ.transform );
		image.transform.SetSiblingIndex( targ.UltimateRadialButtonList[ targ.UltimateRadialButtonList.Count - 1 ].buttonTransform.GetSiblingIndex() + 1 );

		image.gameObject.name = "Radial Image " + ( targ.UltimateRadialButtonList.Count ).ToString( "00" );

		RectTransform trans = image.GetComponent<RectTransform>();
		
		trans.anchorMin = new Vector2( 0.5f, 0.5f );
		trans.anchorMax = new Vector2( 0.5f, 0.5f );
		trans.pivot = new Vector2( 0.5f, 0.5f );

		serializedObject.FindProperty( "menuButtonCount" ).intValue++;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].radialMenu", index ) ).objectReferenceValue = targ;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonTransform", index ) ).objectReferenceValue = trans;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].radialImage", index ) ).objectReferenceValue = trans.GetComponent<Image>();
		serializedObject.ApplyModifiedProperties();

		if( targ.useButtonIcon )
		{
			GameObject newIcon = new GameObject();
			newIcon.AddComponent<CanvasRenderer>();
			RectTransform iconTransform = newIcon.AddComponent<RectTransform>();
			Image iconImage = newIcon.AddComponent<Image>();

			newIcon.transform.SetParent( targ.UltimateRadialButtonList[ index ].buttonTransform );
			newIcon.name = "Icon";

			iconTransform.pivot = new Vector2( 0.5f, 0.5f );
			iconTransform.localScale = Vector3.one;

			if( iconPlaceholderSprite == null && targ.UltimateRadialButtonList.Count > 0 && targ.UltimateRadialButtonList[ 0 ].icon != null && targ.UltimateRadialButtonList[ 0 ].icon.sprite != null )
				iconPlaceholderSprite = targ.UltimateRadialButtonList[ 0 ].icon.sprite;

			iconImage.color = targ.iconNormalColor;
			if( iconPlaceholderSprite != null )
			{
				iconImage.sprite = iconPlaceholderSprite;
				iconImage.color = targ.iconNormalColor;
			}
			else
				iconImage.color = Color.clear;

			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].icon", index ) ).objectReferenceValue = iconImage;
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconTransform", index ) ).objectReferenceValue = iconTransform;
			serializedObject.ApplyModifiedProperties();
		}

		if( targ.useButtonText )
		{
			GameObject newText = new GameObject();
			RectTransform textTransform = newText.AddComponent<RectTransform>();
			newText.AddComponent<CanvasRenderer>();
			Text textComponent = newText.AddComponent<Text>();
			newText.name = "Text";

			newText.transform.SetParent( targ.UltimateRadialButtonList[ index ].buttonTransform );
			newText.transform.SetAsLastSibling();

			textTransform.position = targ.UltimateRadialButtonList[ radialNameListIndex ].buttonTransform.position;
			textTransform.localScale = Vector3.one;
			textTransform.pivot = new Vector2( 0.5f, 0.5f );

			textComponent.text = "Text";
			textComponent.resizeTextForBestFit = true;
			textComponent.resizeTextMinSize = 0;
			textComponent.resizeTextMaxSize = 300;
			textComponent.alignment = TextAnchor.MiddleCenter;
			textComponent.color = targ.textNormalColor;

			if( targ.buttonTextFont != null )
				textComponent.font = targ.buttonTextFont;

			if( targ.buttonTextOutline )
			{
				UnityEngine.UI.Outline textOutline = textComponent.gameObject.AddComponent<UnityEngine.UI.Outline>();
				textOutline.effectColor = targ.buttonTextOutlineColor;
			}
			
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].text", index ) ).objectReferenceValue = textComponent;
			serializedObject.ApplyModifiedProperties();
		}

		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].useIconUnique", index ) ).boolValue = false;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonDisabled", index ) ).boolValue = targ.UltimateRadialButtonList[ 0 ].buttonDisabled;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].name", index ) ).stringValue = string.Empty;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].description", index ) ).stringValue = string.Empty;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconSize", index ) ).floatValue = 0.0f;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconHorizontalPosition", index ) ).floatValue = 0.0f;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconVerticalPosition", index ) ).floatValue = 0.0f;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].iconRotation", index ) ).floatValue = 0.0f;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].id", index ) ).intValue = 0;
		serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].key", index ) ).stringValue = string.Empty;
		serializedObject.ApplyModifiedProperties();
		
		buttonInputAngle.floatValue = 1.0f;
		serializedObject.ApplyModifiedProperties();

		Undo.RegisterCreatedObjectUndo( image, "Create Radial Button Object" );

		radialNameListIndex = index;

		StoreReferences();

		if( newGameObject != targ.UltimateRadialButtonList[ 0 ].buttonTransform.gameObject )
			DestroyImmediate( newGameObject );

		UpdateRadialButtonStyle();
	}

	void RemoveRadialButton ( int index )
	{
		GameObject objToDestroy = targ.UltimateRadialButtonList[ index ].radialImage.gameObject;
		menuButtonCount.intValue = menuButtonCount.intValue - 1;
		serializedObject.FindProperty( "UltimateRadialButtonList" ).DeleteArrayElementAtIndex( index );
		buttonInputAngle.floatValue = 1.0f;
		serializedObject.ApplyModifiedProperties();

		Undo.DestroyObjectImmediate( objToDestroy );

		if( index == targ.UltimateRadialButtonList.Count )
			radialNameListIndex = targ.UltimateRadialButtonList.Count - 1;

		StoreReferences();

		UpdateRadialButtonStyle();
	}
	
	void GenerateRadialImages ()
	{
		GameObject newGameObject = new GameObject();
		newGameObject.AddComponent<RectTransform>();
		newGameObject.AddComponent<CanvasRenderer>();
		newGameObject.AddComponent<Image>();

		newGameObject.GetComponent<Image>().color = targ.normalColor;

		if( targ.normalSprite != null )
			newGameObject.GetComponent<Image>().sprite = targ.normalSprite;
		else
			newGameObject.GetComponent<Image>().color = Color.clear;

		newGameObject.transform.SetParent( targ.transform );
		
		for( int i = 0; i < targ.menuButtonCount; i++ )
		{
			GameObject image = Instantiate( newGameObject, Vector3.zero, Quaternion.identity );
			image.transform.SetParent( targ.transform );

			image.gameObject.name = "Radial Image " + i.ToString( "00" );

			RectTransform trans = image.GetComponent<RectTransform>();

			trans.anchorMin = new Vector2( 0.5f, 0.5f );
			trans.anchorMax = new Vector2( 0.5f, 0.5f );
			trans.pivot = new Vector2( 0.5f, 0.5f );

			serializedObject.FindProperty( "UltimateRadialButtonList" ).arraySize++;
			serializedObject.ApplyModifiedProperties();

			targ.UltimateRadialButtonList[ i ] = new UltimateRadialMenu.UltimateRadialButton();
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].radialMenu", i ) ).objectReferenceValue = targ;
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].buttonTransform", i ) ).objectReferenceValue = trans;
			serializedObject.FindProperty( string.Format( "UltimateRadialButtonList.Array.data[{0}].radialImage", i ) ).objectReferenceValue = trans.GetComponent<Image>();
			serializedObject.ApplyModifiedProperties();
			
			Undo.RegisterCreatedObjectUndo( image, "Create Radial Button Objects" );
		}
		
		buttonInputAngle.floatValue = 1.0f;
		serializedObject.ApplyModifiedProperties();
		StoreReferences();

		DestroyImmediate( newGameObject );

		if( !FindObjectOfType<EventSystem>().GetComponent<UltimateRadialMenuInputManager>() )
			FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();
	}

	void DeleteRadialImages ()
	{
		for( int i = targ.UltimateRadialButtonList.Count - 1; i >= 0; i-- )
			Undo.DestroyObjectImmediate( targ.UltimateRadialButtonList[ i ].radialImage.gameObject );

		serializedObject.FindProperty( "UltimateRadialButtonList" ).ClearArray();
		serializedObject.ApplyModifiedProperties();

		StoreReferences();
	}

	void UpdateRadialButtonStyle ()
	{
		if( targ.radialMenuStyle != null && targ.radialMenuStyle.RadialMenuStyles.Count > 0 )
		{
			int CurrentStyleIndex = targ.menuButtonCount <= targ.radialMenuStyle.RadialMenuStyles[ 0 ].buttonCount ? 0 : targ.radialMenuStyle.RadialMenuStyles.Count - 1;
			for( int i = 0; i < targ.radialMenuStyle.RadialMenuStyles.Count; i++ )
			{
				if( targ.radialMenuStyle.RadialMenuStyles[ i ].buttonCount == targ.menuButtonCount )
				{
					CurrentStyleIndex = i;
					break;
				}
			}

			normalSprite.objectReferenceValue = targ.radialMenuStyle.RadialMenuStyles[ CurrentStyleIndex ].normalSprite;
			serializedObject.ApplyModifiedProperties();

			if( targ.spriteSwap )
			{
				highlightedSprite.objectReferenceValue = targ.radialMenuStyle.RadialMenuStyles[ CurrentStyleIndex ].highlightedSprite;
				pressedSprite.objectReferenceValue = targ.radialMenuStyle.RadialMenuStyles[ CurrentStyleIndex ].pressedSprite;
				selectedSprite.objectReferenceValue = targ.radialMenuStyle.RadialMenuStyles[ CurrentStyleIndex ].selectedSprite;
				disabledSprite.objectReferenceValue = targ.radialMenuStyle.RadialMenuStyles[ CurrentStyleIndex ].disabledSprite;
				serializedObject.ApplyModifiedProperties();
			}
			
			for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
			{
				Undo.RecordObject( targ.UltimateRadialButtonList[ i ].radialImage, "Update Radial Button Style" );

				if( targ.UltimateRadialButtonList[ i ].buttonDisabled && targ.spriteSwap && targ.disabledSprite != null )
					targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.disabledSprite;
				else
					targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.normalSprite;

				if( targ.UltimateRadialButtonList[ i ].buttonDisabled && targ.colorChange )
					targ.UltimateRadialButtonList[ i ].radialImage.color = targ.disabledColor;
				else
					targ.UltimateRadialButtonList[ i ].radialImage.color = targ.normalColor;

				// EDIT: This seems to work. However, the strange thing is that this "Record" is done AFTER the mod, not before like everything else.
				if( prefabRootError )
				{
					Debug.Log( "Record prop mod." );
					PrefabUtility.RecordPrefabInstancePropertyModifications( targ.UltimateRadialButtonList[ i ].radialImage );
				}
			}
		}
		else
		{
			for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
			{
				if( targ.normalSprite != null )
				{
					if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.spriteSwap )
						targ.UltimateRadialButtonList[ i ].radialImage.sprite = targ.normalSprite;

					if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.colorChange )
						targ.UltimateRadialButtonList[ i ].radialImage.color = normalColor.colorValue;
				}
				else
				{
					if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.spriteSwap )
						targ.UltimateRadialButtonList[ i ].radialImage.sprite = null;

					if( !targ.UltimateRadialButtonList[ i ].buttonDisabled || !targ.colorChange )
						targ.UltimateRadialButtonList[ i ].radialImage.color = Color.clear;
				}
			}
		}
	}

	void OnSceneGUI ()
	{
		if( Selection.activeGameObject == null || Application.isPlaying || Selection.objects.Length > 1 || parentCanvas == null )
		{
			if( parentCanvas == null )
			{
				CheckForParentCanvas();
				StoreReferences();
			}

			return;
		}
		
		RectTransform trans = targ.transform.GetComponent<RectTransform>();
		Vector3 center = trans.position;
		float sizeDeltaX = trans.sizeDelta.x * parentCanvasRectTrans.localScale.x;

		Handles.color = colorDefault;

		if( targ.UltimateRadialButtonList.Count == 0 )
		{
			Handles.color = colorDefault;
			Handles.DrawWireDisc( center, Selection.activeGameObject.transform.forward, ( sizeDeltaX / 2 ) );
			Handles.DrawWireDisc( center, Selection.activeGameObject.transform.forward, ( sizeDeltaX / 4 ) );

			Handles.color = colorValueChanged;
			float angleInRadians = ( 360f / targ.menuButtonCount ) * Mathf.Deg2Rad;
			float halfAngle = ( targ.GetAnglePerButton / 2 ) * Mathf.Deg2Rad;

			for( int i = 0; i < targ.menuButtonCount; i++ )
			{
				Vector3 lineStart = Vector3.zero;
				lineStart.x += ( Mathf.Cos( ( angleInRadians * i ) + ( 90 * Mathf.Deg2Rad ) + halfAngle ) * ( trans.sizeDelta.x / 4 ) );
				lineStart.y += ( Mathf.Sin( ( angleInRadians * i ) + ( 90 * Mathf.Deg2Rad ) + halfAngle ) * ( trans.sizeDelta.x / 4 ) );
				Vector3 lineEnd = Vector3.zero;
				lineEnd.x += ( Mathf.Cos( ( angleInRadians * i ) + ( 90 * Mathf.Deg2Rad ) + halfAngle ) * ( trans.sizeDelta.x / 2 ) );
				lineEnd.y += ( Mathf.Sin( ( angleInRadians * i ) + ( 90 * Mathf.Deg2Rad ) + halfAngle ) * ( trans.sizeDelta.x / 2 ) );

				lineStart = targ.transform.TransformPoint( lineStart );
				lineEnd = targ.transform.TransformPoint( lineEnd );
				
				Handles.DrawLine( lineStart, lineEnd );
			}
			SceneView.RepaintAll();
			return;
		}

		if( EditorPrefs.GetBool( "URM_RadialMenuPositioning" ) )
		{
			Handles.color = colorDefault;
			if( DisplayMinRange.HighlightGizmo )
				Handles.color = colorValueChanged;

			Handles.DrawWireDisc( center, Selection.activeGameObject.transform.forward, ( sizeDeltaX / 2 ) * targ.minRange );
			Handles.Label( center + ( -trans.transform.up * ( ( sizeDeltaX / 2 ) * targ.minRange ) ), "Min Range: " + targ.minRange );

			Handles.color = colorDefault;
			if( DisplayMaxRange.HighlightGizmo )
				Handles.color = colorValueChanged;

			if( !targ.infiniteMaxRange )
			{
				Handles.DrawWireDisc( center, Selection.activeGameObject.transform.forward, ( sizeDeltaX / 2 ) * targ.maxRange );
				Handles.Label( center + ( -trans.transform.up * ( ( sizeDeltaX / 2 ) * targ.maxRange ) ), "Max Range: " + ( targ.infiniteMaxRange ? "Infinite" : targ.maxRange.ToString() ) );
			}

			if( targ.UltimateRadialButtonList.Count > 0 )
			{
				float maxRange = targ.maxRange;
				if( targ.infiniteMaxRange )
					maxRange = 1.5f;

				Handles.color = colorDefault;
				if( DisplayInputAngle.HighlightGizmo )
					Handles.color = colorValueChanged;

				float minAngle = targ.UltimateRadialButtonList[ 0 ].angle + targ.UltimateRadialButtonList[ 0 ].angleRange;
				float maxAngle = targ.UltimateRadialButtonList[ 0 ].angle - targ.UltimateRadialButtonList[ 0 ].angleRange;

				Vector3 lineStart = Vector3.zero;
				lineStart.x += ( Mathf.Cos( minAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) );
				lineStart.y += ( Mathf.Sin( minAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) );
				Vector3 lineEnd = Vector3.zero;
				lineEnd.x += ( Mathf.Cos( minAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * maxRange ) );
				lineEnd.y += ( Mathf.Sin( minAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * maxRange ) );

				lineStart = targ.transform.TransformPoint( lineStart );
				lineEnd = targ.transform.TransformPoint( lineEnd );

				Handles.DrawLine( lineStart, lineEnd );

				lineStart = Vector3.zero;
				lineStart.x += ( Mathf.Cos( maxAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) );
				lineStart.y += ( Mathf.Sin( maxAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * targ.minRange ) );
				lineEnd = Vector3.zero;
				lineEnd.x += ( Mathf.Cos( maxAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * maxRange ) );
				lineEnd.y += ( Mathf.Sin( maxAngle * Mathf.Deg2Rad ) * ( ( trans.sizeDelta.x / 2 ) * maxRange ) );

				lineStart = targ.transform.TransformPoint( lineStart );
				lineEnd = targ.transform.TransformPoint( lineEnd );

				Handles.DrawLine( lineStart, lineEnd );
			}
		}

		if( EditorPrefs.GetBool( "URM_RadialMenuOptions" ) )
		{
			if( EditorPrefs.GetBool( "URM_MenuText" ) )
			{
				Handles.color = colorTextBox;
				if( targ.displayButtonName && targ.nameText != null )
					DisplayWireBox( targ.nameText.rectTransform.localPosition, targ.nameText.rectTransform.sizeDelta, targ.transform );

				if( targ.displayButtonDescription && targ.descriptionText != null )
					DisplayWireBox( targ.descriptionText.rectTransform.localPosition, targ.descriptionText.rectTransform.sizeDelta, targ.transform );
			}
			
			if( EditorPrefs.GetBool( "URM_ButtonText" ) && targ.useButtonText )
			{
				Handles.color = colorTextBox;
				for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
				{
					if( targ.UltimateRadialButtonList[ i ].text == null )
						continue;

					DisplayWireBox( targ.UltimateRadialButtonList[ i ].text.rectTransform.InverseTransformPoint( targ.UltimateRadialButtonList[ i ].text.rectTransform.position ), targ.UltimateRadialButtonList[ i ].text.rectTransform.sizeDelta, targ.UltimateRadialButtonList[ i ].text.rectTransform );
				}
			}
		}

		if( EditorPrefs.GetBool( "URM_RadialButtonList" ) )
		{
			for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
			{
				if( radialNameListIndex == i )
					Handles.color = colorButtonSelected;
				else
					Handles.color = colorButtonUnselected;

				float handleSize = sizeDeltaX / 10;
				float distanceMod = ( ( sizeDeltaX / 2 ) * ( targ.radialMenuButtonRadius ) ) + handleSize;

				Vector3 difference = center - targ.UltimateRadialButtonList[ i ].buttonTransform.position;
				
				Vector3 handlePos = center;
				handlePos += -difference.normalized * distanceMod;

				if( Handles.Button( handlePos, Quaternion.identity, handleSize, sizeDeltaX / 10, Handles.SphereHandleCap ) )
				{
					radialNameListIndex = i;
					EditorGUIUtility.PingObject( targ.UltimateRadialButtonList[ i ].buttonTransform );
				}
				GUIStyle labelStyle = new GUIStyle( GUI.skin.label )
				{
					alignment = TextAnchor.MiddleCenter,
					fontStyle = FontStyle.Bold,
				};
				Handles.Label( handlePos, i.ToString( "00" ), labelStyle );
			}
		}

		if( targ.normalSprite == null )
		{
			Handles.color = colorDefault;
			if( EditorPrefs.GetBool( "URM_RadialMenuPositioning" ) )
			{
				Color halfColor = colorDefault;
				halfColor.a /= 3;
				Handles.color = halfColor;
			}

			if( targ.followOrbitalRotation )
			{
				float outerRadius = ( sizeDeltaX / 2 ) * ( targ.radialMenuButtonRadius );
				float innerRadius = outerRadius - ( targ.UltimateRadialButtonList[ 0 ].buttonTransform.sizeDelta.y * parentCanvasRectTrans.localScale.x );

				float lineOuterRadius = ( trans.sizeDelta.x / 2 ) * ( targ.radialMenuButtonRadius );
				float lineInnerRadius = lineOuterRadius - ( targ.UltimateRadialButtonList[ 0 ].buttonTransform.sizeDelta.y );

				Handles.DrawWireDisc( center, Selection.activeGameObject.transform.forward, outerRadius );
				Handles.DrawWireDisc( center, Selection.activeGameObject.transform.forward, innerRadius );

				float angleInRadians = -( 360f / targ.menuButtonCount ) * Mathf.Deg2Rad;
				float halfAngle = ( targ.GetAnglePerButton / 2 ) * Mathf.Deg2Rad;

				for( int i = 0; i < targ.menuButtonCount; i++ )
				{
					Vector3 lineStart = Vector3.zero;
					lineStart.x += ( Mathf.Cos( ( angleInRadians * i ) + ( 90 * Mathf.Deg2Rad ) + halfAngle ) * lineOuterRadius );
					lineStart.y += ( Mathf.Sin( ( angleInRadians * i ) + ( 90 * Mathf.Deg2Rad ) + halfAngle ) * lineOuterRadius );
					Vector3 lineEnd = Vector3.zero;
					lineEnd.x += ( Mathf.Cos( ( angleInRadians * i ) + ( 90 * Mathf.Deg2Rad ) + halfAngle ) * lineInnerRadius );
					lineEnd.y += ( Mathf.Sin( ( angleInRadians * i ) + ( 90 * Mathf.Deg2Rad ) + halfAngle ) * lineInnerRadius );

					lineStart = targ.transform.TransformPoint( lineStart );
					lineEnd = targ.transform.TransformPoint( lineEnd );

					Handles.DrawLine( lineStart, lineEnd );
				}
			}
			else
			{
				for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
					DisplayWireBox( targ.UltimateRadialButtonList[ i ].buttonTransform.localPosition, targ.UltimateRadialButtonList[ i ].buttonTransform.sizeDelta, targ.transform );
			}
		}

		if( ( EditorPrefs.GetBool( "URM_ButtonIcon" ) || EditorPrefs.GetBool( "URM_RadialButtonList" ) ) && targ.useButtonIcon )
			DrawEmptyIcons();

		DisplayMinRange.frames++;
		DisplayMaxRange.frames++;
		DisplayInputAngle.frames++;

		SceneView.RepaintAll();
	}

	void DrawEmptyIcons ()
	{
		Handles.color = colorDefault;
		for( int i = 0; i < targ.UltimateRadialButtonList.Count; i++ )
		{
			if( targ.UltimateRadialButtonList[ i ].icon == null || targ.UltimateRadialButtonList[ i ].icon.sprite != null )
				continue;

			Handles.DrawWireDisc( targ.UltimateRadialButtonList[ i ].iconTransform.position, Selection.activeGameObject.transform.forward, ( targ.UltimateRadialButtonList[ i ].iconTransform.sizeDelta.x * parentCanvasRectTrans.localScale.x ) / 2 );

			GUIStyle labelStyle = new GUIStyle( GUI.skin.label )
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Bold,
			};
			Handles.Label( targ.UltimateRadialButtonList[ i ].iconTransform.position, "Icon", labelStyle );
		}
	}

	void DisplayWireBox ( Vector3 center, Vector2 sizeDelta, Transform trans )
	{
		float halfHeight = sizeDelta.y / 2;
		float halfWidth = sizeDelta.x / 2;

		Vector3 topLeft = center + new Vector3( -halfWidth, halfHeight, 0 );
		Vector3 topRight = center + new Vector3( halfWidth, halfHeight, 0 );
		Vector3 bottomRight = center + new Vector3( halfWidth, -halfHeight, 0 );
		Vector3 bottomLeft = center + new Vector3( -halfWidth, -halfHeight, 0 );

		topLeft = trans.TransformPoint( topLeft );
		topRight = trans.TransformPoint( topRight );
		bottomRight = trans.TransformPoint( bottomRight );
		bottomLeft = trans.TransformPoint( bottomLeft );

		Handles.DrawLine( topLeft, topRight );
		Handles.DrawLine( topRight, bottomRight );
		Handles.DrawLine( bottomRight, bottomLeft );
		Handles.DrawLine( bottomLeft, topLeft );
	}

	void CheckEventSystem ()
	{
		Object esys = FindObjectOfType<EventSystem>();
		if( esys == null )
		{
			GameObject eventSystem = new GameObject( "EventSystem" );
			esys = eventSystem.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
			eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
			eventSystem.AddComponent<StandaloneInputModule>();
#endif
			eventSystem.AddComponent<UltimateRadialMenuInputManager>();

			Undo.RegisterCreatedObjectUndo( eventSystem, "Create " + eventSystem.name );
		}
	}
	
	void RequestCanvas ( GameObject child, bool screenSpaceOverlay = true )
	{
		// Store all canvas objects to check the render mode options.
		UnityEngine.Canvas[] allCanvas = Object.FindObjectsOfType( typeof( UnityEngine.Canvas ) ) as UnityEngine.Canvas[];

		// Loop through each canvas.
		for( int i = 0; i < allCanvas.Length; i++ )
		{
			// If the user wants a screen space canvas...
			if( screenSpaceOverlay )
			{
				// Check to see if this canvas is set to Screen Space and it is enabled. Then set the parent and check for an event system.
				if( allCanvas[ i ].renderMode == RenderMode.ScreenSpaceOverlay && allCanvas[ i ].enabled == true )
				{
					Undo.SetTransformParent( child.transform, allCanvas[ i ].transform, "Update Radial Menu Parent" );
					CheckEventSystem();
					return;
				}
			}
			// Else the user wants a world space canvas...
			else
			{
				// Check to see if this canvas is set to World Space and see if it is enabled. Then set the parent and check for an event system.
				if( allCanvas[ i ].renderMode == RenderMode.WorldSpace && allCanvas[ i ].enabled == true )
				{
					Undo.SetTransformParent( child.transform, allCanvas[ i ].transform, "Update Radial Menu Parent" );
					CheckEventSystem();
					return;
				}
			}
		}

		// If there have been no canvas objects found for this child, then create a new one.
		GameObject newCanvasObject = new GameObject( "Canvas" );
		newCanvasObject.layer = LayerMask.NameToLayer( "UI" );
		UnityEngine.Canvas canvas = newCanvasObject.AddComponent<UnityEngine.Canvas>();
		newCanvasObject.AddComponent<GraphicRaycaster>();

		if( screenSpaceOverlay )
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		else
			canvas.renderMode = RenderMode.WorldSpace;

		Undo.RegisterCreatedObjectUndo( newCanvasObject, "Create New Canvas" );
		Undo.SetTransformParent( child.transform, newCanvasObject.transform, "Create New Canvas" );
		CheckEventSystem();
	}

	[MenuItem( "GameObject/UI/Ultimate Radial Menu" )]
	public static void CreateUltimateRadialMenuFromScratch ()
	{
		GameObject ultimateRadialMenu = new GameObject( "Ultimate Radial Menu" );
		ultimateRadialMenu.AddComponent<RectTransform>();
		ultimateRadialMenu.AddComponent<CanvasGroup>();
		ultimateRadialMenu.AddComponent<UltimateRadialMenu>();

		Selection.activeGameObject = ultimateRadialMenu;
	}
}