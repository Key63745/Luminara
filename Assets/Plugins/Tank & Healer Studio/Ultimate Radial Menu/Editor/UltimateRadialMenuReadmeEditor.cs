/* UltimateRadialMenuReadmeEditor.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
[CustomEditor( typeof( UltimateRadialMenuReadme ) )]
public class UltimateRadialMenuReadmeEditor : Editor
{
	static UltimateRadialMenuReadme readme;

	// LAYOUT STYLES //
	const string Indent =  "    ";
	int sectionSpace = 20;
	int itemHeaderSpace = 10;
	int paragraphSpace = 5;
	GUIStyle titleStyle = new GUIStyle();
	GUIStyle sectionHeaderStyle = new GUIStyle();
	GUIStyle itemHeaderStyle = new GUIStyle();
	GUIStyle paragraphStyle = new GUIStyle();
	GUIStyle versionStyle = new GUIStyle();

	// PAGE INFORMATION //
	class PageInformation
	{
		public string pageName = "";
		public delegate void TargetMethod ();
		public TargetMethod targetMethod;
	}
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation[] AllPages = new PageInformation[]
	{
		// MAIN MENU - 0 //
		new PageInformation()
		{
			pageName = "Product Manual"
		},
		// Getting Started - 1 //
		new PageInformation()
		{
			pageName = "Getting Started"
		},
		// Overview - 2 //
		new PageInformation()
		{
			pageName = "Overview"
		},
		// Documentation - 3 //
		new PageInformation()
		{
			pageName = "Documentation"
		},
		// Documentation - URM - 4 //
		new PageInformation()
		{
			pageName = "Documentation"
		},
		// Documentation - URBI - 5 //
		new PageInformation()
		{
			pageName = "Documentation"
		},
		// Documentation - URMIM - 6 //
		new PageInformation()
		{
			pageName = "Documentation"
		},
		// Version History - 7 //
		new PageInformation()
		{
			pageName = "Version History"
		},
		// Important Change - 8 //
		new PageInformation()
		{
			pageName = "Important Change"
		},
		// Thank You! - 9 //
		new PageInformation()
		{
			pageName = "Thank You!"
		},
		// Settings - 10 //
		new PageInformation()
		{
			pageName = "Settings"
		},
	};

	// OVERVIEW //
	bool showRadialMenuPositioning = false, showRadialMenuOptions = false, showButtonInteraction;
	bool showRadialButtonList = false, showScriptReference = false;

	// DOCUMENTATION //
	class DocumentationInfo
	{
		public string functionName = "";
		public bool showMore = false;
		public string[] parameter;
		public string returnType = "";
		public string description = "";
		public string codeExample = "";
	}
	const string radialMenuName = "OptionsMenu";
	DocumentationInfo[] UltimateRadialMenu_StaticFunctions = new DocumentationInfo[]
	{
		// GetUltimateRadialMenu
		new DocumentationInfo()
		{
			functionName = "GetUltimateRadialMenu()",
			returnType = "UltimateRadialMenu",
			parameter = new string[]
			{
				"string radialMenuName - The registered name of the targeted Ultimate Radial Menu.",
			},
			description = "This function will return the Ultimate Radial Menu component that has been registered with the <i>radialMenuName</i> parameter.",
			codeExample = "UltimateRadialMenu radialMenu = UltimateRadialMenu.GetUltimateRadialMenu( \"" + radialMenuName + "\" );"
		},
		// RegisterToRadialMenu
		new DocumentationInfo()
		{
			functionName = "RegisterToRadialMenu()",
			parameter = new string[]
			{
				"string radialMenuName - The registered name of the targeted Ultimate Radial Menu.",
				"Action(+2 Overloads: <int>, <string>) ButtonCallback - The action callback to be called when the button is interacted with.",
				"UltimateRadialButtonInfo buttonInfo - The button information to apply to the button.",
				"int buttonIndex = -1 - Optional parameter that allow the user to determine where this information will get registered to on the menu. Leave out this parameter to register the provided information to the first available button.",
			},
			description = "Registered the provided information to the targeted Ultimate Radial Menu.",
			codeExample = "UltimateRadialMenu.RegisterToRadialMenu( \"" + radialMenuName + "\", MyCallbackFunction, myButtonInfo );"
		},
		// EnableRadialMenu
		new DocumentationInfo()
		{
			functionName = "EnableRadialMenu()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
			},
			description = "Enables the targeted Ultimate Radial Menu so that it can be interacted with.",
			codeExample = "UltimateRadialMenu.EnableRadialMenu( \"" + radialMenuName + "\" );"
		},
		// DisableRadialMenu
		new DocumentationInfo()
		{
			functionName = "DisableRadialMenu()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
			},
			description = "Disables the targeted Ultimate Radial Menu so that it can not be interacted with.",
			codeExample = "UltimateRadialMenu.DisableRadialMenu( \"" + radialMenuName + "\" );"
		},
		// DisableRadialMenuImmediate
		new DocumentationInfo()
		{
			functionName = "DisableRadialMenuImmediate()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
			},
			description = "Disables the Ultimate Radial Menu immediately so that it can not be interacted with.",
			codeExample = "UltimateRadialMenu.DisableRadialMenuImmediate( \"" + radialMenuName + "\" );"
		},
		// CreateEmptyRadialButton
		new DocumentationInfo()
		{
			functionName = "CreateEmptyRadialButton()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
			},
			description = "Creates an empty radial button at the end of the targeted radial menu.",
			codeExample = "UltimateRadialMenu.CreateEmptyRadialButton( \"" + radialMenuName + "\" );"
		},
		// RemoveAllRadialButtons
		new DocumentationInfo()
		{
			functionName = "RemoveAllRadialButtons()",
			parameter = new string[]
			{
				"string radialMenuName - The registered name of the targeted Ultimate Radial Menu.",
			},
			description = "Deletes all of the radial menu buttons from the radial menu.",
			codeExample = "UltimateRadialMenu.RemoveAllRadialButtons( \"" + radialMenuName + "\" );"
		},
		// RemoveRadialButton
		new DocumentationInfo()
		{
			functionName = "RemoveRadialButton()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
				"int buttonIndex - The index to remove the radial button at.",
			},
			description = "Removes the radial button at the targeted index.",
			codeExample = "UltimateRadialMenu.RemoveRadialButton( \"" + radialMenuName + "\", 0 );"
		},
		// ClearRadialButtonInformations
		new DocumentationInfo()
		{
			functionName = "ClearRadialButtonInformations()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
			},
			description = "Clears the registered button information.",
			codeExample = "UltimateRadialMenu.ClearRadialButtonInformations( \"" + radialMenuName + "\" );"
		},
		// SetPosition
		new DocumentationInfo()
		{
			functionName = "SetPosition()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
				"Vector3 position - The new position on the screen.",
			},
			description = "Updates the targeted radial menu's position to the new position on the screen.",
			codeExample = "UltimateRadialMenu.SetPosition( \"" + radialMenuName + "\", newScreenPosition );"
		},
		// ResetPosition
		new DocumentationInfo()
		{
			functionName = "ResetPosition()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
			},
			description = "Resets the position of the radial menu back to it's original position.",
			codeExample = "UltimateRadialMenu.ResetPosition( \"" + radialMenuName + "\" );"
		},
		// SetParent
		new DocumentationInfo()
		{
			functionName = "SetParent()",
			parameter = new string[]
			{
				"string radialMenuName - The string name that the targeted Ultimate Radial Menu has been registered with.",
				"Transform parent - The new parent transform to put the canvas in.",
				"Vector3 localPosition - The local position of the canvas in the new parent transform.",
				"Quaternion localRotation - The local rotation of the canvas in the new parent transform.",
			},
			description = "Sets the parent of the canvas to the new parent transform.",
			codeExample = "UltimateRadialMenu.SetParent( \"" + radialMenuName + "\", newParentTransform, Vector3.zero, Quaternion.identity );"
		},
	};
	DocumentationInfo[] UltimateRadialMenu_PublicFunctions = new DocumentationInfo[]
	{
		// UpdatePositioning
		new DocumentationInfo()
		{
			functionName = "UpdatePositioning()",
			description = "Updates the positioning of the radial menu according to the user's options.",
			codeExample = "radialMenu.UpdatePositioning();"
		},
		// RegisterToRadialMenu
		new DocumentationInfo()
		{
			functionName = "RegisterToRadialMenu()",
			parameter = new string[]
			{
				"Action(+2 Overloads: <int>, <string>) ButtonCallback - The action callback to be called when the button is interacted with.",
				"UltimateRadialButtonInfo buttonInfo - The button information to apply to the button.",
				"int buttonIndex = -1 - Optional parameter that allow the user to determine where this information will get registered to on the menu. Leave out this parameter to register the provided information to the first available button.",
			},
			description = "Registered the provided information to the targeted Ultimate Radial Menu.",
			codeExample = "radialMenu.RegisterToRadialMenu( MyCallbackFunction, myButtonInfo );"
		},
		// EnableRadialMenu
		new DocumentationInfo()
		{
			functionName = "EnableRadialMenu()",
			description = "Enables the Ultimate Radial Menu so that it can be interacted with.",
			codeExample = "radialMenu.EnableRadialMenu();"
		},
		// DisableRadialMenu
		new DocumentationInfo()
		{
			functionName = "DisableRadialMenu()",
			description = "Disables the Ultimate Radial Menu so that it can not be interacted with.",
			codeExample = "radialMenu.DisableRadialMenu();"
		},
		// DisableRadialMenuImmediate
		new DocumentationInfo()
		{
			functionName = "DisableRadialMenuImmediate()",
			description = "Disables the Ultimate Radial Menu immediately so that it can not be interacted with.",
			codeExample = "radialMenu.DisableRadialMenuImmediate();"
		},
		// CreateEmptyRadialButton
		new DocumentationInfo()
		{
			functionName = "CreateEmptyRadialButton()",
			description = "Creates an empty radial button at the end of the radial menu.",
			codeExample = "radialMenu.CreateEmptyRadialButton();"
		},
		// RemoveAllRadialButtons
		new DocumentationInfo()
		{
			functionName = "RemoveAllRadialButtons()",
			description = "Deletes all of the radial menu buttons from the radial menu.",
			codeExample = "radialMenu.RemoveAllRadialButtons();"
		},
		// RemoveRadialButton
		new DocumentationInfo()
		{
			functionName = "RemoveRadialButton()",
			parameter = new string[]
			{
				"int buttonIndex - The index to remove the radial button at.",
			},
			description = "Removes the radial button at the targeted index.",
			codeExample = "radialMenu.RemoveRadialButton( 0 );"
		},
		// ClearRadialButtonInformations
		new DocumentationInfo()
		{
			functionName = "ClearRadialButtonInformations()",
			description = "Clears the registered button information.",
			codeExample = "radialMenu.ClearRadialButtonInformations();"
		},
		// SetPosition
		new DocumentationInfo()
		{
			functionName = "SetPosition()",
			parameter = new string[]
			{
				"Vector3 position - The new position either on the screen, or in the world.",
				"bool local - [OPTIONAL] Determines whether or not to apply the provided position to local space or not. Defaults to world space.",
			},
			description = "Updates the radial menu's position to the new position on the screen.",
			codeExample = "radialMenu.SetPosition( newScreenPosition );"
		},
		// ResetPosition
		new DocumentationInfo()
		{
			functionName = "ResetPosition()",
			description = "Resets the position of the radial menu back to it's original position.",
			codeExample = "radialMenu.ResetPosition();"
		},
		// SetParent
		new DocumentationInfo()
		{
			functionName = "SetParent()",
			parameter = new string[]
			{
				"Transform parent - The new parent transform to put the canvas in.",
				"Vector3 localPosition - The local position of the canvas in the new parent transform.",
				"Quaternion localRotation - The local rotation of the canvas in the new parent transform.",
			},
			description = "Sets the parent of the canvas to the new parent transform.",
			codeExample = "radialMenu.SetParent( newParentTransform, Vector3.zero, Quaternion.identity );"
		},
	};
	DocumentationInfo[] UltimateRadialButtonInfo_PublicFunctions = new DocumentationInfo[]
	{
		// UpdateIcon
		new DocumentationInfo()
		{
			functionName = "UpdateIcon()",
			parameter = new string[]
			{
				"Sprite newIcon - The new sprite to assign as the icon for the radial button.",
			},
			description = "Assigns a new sprite to the radial button's icon image.",
			codeExample = "buttonInfo.UpdateIcon( myNewIcon );"
		},
		// UpdateText
		new DocumentationInfo()
		{
			functionName = "UpdateText()",
			parameter = new string[]
			{
				"string newText - The new string to apply to the radial button.",
			},
			description = "Applies a new string to the radial button's text component.",
			codeExample = "buttonInfo.UpdateText( \"New Text\" );"
		},
		// UpdateDescription
		new DocumentationInfo()
		{
			functionName = "UpdateDescription()",
			parameter = new string[]
			{
				"string newDescription - The new string to apply to the radial button's description.",
			},
			description = "Updates the radial button with a new description.",
			codeExample = "buttonInfo.UpdateDescription( \"New Description\" );"
		},
		// EnableButton
		new DocumentationInfo()
		{
			functionName = "EnableButton()",
			description = "Enables the radial menu button.",
			codeExample = "buttonInfo.EnableButton();"
		},
		// DisableButton
		new DocumentationInfo()
		{
			functionName = "DisableButton()",
			description = "Disables the radial menu button.",
			codeExample = "buttonInfo.DisableButton();"
		},
		// SelectButton
		new DocumentationInfo()
		{
			functionName = "SelectButton()",
			description = "Selects this button.",
			codeExample = "buttonInfo.SelectButton();"
		},
		// DeselectButton
		new DocumentationInfo()
		{
			functionName = "DeselectButton()",
			description = "Deselects this button.",
			codeExample = "buttonInfo.DeselectButton();"
		},
		// RemoveRadialButton
		new DocumentationInfo()
		{
			functionName = "RemoveRadialButton()",
			description = "Deletes the radial menu button.",
			codeExample = "buttonInfo.RemoveRadialButton();"
		},
		// ExistsOnRadialMenu
		new DocumentationInfo()
		{
			functionName = "ExistsOnRadialMenu()",
			returnType = "bool",
			description = "Returns the existence of this information on a radial menu.",
			codeExample = "if( buttonInfo.ExistsOnRadialMenu() )\n{\n" + Indent + "// The buttonInfo is assigned to a radial menu, do something here...\n}"
		},
		// RemoveInfoFromRadialButton
		new DocumentationInfo()
		{
			functionName = "RemoveInfoFromRadialButton()",
			description = "Removes this information from the current radial button.",
			codeExample = "buttonInfo.RemoveInfoFromRadialButton();"
		},
	};
	DocumentationInfo[] UltimateRadialMenu_Events = new DocumentationInfo[]
	{
		// OnRadialButtonEnter
		new DocumentationInfo()
		{
			functionName = "OnRadialButtonEnter",
			parameter = new string[]
			{
				"int - The index of the radial button that was entered.",
			},
			description = "This event is called when a new radial button is entered.",
			codeExample = "radialMenu.OnRadialButtonEnter += YourOnRadialButtonEnterFunction;",
		},
		// OnRadialButtonExit
		new DocumentationInfo()
		{
			functionName = "OnRadialButtonExit",
			parameter = new string[]
			{
				"int - The index of the radial button that was exited.",
			},
			description = "This event is called when the input exits the radial button that recently had focus.",
			codeExample = "radialMenu.OnRadialButtonExit += YourOnRadialButtonExitFunction;",
		},
		// OnRadialButtonInputDown
		new DocumentationInfo()
		{
			functionName = "OnRadialButtonInputDown",
			parameter = new string[]
			{
				"int - The index of the radial button that received the down input.",
			},
			description = "This callback will be called when the input has been pressed on a radial button.",
			codeExample = "radialMenu.OnRadialButtonInputDown += YourOnRadialButtonInputDownFunction;",
		},
		// OnRadialButtonInputUp
		new DocumentationInfo()
		{
			functionName = "OnRadialButtonInputUp",
			parameter = new string[]
			{
				"int - The index of the radial button that received the up input.",
			},
			description = "This callback will be called when the input has been released on a radial button.",
			codeExample = "radialMenu.OnRadialButtonInputUp += YourOnRadialButtonInputUpFunction;",
		},
		// OnRadialButtonInteract
		new DocumentationInfo()
		{
			functionName = "OnRadialButtonInteract",
			parameter = new string[]
			{
				"int - The index of the radial button that has been interacted with.",
			},
			description = "This callback will be called when a radial button has been interacted with.",
			codeExample = "radialMenu.OnRadialButtonInteract += YourOnRadialButtonInteractFunction;",
		},
		// OnRadialButtonSelected
		new DocumentationInfo()
		{
			functionName = "OnRadialButtonSelected",
			parameter = new string[]
			{
				"int - The index of the radial button that has been selected.",
			},
			description = "This callback will be called with a radial menu has been selected.",
			codeExample = "radialMenu.OnRadialButtonSelected += YourOnRadialButtonSelectedFunction;",
		},
		// OnRadialMenuLostFocus
		new DocumentationInfo()
		{
			functionName = "OnRadialMenuLostFocus",
			description = "This callback will be called when the radial menu has lost focus.",
			codeExample = "radialMenu.OnRadialMenuLostFocus += YourOnRadialMenuLostFocusFunction;",
		},
		// OnRadialMenuEnabled
		new DocumentationInfo()
		{
			functionName = "OnRadialMenuEnabled",
			description = "This callback will be called when the radial menu has been enabled.",
			codeExample = "radialMenu.OnRadialMenuEnabled += YourOnRadialMenuEnabledFunction;",
		},
		// OnRadialMenuDisabled
		new DocumentationInfo()
		{
			functionName = "OnRadialMenuDisabled",
			description = "This callback will be called when the radial menu has been disabled.",
			codeExample = "radialMenu.OnRadialMenuDisabled += YourOnRadialMenuDisabledFunction;",
		},
		// OnUpdatePositioning
		new DocumentationInfo()
		{
			functionName = "OnUpdatePositioning",
			description = "This callback will be called when the radial menu's positioning has been updated.",
			codeExample = "radialMenu.OnUpdatePositioning += YourOnUpdatePositioningFunction;",
		},
		// OnRadialMenuButtonCountModified
		new DocumentationInfo()
		{
			functionName = "OnRadialMenuButtonCountModified",
			parameter = new string[]
			{
				"int - The current number of buttons in the menu after the modification.",
			},
			description = "This callback will be called whenever a radial button is added or subtracted from the Radial Menu. This is useful for swapping sprites and positioning for a new count.",
			codeExample = "radialMenu.OnRadialMenuButtonCountModified += YourOnRadialMenuButtonCountModifiedFunction;",
		},
	};
	DocumentationInfo[] UltimateRadialMenuInputManager_PublicFunctions = new DocumentationInfo[]
	{
		// SetMainCamera
		new DocumentationInfo()
		{
			functionName = "SetMainCamera()",
			parameter = new string[]
			{
				"Camera newMainCamera - The new camera to use for calculations.",
			},
			description = "Sets the camera to the provided camera parameter for calculations.",
			codeExample = "UltimateRadialMenuInputManager.Instance.SetMainCamera( newMainCamera );"
		},
	};

	int ultimateRadialMenuPublicSearchIndex = 0;
	int ultimateRadialMenuStaticSearchIndex = 0;
	int ultimateRadialButtonInfoSearchIndex = 0;
	int ultimateRadialMenuEventsSearchIndex = 0;
	int ultimateRadialMenuInputManagerSearchIndex = 0;
	List<DocumentationInfo> SearchResults = new List<DocumentationInfo>();
	string searchValue = "";
	
	// END PAGE COMMENTS //
	class EndPageComment
	{
		public string comment = "";
		public string url = "";
	}
	EndPageComment[] endPageComments = new EndPageComment[]
	{
		new EndPageComment()
		{
			comment = "Enjoying the Ultimate Radial Menu? Leave us a review on the <b><color=blue>Unity Asset Store</color></b>!",
			url = "https://assetstore.unity.com/packages/slug/137681"
		},
		new EndPageComment()
		{
			comment = "Looking for a mobile joystick for your game? Check out the <b><color=blue>Ultimate Joystick</color></b>!",
			url = "https://www.tankandhealerstudio.com/ultimate-joystick.html"
		},
		new EndPageComment()
		{
			comment = "Looking for a health bar for your game? Check out the <b><color=blue>Ultimate Status Bar</color></b>!",
			url = "https://www.tankandhealerstudio.com/ultimate-status-bar.html"
		},
		new EndPageComment()
		{
			comment = "Check out our <b><color=blue>other products</color></b>!",
			url = "https://www.tankandhealerstudio.com/assets.html"
		},
	};
	int randomComment = 0;
	

	static UltimateRadialMenuReadmeEditor ()
	{
		// Add the WaitForCompile function to the editor application update callback.
		EditorApplication.update += WaitForCompile;
	}

	static void WaitForCompile ()
	{
		// If the editor is compiling then just return.
		if( EditorApplication.isCompiling )
			return;

		// Remove the WaitForCompile function from the editor application update.
		EditorApplication.update -= WaitForCompile;
		
		// If this is the first time that the user has downloaded the Ultimate Radial Menu...
		if( !EditorPrefs.HasKey( "UltimateRadialMenuVersion" ) )
		{
			// Set the version to current so they won't see these version changes.
			EditorPrefs.SetInt( "UltimateRadialMenuVersion", UltimateRadialMenuReadme.ImportantChange );
			
			// Select the readme file.
			SelectReadmeFile();

			// If the readme file is assigned, then navigate to the Thank You page.
			if( readme != null )
				NavigateForward( 9 );
		}
		// Else the user has downloaded the radial menu before, so if the version recorded is less than the important change...
		else if( EditorPrefs.GetInt( "UltimateRadialMenuVersion" ) < UltimateRadialMenuReadme.ImportantChange )
		{
			// Set the version to current so they won't see this page again.
			EditorPrefs.SetInt( "UltimateRadialMenuVersion", UltimateRadialMenuReadme.ImportantChange );
			
			// Select the readme file.
			SelectReadmeFile();

			// If the readme file is assigned, then navigate to the Version Changes page.
			if( readme != null )
				NavigateForward( 8 );
		}
	}
	
	void OnEnable ()
	{
		readme = ( UltimateRadialMenuReadme )target;
		
		if( !EditorPrefs.HasKey( "URM_ColorHexSetup" ) )
		{
			EditorPrefs.SetBool( "URM_ColorHexSetup", true );
			ResetColors();
		}

		ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorDefaultHex" ), out readme.colorDefault );
		ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorValueChangedHex" ), out readme.colorValueChanged );
		ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorButtonSelectedHex" ), out readme.colorButtonSelected );
		ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorButtonUnselectedHex" ), out readme.colorButtonUnselected );
		ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "URM_ColorTextBoxHex" ), out readme.colorTextBox );
		
		AllPages[ 0 ].targetMethod = MainPage;
		AllPages[ 1 ].targetMethod = GettingStarted;
		AllPages[ 2 ].targetMethod = Overview;
		AllPages[ 3 ].targetMethod = Documentation;
		AllPages[ 4 ].targetMethod = Documentation_UltimateRadialMenu;
		AllPages[ 5 ].targetMethod = Documentation_UltimateRadialButtonInfo;
		AllPages[ 6 ].targetMethod = Documentation_UltimateRadialMenuInputManager;
		AllPages[ 7 ].targetMethod = VersionHistory;
		AllPages[ 8 ].targetMethod = ImportantChange;
		AllPages[ 9 ].targetMethod = ThankYou;
		AllPages[ 10 ].targetMethod = Settings;

		pageHistory = new List<PageInformation>();
		for( int i = 0; i < readme.pageHistory.Count; i++ )
			pageHistory.Add( AllPages[ readme.pageHistory[ i ] ] );

		if( !pageHistory.Contains( AllPages[ 0 ] ) )
		{
			pageHistory.Insert( 0, AllPages[ 0 ] );
			readme.pageHistory.Insert( 0, 0 );
		}
		
		randomComment = Random.Range( 0, endPageComments.Length );
		searchValue = "";
		
		Undo.undoRedoPerformed += UndoRedoCallback;
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}
	
	void UndoRedoCallback ()
	{
		if( pageHistory[ pageHistory.Count - 1 ] != AllPages[ 10 ] )
			return;
		
		EditorPrefs.SetString( "URM_ColorDefaultHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorDefault ) );
		EditorPrefs.SetString( "URM_ColorValueChangedHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorValueChanged ) );
		EditorPrefs.SetString( "URM_ColorButtonSelectedHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorButtonSelected ) );
		EditorPrefs.SetString( "URM_ColorButtonUnselectedHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorButtonUnselected ) );
		EditorPrefs.SetString( "URM_ColorTextBoxHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorTextBox ) );
	}

	protected override void OnHeaderGUI ()
	{
		UltimateRadialMenuReadme readme = ( UltimateRadialMenuReadme )target;

		var iconWidth = Mathf.Min( EditorGUIUtility.currentViewWidth, 350f );

		Vector2 ratio = new Vector2( readme.icon.width, readme.icon.height ) / ( readme.icon.width > readme.icon.height ? readme.icon.width : readme.icon.height );

		GUILayout.BeginHorizontal( "In BigTitle" );
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label( readme.icon, GUILayout.Width( iconWidth * ratio.x ), GUILayout.Height( iconWidth * ratio.y ) );
			GUILayout.Space( -20 );
			if( GUILayout.Button( readme.versionHistory[ 0 ].versionNumber, versionStyle ) && !pageHistory.Contains( AllPages[ 7 ] ) )
				NavigateForward( 7 );
			var rect = GUILayoutUtility.GetLastRect();
			if( pageHistory[ pageHistory.Count - 1 ] != AllPages[ 7 ] )
				EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		paragraphStyle = new GUIStyle( EditorStyles.label ) { wordWrap = true, richText = true, fontSize = 12 };
		itemHeaderStyle = new GUIStyle( paragraphStyle ) { fontSize = 12, fontStyle = FontStyle.Bold };
		sectionHeaderStyle = new GUIStyle( paragraphStyle ) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		titleStyle = new GUIStyle( paragraphStyle ) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		versionStyle = new GUIStyle( paragraphStyle ) { alignment = TextAnchor.MiddleCenter, fontSize = 10 };

		paragraphStyle.active.textColor = paragraphStyle.normal.textColor;

		// SETTINGS BUTTON //
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( readme.settings, versionStyle, GUILayout.Width( 24 ), GUILayout.Height( 24 ) ) && !pageHistory.Contains( AllPages[ 10 ] ) )
			NavigateForward( 10 );
		var rect = GUILayoutUtility.GetLastRect();
		if( pageHistory[ pageHistory.Count - 1 ] != AllPages[ 10 ] )
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		GUILayout.EndHorizontal();
		GUILayout.Space( -24 );
		GUILayout.EndVertical();

		// BACK BUTTON //
		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginDisabledGroup( pageHistory.Count <= 1 );
		if( GUILayout.Button( "◄", titleStyle, GUILayout.Width( 24 ) ) )
			NavigateBack();
		if( pageHistory.Count > 1 )
		{
			rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		}
		EditorGUI.EndDisabledGroup();
		GUILayout.Space( -24 );

		// PAGE TITLE //
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( pageHistory[ pageHistory.Count - 1 ].pageName, titleStyle );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		// DISPLAY PAGE //
		if( pageHistory[ pageHistory.Count - 1 ].targetMethod != null )
			pageHistory[ pageHistory.Count - 1 ].targetMethod();

		Repaint();
	}

	void StartPage ()
	{
		readme.scrollValue = EditorGUILayout.BeginScrollView( readme.scrollValue, false, false );
		GUILayout.Space( 15 );
	}

	void EndPage ()
	{
		EditorGUILayout.EndScrollView();
	}

	static void NavigateBack ()
	{
		readme.pageHistory.RemoveAt( readme.pageHistory.Count - 1 );
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		GUI.FocusControl( "" );

		readme.scrollValue = Vector2.zero;

		if( readme.pageHistory.Count == 1 )
			EditorUtility.SetDirty( readme );
	}

	static void NavigateForward ( int menuIndex )
	{
		pageHistory.Add( AllPages[ menuIndex ] );
		GUI.FocusControl( "" );
		
		readme.pageHistory.Add( menuIndex );
		readme.scrollValue = Vector2.zero;
	}

	void MainPage ()
	{
		StartPage();

		EditorGUILayout.LabelField( "We hope that you are enjoying using the Ultimate Radial Menu in your project! Here is a list of helpful resources for this asset:", paragraphStyle );
		
		EditorGUILayout.Space();

		if( GUILayout.Button( "  • Read the <b><color=blue>Getting Started</color></b> section of this README!", paragraphStyle ) )
			NavigateForward( 1 );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		
		EditorGUILayout.Space();

		if( GUILayout.Button( "  • To learn more about the sections of the inspector, read the <b><color=blue>Overview</color></b> section!", paragraphStyle ) )
			NavigateForward( 2 );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • Check out the <b><color=blue>Documentation</color></b> section!", paragraphStyle ) )
			NavigateForward( 3 );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		
		EditorGUILayout.Space();

		if( GUILayout.Button( "  • Watch our <b><color=blue>Video Tutorials</color></b> on the Ultimate Radial Menu!", paragraphStyle ) )
		{
			Debug.Log( "Ultimate Radial Menu\nOpening YouTube Tutorials" );
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu" );
		}
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • Join our <b><color=blue>Discord Server</color></b> so that you can get live help from us and other community members.", paragraphStyle ) )
		{
			Debug.Log( "Ultimate Radial Menu\nOpening Tank & Healer Studio Discord Server" );
			Application.OpenURL( "https://discord.gg/YrtEHRqw6y" );
		}
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.Space();

		if( GUILayout.Button( "  • <b><color=blue>Contact Us</color></b> directly with your issue! We'll try to help you out as much as we can.", paragraphStyle ) )
		{
			Debug.Log( "Ultimate Radial Menu\nOpening Online Contact Form" );
			Application.OpenURL( "https://www.tankandhealerstudio.com/contact-us.html" );
		}
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		
		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Now you have the tools you need to get the Ultimate Radial Menu working in your project. Now get out there and make your awesome game!", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", paragraphStyle );

		GUILayout.Space( 20 );

		GUILayout.FlexibleSpace();

		if( GUILayout.Button( endPageComments[ randomComment ].comment, paragraphStyle ) )
			Application.OpenURL( endPageComments[ randomComment ].url );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		
		EndPage();
	}

	void GettingStarted ()
	{
		StartPage();

		EditorGUILayout.LabelField( "Video Introduction", sectionHeaderStyle );

		if( GUILayout.Button( Indent + "To begin, please watch the <b><color=blue>Introduction Video</color></b> from our website for the Ultimate Radial Menu. This video will explain how to get started using the Ultimate Radial Menu and help you understand how to implement it into your project.", paragraphStyle ) )
		{
			Debug.Log( "Ultimate Radial Menu\nOpening Online Video Tutorials" );
			Application.OpenURL( "https://www.tankandhealerstudio.com/ultimate-radial-menu_documentation_video-tutorials.html" );
		}
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Written Introduction", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "The Ultimate Radial Menu has been built from the ground up with being easy to use and customize to make it work the way that you want. However, that being said, the Ultimate Radial Menu asset can be a bit tricky to understand how to use at first.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "To begin we'll look at how to simply create an Ultimate Radial Menu in your scene. After that we will go over how to reference the Ultimate Radial Menu in your custom scripts. Let's begin!", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "To create an Ultimate Radial Menu in your scene, simply find the Ultimate Radial Menu prefab that you would like to add and drag the prefab into the scene. The Ultimate Radial Menu prefab will automatically find or create a canvas in your scene for you.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Prefabs can be found at: Assets/Ultimate Radial Menu/Prefabs.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To understand how to use the Ultimate Radial Menu into your scripts, first let us talk about how it actually works to see how we can best implement the radial menu. We will be going over the <b>Callback System</b> that the radial menu uses as well as a certain class that you will be using to send information to the radial button. Below are the topics mentioned.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Callback System", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Callback System simply sends information to your scripts about which button has been interacted with. When you get to implementing the code, you will see a parameter named: ButtonCallback. This is where you will pass your function that you want the button to call when being interacted with.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For example, let's say you want to use the radial menu as a pause menu. Likely, in your pause menu script, you have a function named: PauseGame(), or something similar. When subscribing to the pause button on your radial menu, you will want to pass your PauseGame function as the ButtonCallback parameter. Then, when the user clicks on the pause button the Ultimate Radial Menu will call your PauseGame function and pause your game.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "UltimateRadialButtonInfo Class", itemHeaderStyle );
		EditorGUILayout.LabelField( "The UltimateRadialButtonInfo class is public and will be used inside of your own custom scripts. It is used just like any other variable inside of your own scripts, but has a custom property drawer so that the information is a little easier to work with.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "This class is what you will send to the Ultimate Radial Menu to update the information like: Name, Description, Key, ID, Icon, and so forth. After sending in your UltimateRadialButtonInfo to the radial menu, it will then have access to a few functions that you can use to keep information updated, without referencing back to the Ultimate Radial Menu.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For instance, let's continue on the example above with using the radial menu as a pause menu. In your variables, you will want to have a public UltimateRadialButtonInfo variable.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Example Code:", itemHeaderStyle );
		EditorGUILayout.TextArea( "public UltimateRadialButtonInfo pauseButtonInfo;", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "After customizing the variable in the inspector you will want to past it to the radial menu. Below is some example code in a Start() function to help demonstrate how it works:", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Example Code:", itemHeaderStyle );
		EditorGUILayout.TextArea( "void Start ()\n{\n" + Indent + "// Call the UpdateRadialButton function on the PauseMenu radial menu, and pass in the PauseGame function as the ButtonCallback and then the pauseButtonInfo as the new button information to the first index of the pause menu buttons.\n" + Indent + "UltimateRadialMenu.RegisterToRadialMenu( \"PauseMenu \", PauseGame, pauseButtonInfo );\n}", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( paragraphSpace );
		
		EditorGUILayout.LabelField( "After this code, the radial menu's first index button will be your pause button, and will call the PauseGame function when being interacted with.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "We are not done though! The UltimateRadialButtonInfo class has so much more functionality than first meets the eye. After passing in the pauseButtonInfo to the radial menu, it has actually been authorized control over the radial button that it has been assigned to, giving you access to more useful functions to keep the button updated.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Let's discuss one more example of how to update the radial button using this class. In the same scenario, inside your PauseGame() function, now you can update the icon of the button by simply using the pauseButtonInfo class. Let's see how:", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Example Code:", itemHeaderStyle );
		EditorGUILayout.TextArea( "void PauseGame ()\n{\n" + Indent + "// Here is where your custom logic would be to pause the game, but this is just a simple way to pause your game.\n\n" + Indent + "// If the timescale is 1, then the game is playing right now...\n" + Indent + "if( Time.timeScale == 1.0f )\n" + Indent + "{\n" + Indent + Indent + "// So set the timescale to 0 to pause the game.\n" + Indent + Indent + "Time.timeScale = 0.0f;\n\n" + Indent + Indent + "// Since the game is now paused, update the icon to being a play button, instead of pause.\n" + Indent + Indent + "pauseButtonInfo.UpdateIcon( playButtonIcon );\n\n" + Indent + Indent + "// Update the text of the button to say \"Play\" now instead of pause.\n" + Indent + Indent + "pauseButtonInfo.UpdateText( \"Play\" );\n" + Indent + "}\n" + Indent + "// Else the timescale is not 1, and therefore is paused...\n" + Indent + "else\n" + Indent + "{\n" + Indent + Indent + "// So set the timescale to 1 to play the game again.\n" + Indent + Indent + "Time.timeScale = 1.0f;\n\n" + Indent + Indent + "// Since the game is playing again, update the icon back to being a pause button.\n" + Indent + Indent + "pauseButtonInfo.UpdateIcon( pauseButtonIcon );\n\n" + Indent + Indent + "// Update the text to now say \"Pause\" instead of play.\n" + Indent + Indent + "pauseButtonInfo.UpdateText( \"Pause\" );\n" + Indent + "}\n}", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "As you can see, the pauseButtonInfo can now be used to update information about the button that it has been assigned.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		if( GUILayout.Button( "For a full list of the functions available, please see the <b><color=blue>Documentation</color></b> section of this README.", paragraphStyle ) )
			NavigateForward( 3 );

		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EndPage();
	}

	void Overview ()
	{
		StartPage();

		EditorGUILayout.LabelField( "Sections", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The display below is mimicking the Ultimate Radial Menu inspector. Expand each section to learn what each one is designed for.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// RADIAL MENU POSITIONING //
		GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11 };
		
		showRadialMenuPositioning = GUILayout.Toggle( showRadialMenuPositioning, ( showRadialMenuPositioning ? "▼" : "►" ) + "Radial Menu Positioning", toolbarStyle );
		if( showRadialMenuPositioning )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "This section handles the positioning of the Ultimate Radial Menu, as well as the orbit distance and orientation of each button that is a part of the menu.", paragraphStyle );
		}
		
		EditorGUILayout.Space();

		// RADIAL MENU OPTIONS //
		showRadialMenuOptions = GUILayout.Toggle( showRadialMenuOptions, ( showRadialMenuOptions ? "▼" : "►" ) + "Radial Menu Options", toolbarStyle );
		if( showRadialMenuOptions )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "The options in this section will affect the menu as a whole and the buttons collectively. The options here for the buttons determine the game objects used for the radial buttons.", paragraphStyle );
		}

		EditorGUILayout.Space();

		// BUTTON INTERACTION //
		showButtonInteraction = GUILayout.Toggle( showButtonInteraction, ( showButtonInteraction ? "▼" : "►" ) + "Button Interaction", toolbarStyle );
		if( showButtonInteraction )
		{
			GUILayout.Space( paragraphSpace );

			EditorGUILayout.LabelField( "The settings in this section determine how the different states of the radial buttons look when being interacted with.", paragraphStyle );
		}

		EditorGUILayout.Space();

		// RADIAL BUTTON LIST //
		showRadialButtonList = GUILayout.Toggle( showRadialButtonList, ( showRadialButtonList ? "▼" : "►" ) + "Radial Button List", toolbarStyle );
		if( showRadialButtonList )
		{
			GUILayout.Space( paragraphSpace );
			
			EditorGUILayout.LabelField( "Here is where you can customize and edit each button individually.", paragraphStyle );
		}

		EditorGUILayout.Space();

		// SCRIPT REFERENCE //
		showScriptReference = GUILayout.Toggle( showScriptReference, ( showScriptReference ? "▼" : "►" ) + "Script Reference", toolbarStyle );
		if( showScriptReference )
		{
			GUILayout.Space( paragraphSpace );
			EditorGUILayout.LabelField( "In this section you will be able to setup the reference to this Ultimate Radial Menu, and you will be provided with code examples to be able to copy and paste into your own scripts.", paragraphStyle );
		}

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Tooltips", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "To learn more about each option in these sections, please select the Ultimate Radial Menu in your scene, and hover over each item to read the provided tooltip.", paragraphStyle );

		EndPage();
	}

	void Documentation ()
	{
		StartPage();

		EditorGUILayout.LabelField( "Introduction", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "Welcome to the Documentation section. This section will go over the various functions that you have available. Please click on the class to learn more about each function.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginChangeCheck();
		searchValue = EditorGUILayout.TextField( "Search Documentation", searchValue );
		EditorGUILayout.EndHorizontal();
		if( EditorGUI.EndChangeCheck() )
			DocumentationSearchResults();
		
		GUILayout.Space( sectionSpace );

		if( searchValue != string.Empty )
		{
			bool hasSearchHeader = false;
			EditorGUILayout.LabelField( "Search Results:", sectionHeaderStyle );
			if( SearchResults.Count > 0 )
			{
				for( int i = 0; i < SearchResults.Count; i++ )
				{
					if( i == ultimateRadialMenuPublicSearchIndex )
					{
						EditorGUILayout.LabelField( "<b>▼UltimateRadialMenu.cs▼</b>\n<i>Public Functions Listed Below</i>", paragraphStyle );
						hasSearchHeader = true;
					}
					else if( i == ultimateRadialMenuStaticSearchIndex )
					{
						if( hasSearchHeader )
							EditorGUILayout.Space();

						EditorGUILayout.LabelField( "<b>▼UltimateRadialMenu.cs▼</b>\n<i>Static Functions Listed Below</i>", paragraphStyle );
						hasSearchHeader = true;
					}
					else if( i == ultimateRadialButtonInfoSearchIndex )
					{
						if( hasSearchHeader )
							EditorGUILayout.Space();

						EditorGUILayout.LabelField( "<b>▼UltimateRadialButtonInfo.cs▼</b>\n<i>Public Functions Listed Below</i>", paragraphStyle );
						hasSearchHeader = true;
					}
					else if( i == ultimateRadialMenuEventsSearchIndex )
					{
						if( hasSearchHeader )
							EditorGUILayout.Space();

						EditorGUILayout.LabelField( "<b>▼UltimateRadialMenu.cs▼</b>\n<i>Events Listed Below</i>", paragraphStyle );
						hasSearchHeader = true;
					}
					else if( i == ultimateRadialMenuInputManagerSearchIndex )
					{
						if( hasSearchHeader )
							EditorGUILayout.Space();

						EditorGUILayout.LabelField( "<b>▼UltimateRadialMenuInputManager.cs▼</b>\n<i>Public Functions Listed Below</i>", paragraphStyle );
						hasSearchHeader = true;
					}

					ShowDocumentation( SearchResults[ i ] );
				}
			}
		}
		else
		{
			// UltimateRadialMenu.cs
			if( GUILayout.Button( "UltimateRadialMenu.cs", itemHeaderStyle ) )
			{
				NavigateForward( 4 );
				GUI.FocusControl( "" );
			}
			var rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

			// UltimateRadialButtonInfo.cs
			if( GUILayout.Button( "UltimateRadialButtonInfo.cs", itemHeaderStyle ) )
			{
				NavigateForward( 5 );
				GUI.FocusControl( "" );
			}
			rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

			// UltimateRadialMenuInputManager.cs
			if( GUILayout.Button( "UltimateRadialMenuInputManager.cs", itemHeaderStyle ) )
			{
				NavigateForward( 6 );
				GUI.FocusControl( "" );
			}
			rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		}

		EndPage();
	}

	void DocumentationSearchResults ()
	{
		SearchResults = new List<DocumentationInfo>();
		ultimateRadialMenuPublicSearchIndex = -1;
		ultimateRadialMenuStaticSearchIndex = -1;

		if( searchValue == string.Empty )
			return;

		// PUBLIC FUNCTIONS //
		bool codeFound = false;
		ultimateRadialMenuPublicSearchIndex = 0;
		for( int i = 0; i < UltimateRadialMenu_PublicFunctions.Length; i++ )
			CheckDocumentationForRelevance( UltimateRadialMenu_PublicFunctions[ i ], ref codeFound );

		if( !codeFound )
			ultimateRadialMenuPublicSearchIndex = -1;

		// STATIC FUNCTIONS //
		codeFound = false;
		ultimateRadialMenuStaticSearchIndex = SearchResults.Count;
		for( int i = 0; i < UltimateRadialMenu_StaticFunctions.Length; i++ )
			CheckDocumentationForRelevance( UltimateRadialMenu_StaticFunctions[ i ], ref codeFound );

		if( !codeFound )
			ultimateRadialMenuStaticSearchIndex = -1;

		// BUTTON INFO PUBLIC FUNCTIONS //
		codeFound = false;
		ultimateRadialButtonInfoSearchIndex = SearchResults.Count;
		for( int i = 0; i < UltimateRadialButtonInfo_PublicFunctions.Length; i++ )
			CheckDocumentationForRelevance( UltimateRadialButtonInfo_PublicFunctions[ i ], ref codeFound );

		if( !codeFound )
			ultimateRadialButtonInfoSearchIndex = -1;

		// EVENTS //
		codeFound = false;
		ultimateRadialMenuEventsSearchIndex = SearchResults.Count;
		for( int i = 0; i < UltimateRadialMenu_Events.Length; i++ )
			CheckDocumentationForRelevance( UltimateRadialMenu_Events[ i ], ref codeFound );
		if( !codeFound )
			ultimateRadialMenuEventsSearchIndex = -1;

		// INPUT MANAGER PUBLIC FUNCTIONS //
		codeFound = false;
		ultimateRadialMenuInputManagerSearchIndex = SearchResults.Count;
		for( int i = 0; i < UltimateRadialMenuInputManager_PublicFunctions.Length; i++ )
			CheckDocumentationForRelevance( UltimateRadialMenuInputManager_PublicFunctions[ i ], ref codeFound );
		if( !codeFound )
			ultimateRadialMenuInputManagerSearchIndex = -1;
	}

	void CheckDocumentationForRelevance ( DocumentationInfo info, ref bool codeFound )
	{
		// Check function name.
		if( info.functionName.IndexOf( searchValue, System.StringComparison.OrdinalIgnoreCase ) >= 0 )
		{
			SearchResults.Add( info );
			codeFound = true;
			return;
		}

		// Check function description.
		if( info.description.IndexOf( searchValue, System.StringComparison.OrdinalIgnoreCase ) >= 0 )
		{
			SearchResults.Add( info );
			codeFound = true;
			return;
		}

		// Parameter
		if( info.parameter != null && info.parameter.Length > 0 )
		{
			for( int n = 0; n < info.parameter.Length; n++ )
			{
				if( info.parameter[ n ].IndexOf( searchValue, System.StringComparison.OrdinalIgnoreCase ) >= 0 )
				{
					SearchResults.Add( info );
					codeFound = true;
					return;
				}
			}
		}

		// Return type
		if( info.returnType.IndexOf( searchValue, System.StringComparison.OrdinalIgnoreCase ) >= 0 )
		{
			SearchResults.Add( info );
			return;
		}
	}

	void Documentation_UltimateRadialMenu ()
	{
		StartPage();

		// STATIC FUNCTIONS //
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "The following functions can be referenced from your scripts without the need for an assigned local Ultimate Radial Menu variable. However, each function must have the targeted Ultimate Radial Menu name in order to find the correct Ultimate Radial Menu in the scene. Each example code provided uses the name <i>" + radialMenuName + "</i> as the Radial Menu name.", paragraphStyle );

		Vector2 ratio = new Vector2( readme.scriptReference.width, readme.scriptReference.height ) / ( readme.scriptReference.width > readme.scriptReference.height ? readme.scriptReference.width : readme.scriptReference.height );

		float imageWidth = readme.scriptReference.width > Screen.width - 50 ? Screen.width - 50 : readme.scriptReference.width;

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( readme.scriptReference, GUILayout.Width( imageWidth * ratio.x ), GUILayout.Height( imageWidth ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateRadialMenu_StaticFunctions.Length; i++ )
			ShowDocumentation( UltimateRadialMenu_StaticFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		// PUBLIC FUNCTIONS //
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Ultimate Radial Menu class. Each example provided relies on having a Ultimate Radial Menu variable named 'radialMenu' stored inside your script. When using any of the example code provided, make sure that you have a Ultimate Radial Menu variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place this in your public variables and assign it in the inspector. //\npublic UltimateRadialMenu radialMenu;", GUI.skin.textArea );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateRadialMenu_PublicFunctions.Length; i++ )
			ShowDocumentation( UltimateRadialMenu_PublicFunctions[ i ] );
		
		GUILayout.Space( sectionSpace );

		// EVENTS //
		EditorGUILayout.LabelField( "Events", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "These events are called when certain actions are performed on the radial menu. By subscribing a function to these events you will be notified with the particular action is performed.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateRadialMenu_Events.Length; i++ )
			ShowDocumentation( UltimateRadialMenu_Events[ i ] );

		EndPage();
	}

	void Documentation_UltimateRadialButtonInfo ()
	{
		StartPage();

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to an UltimateRadialButtonInfo class:", paragraphStyle );

		EditorGUILayout.TextArea( "// Place these with your variables.\npublic UltimateRadialButtonInfo buttonInfo;", GUI.skin.textArea );

		EditorGUILayout.LabelField( Indent + "The examples provided rely on having an UltimateRadialButtonInfo variable named 'buttonInfo' stored inside your script. Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateRadialButtonInfo_PublicFunctions.Length; i++ )
			ShowDocumentation( UltimateRadialButtonInfo_PublicFunctions[ i ] );

		GUILayout.Space( itemHeaderSpace );

		EndPage();
	}

	void Documentation_UltimateRadialMenuInputManager ()
	{
		StartPage();

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are available from the Instance of the UltimateRadialMenuInputManager gameObject.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < UltimateRadialMenuInputManager_PublicFunctions.Length; i++ )
			ShowDocumentation( UltimateRadialMenuInputManager_PublicFunctions[ i ] );

		GUILayout.Space( itemHeaderSpace );

		EndPage();
	}

	void VersionHistory ()
	{
		StartPage();

		for( int i = 0; i < readme.versionHistory.Length; i++ )
		{
			EditorGUILayout.LabelField( "Version " + readme.versionHistory[ i ].versionNumber, itemHeaderStyle );

			for( int n = 0; n < readme.versionHistory[ i ].changes.Length; n++ )
				EditorGUILayout.LabelField( "• " + readme.versionHistory[ i ].changes[ n ] + ".", paragraphStyle );

			if( i < ( readme.versionHistory.Length - 1 ) )
				GUILayout.Space( itemHeaderSpace );
		}

		EndPage();
	}

	void ImportantChange ()
	{
		StartPage();

		EditorGUILayout.LabelField( Indent + "Thank you for downloading the most recent version of the Ultimate Radial Menu. This update was very big and even though we tried our best to allow for existing projects to continue using the Ultimate Radial Menu without any change, some of the previous functionality may have been removed which may cause issues with existing projects. Before you do anything, please take just a few moments to watch this video that we made to help you get the Ultimate Radial Menu working in your project after the new update:", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		if( GUILayout.Button( "Ultimate Radial Menu - 2.0.0 Important Changes", itemHeaderStyle ) )
		{
			Debug.Log( "Ultimate Radial Menu\nOpening 2.0.0 Important Changes Video" );
			Application.OpenURL( "https://youtu.be/B6V6wgS2qpU" );
		}
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Depreciated Functions and Events", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "A few of the functions that were previously a part of referencing the Ultimate Radial Menu have been depreciated. This means that they will still function as they did previously, but the new methods should be implemented as soon as possible. The new methods of referencing the Ultimate Radial Menu are more straight forward and easier to use (hopefully). Below is a list of the main functions that you may have been using, as well as a quick example of how to replace it with the new method.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "UpdateRadialButton()", itemHeaderStyle );
		EditorGUILayout.LabelField( "This function provided a way to update an existing radial button in the menu. Instead, you can now use the RegisterToRadialMenu function and leave out the index parameter. This method will simply assign the provided information to the first available button on the radial menu. Here is an example:", paragraphStyle );
		EditorGUILayout.LabelField( "RegisterToRadialMenu( YourCallbackFunction, YourButtonInfo );", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "AddRadialButton()", itemHeaderStyle );
		EditorGUILayout.LabelField( "This function provided a way to add a completely new radial button to the menu. Instead, you can now use the RegisterToRadialMenu function and pass in an index that is out of the range of the menu (like 1000). Here is an example:", paragraphStyle );
		EditorGUILayout.LabelField( "RegisterToRadialMenu( YourCallbackFunction, YourButtonInfo, 1000 );", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "InsertRadialButton()", itemHeaderStyle );
		EditorGUILayout.LabelField( "This function provided a way to insert a new radial button to a specific index of the menu. Instead, you can now use the RegisterToRadialMenu function and pass in an index that is at the index you want to add it at. Here is an example:", paragraphStyle );
		EditorGUILayout.LabelField( "RegisterToRadialMenu( YourCallbackFunction, YourButtonInfo, 2 );", paragraphStyle );
		
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Conclusion", sectionHeaderStyle );
		EditorGUILayout.LabelField( "For a full list of changes please click on the version number in the title at the top of this README. There are a few more depreciated functions that you may have been using. As always, if you run into any issues with this asset, please contact us at:", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Got it!", GUILayout.Width( Screen.width / 2 ) ) )
			NavigateBack();

		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void ThankYou ()
	{
		StartPage();

		EditorGUILayout.LabelField( "The two of us at Tank & Healer Studio would like to thank you for purchasing the Ultimate Radial Menu asset package from the Unity Asset Store.", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		
		EditorGUILayout.LabelField( "We hope that the Ultimate Radial Menu will be a great help to you in the development of your game. After clicking the <i>Continue</i> button below, you will be presented with information to assist you in getting to know the Ultimate Radial Menu and getting it implementing into your project.", paragraphStyle );

		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField( "You can access this information at any time by clicking on the <b>README</b> file inside the Ultimate Radial Menu folder.", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Again, thank you for downloading the Ultimate Radial Menu. We hope that your project is a success!", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", paragraphStyle );

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", GUILayout.Width( Screen.width / 2 ) ) )
			NavigateBack();

		var rect2 = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect2, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void Settings ()
	{
		StartPage();

		EditorGUILayout.LabelField( "Gizmo Colors", sectionHeaderStyle );
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( serializedObject.FindProperty( "colorDefault" ), new GUIContent( "Default" ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			EditorPrefs.SetString( "URM_ColorDefaultHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorDefault ) );
		}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( serializedObject.FindProperty( "colorValueChanged" ), new GUIContent( "Value Changed" ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			EditorPrefs.SetString( "URM_ColorValueChangedHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorValueChanged ) );
		}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( serializedObject.FindProperty( "colorButtonSelected" ), new GUIContent( "Button Selected" ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			EditorPrefs.SetString( "URM_ColorButtonSelectedHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorButtonSelected ) );
		}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( serializedObject.FindProperty( "colorButtonUnselected" ), new GUIContent( "Button Unselected" ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			EditorPrefs.SetString( "URM_ColorButtonUnselectedHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorButtonUnselected ) );
		}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( serializedObject.FindProperty( "colorTextBox" ), new GUIContent( "Text Box" ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			EditorPrefs.SetString( "URM_ColorTextBoxHex", "#" + ColorUtility.ToHtmlStringRGBA( readme.colorTextBox ) );
		}

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Reset", GUILayout.Width( EditorGUIUtility.currentViewWidth / 2 ) ) )
		{
			if( EditorUtility.DisplayDialog( "Reset Gizmo Color", "Are you sure that you want to reset the gizmo colors back to default?", "Yes", "No" ) )
			{
				ResetColors();
			}
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		if( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) )
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField( "Development Mode", sectionHeaderStyle );
			base.OnInspectorGUI();
			EditorGUILayout.Space();
		}

		GUILayout.FlexibleSpace();

		GUILayout.Space( sectionSpace );

		EditorGUI.BeginChangeCheck();
		GUILayout.Toggle( EditorPrefs.GetBool( "UUI_DevelopmentMode" ), ( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) ? "Disable" : "Enable" ) + " Development Mode", EditorStyles.radioButton );
		if( EditorGUI.EndChangeCheck() )
		{
			if( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) == false )
			{
				if( EditorUtility.DisplayDialog( "Enable Development Mode", "Are you sure you want to enable development mode for Tank & Healer Studio assets? This mode will allow you to see the default inspector for this asset which is useful when adding variables to this script without having to edit the custom editor script.", "Enable", "Cancel" ) )
				{
					EditorPrefs.SetBool( "UUI_DevelopmentMode", !EditorPrefs.GetBool( "UUI_DevelopmentMode" ) );
				}
			}
			else
				EditorPrefs.SetBool( "UUI_DevelopmentMode", !EditorPrefs.GetBool( "UUI_DevelopmentMode" ) );
		}

		EndPage();
	}

	void ResetColors ()
	{
		serializedObject.FindProperty( "colorDefault" ).colorValue = Color.black;
		serializedObject.FindProperty( "colorValueChanged" ).colorValue = Color.cyan;
		serializedObject.FindProperty( "colorButtonSelected" ).colorValue = Color.yellow;
		serializedObject.FindProperty( "colorButtonUnselected" ).colorValue = Color.white;
		serializedObject.FindProperty( "colorTextBox" ).colorValue = Color.yellow;
		serializedObject.ApplyModifiedProperties();

		EditorPrefs.SetString( "URM_ColorDefaultHex", "#" + ColorUtility.ToHtmlStringRGBA( Color.black ) );
		EditorPrefs.SetString( "URM_ColorValueChangedHex", "#" + ColorUtility.ToHtmlStringRGBA( Color.cyan ) );
		EditorPrefs.SetString( "URM_ColorButtonSelectedHex", "#" + ColorUtility.ToHtmlStringRGBA( Color.yellow ) );
		EditorPrefs.SetString( "URM_ColorButtonUnselectedHex", "#" + ColorUtility.ToHtmlStringRGBA( Color.white ) );
		EditorPrefs.SetString( "URM_ColorTextBoxHex", "#" + ColorUtility.ToHtmlStringRGBA( Color.yellow ) );
	}

	void ShowDocumentation ( DocumentationInfo info )
	{
		GUILayout.Space( paragraphSpace );

		if( GUILayout.Button( info.functionName, itemHeaderStyle ) )
		{
			info.showMore = !info.showMore;
			GUI.FocusControl( "" );
		}
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		if( info.showMore )
		{
			EditorGUILayout.LabelField( Indent + "<i>Description:</i> " + info.description, paragraphStyle );

			if( info.parameter != null )
			{
				for( int i = 0; i < info.parameter.Length; i++ )
					EditorGUILayout.LabelField( Indent + "<i>Parameter:</i> " + info.parameter[ i ], paragraphStyle );
			}
			if( info.returnType != string.Empty )
				EditorGUILayout.LabelField( Indent + "<i>Return type:</i> " + info.returnType, paragraphStyle );

			if( info.codeExample != string.Empty )
				EditorGUILayout.TextArea( info.codeExample, GUI.skin.textArea );

			GUILayout.Space( paragraphSpace );
		}
	}

	public static void OpenReadmeDocumentation ()
	{
		SelectReadmeFile();

		if( !pageHistory.Contains( AllPages[ 3 ] ) )
			NavigateForward( 3 );

		if( !pageHistory.Contains( AllPages[ 4 ] ) )
			NavigateForward( 4 );
	}

	[MenuItem( "Window/Tank and Healer Studio/Ultimate Radial Menu", false, 5 )]
	public static void SelectReadmeFile ()
	{
		var ids = AssetDatabase.FindAssets( "README t:UltimateRadialMenuReadme" );
		if( ids.Length == 1 )
		{
			var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
			Selection.objects = new Object[] { readmeObject };
			readme = ( UltimateRadialMenuReadme )readmeObject;
		}
		else
			Debug.LogError( "There is no README object in the Ultimate Radial Menu folder." );
	}
}