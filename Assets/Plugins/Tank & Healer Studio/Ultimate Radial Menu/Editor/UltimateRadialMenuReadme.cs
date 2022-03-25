/* UltimateRadialMenuReadme.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using System.Collections.Generic;

//[CreateAssetMenu( fileName = "README", menuName = "Tank and Healer Studio/Ultimate Radial Menu README File", order = 1 )]
public class UltimateRadialMenuReadme : ScriptableObject
{
	public Texture2D icon;
	public Texture2D scriptReference;
	public Texture2D settings;

	// GIZMO COLORS //
	[HideInInspector]
	public Color colorDefault = Color.black;
	[HideInInspector]
	public Color colorValueChanged = Color.cyan;
	[HideInInspector]
	public Color colorButtonSelected = Color.yellow;
	[HideInInspector]
	public Color colorButtonUnselected = Color.white;
	[HideInInspector]
	public Color colorTextBox = Color.yellow;

	public static int ImportantChange = 2;
	public class VersionHistory
	{
		public string versionNumber = "";
		public string[] changes;
	}
	public VersionHistory[] versionHistory = new VersionHistory[]
	{
		// VERSION 2.2.2
		new VersionHistory()
		{
			versionNumber = "2.2.2",
			changes = new string[]
			{
				// GENERAL //
				"Added some code that will unpack the radial menu prefab when adding to a scene. Unity's prefab system was forcing unwanted behavior on the radial menus and reverting button sprites",
				// REMOVED //
				"Removed all the depreciated functions from the UltimateRadialMenu.cs script that were depreciated in version 2.0.0",
				// BUG FIXES //
				"Fixed a rare issue where the radial button would revert back to a null sprite if selecting or deselecting a button",
				"Fixed a very rare issue with the Callback system invoking multiple callbacks when assigning a string with the invoked function",
			}
		},
		// VERSION 2.2.1
		new VersionHistory()
		{
			versionNumber = "2.2.1",
			changes = new string[]
			{
				// BUG FIXES //
				"Fixed the Ultimate Radial Menu Input Manager code to correctly use the unique Interact Settings from the unique input manager if it is being used",
			}
		},
		// VERSION 2.2.0
		new VersionHistory()
		{
			versionNumber = "2.2.0",
			changes = new string[]
			{
				// GENERAL //
				"Improved the input manager to support multiple controller joysticks for individual radial menus if needed",
				"Updated functionality for the radial menu when being used in Screen Space - Camera",
				"Improved the Ultimate Radial Menu Input Manager for the World Space and Screen Space - Camera Canvas options",
				"Updated the radial menu to retain the current selected button even when adding and removing buttons at runtime",
				"Updated the Gun Wheel Example scene to correctly display the currently selected weapon when changing from menu to menu",
				"Removed the Simple World Space Example files and replaced it with a new example named: Look At Input Example which will hopefully be more useful",
				"Moved the Ultimate Radial Menu folder into a root folder named: Tank & Healer Studio",
			}
		},
		// VERSION 2.1.6
		new VersionHistory()
		{
			versionNumber = "2.1.6",
			changes = new string[]
			{
				// GENERAL //
				"Added a check for the radial menu not being in world space before calculating the input in relative position of the menu",
				// BUG FIXES //
				"Fixed an issue with the radial menu not processing input correctly when using a controller for input",
			}
		},
		// VERSION 2.1.5
		new VersionHistory()
		{
			versionNumber = "2.1.5",
			changes = new string[]
			{
				// GENERAL //
				"Improved performance when processing input to a radial menu",
				"Small improvements to the README file",
				// BUG FIXES //
				"Fixed a rare issue that could occur when using one radial menu for sub menus too",
			}
		},
		// VERSION 2.1.4
		new VersionHistory()
		{
			versionNumber = "2.1.4",
			changes = new string[]
			{
				// GENERAL //
				"Small overall improvements to the positioning of the radial menu on the canvas with different canvas options",
				"Updated the SetPosition function to optionally allow for local space as well",
				"Improved the Input Manager script internally to handle input better for different canvas options",
				"Added a new option to the Touch Input section of the Input Manager script. This new option allows the user a more clear way to control the enabled/disabled state when using the Touch Input option",
				// BUG FIXES //
				"Fixed the OnRadialButtonSelected callback from being called multiple times without any new selected buttons",
				"Fixed a rare issue where the Input Manager would throw an error if a menu was deleted at runtime",
			}
		},
		// VERSION 2.1.3
		new VersionHistory()
		{
			versionNumber = "2.1.3",
			changes = new string[]
			{
				// BUG FIXES //
				"Fixed a small error that could occur when loading the README file",
			}
		},
		// VERSION 2.1.2
		new VersionHistory()
		{
			versionNumber = "2.1.2",
			changes = new string[]
			{
				// GENERAL //
				"Updated the input manager script to have a specific option for disabling the ability to toggle the radial menu state from the input manager. This is useful for if the user wants to enable/disable the radial menu through their custom code",
				"Updated the input manager editor script to be easier to work with and more visually appealing. Additionally added the Development Inspector option to the input manager editor",
				"Corrected a reference to a depreciated function in the example code for the README file",
				"Improved the README file to stay on the same page even after Unity compiles scripts",
				// BUG FIXES //
				"Fixed a small error that would occur if the user had a class named Outline which would cause conflicts with Unity's default Outline class",
				"Fixed a small error that could occur if the user added the Ultimate Radial Menu script to a non UI object",
			}
		},
		// VERSION 2.1.1
		new VersionHistory()
		{
			versionNumber = "2.1.1",
			changes = new string[]
			{
				// GENERAL //
				"Added a new prefab for the Dark style to have the center image",
				// ADDED METHODS //
				"Added a function to the Input Manager to update the current camera used for calculations for World Space radial menus. This is useful for when a camera is not tagged as MainCamera or if the user is changing cameras at runtime",
				"Added a new public function to the Input Manager to specifically set the current camera used for calculations for World Space radial menus",
			}
		},
		// VERSION 2.1.0
		new VersionHistory()
		{
			versionNumber = "2.1.0",
			changes = new string[]
			{
				// BUG FIXES //
				"Fixed a small issue when updating the icon from code when registering the button information without an icon assigned",
				"Fixed a issue when using a radial menu without a button sprite assigned",
				"Various performance improvements",
				"Various small bug fixes",
				// GENERAL //
				"Changed the function of the Angle Offset setting to work better with dynamically adding and removing buttons at runtime",
				"Added 2 new scripts to handle a complete visual style for each different button count of the radial menu. These scripts are the: UltimateRadialMenuStyle and UltimateRadialMenuStyleEditor",
				"Revamped existing radial menu images to work with the new style system",
				"Improved the Ultimate Radial Menu Pointer script to handle different styles",
				"Simplified the Ultimate Radial Menu Pointer inspector to be easier to work with",
				"Added object pooling to the radial menu to improve performance when clearing and populating the menu many times during runtime",
				"Expanded the functionality of the RemoveAllRadialButtons function to allow for keeping a certain number of buttons if needed. This can improve performance if clearing the menu just to register new information right after",
				"Changed the position modifier option for the different states to allow for negative values",
				"Removed the enum option for text positioning since there was only two options. Replaced it with a boolean for Local Position",
			}
		},
		// VERSION 2.0.0 // IMPORTANT CHANGE 2
		new VersionHistory ()
		{
			versionNumber = "2.0.0",
			changes = new string[]
			{
				// GENERAL //
				"Added positioning support for world space canvas",
				"Improved positioning calculations to support other canvas options",
				"Updated input manager logic to support world space canvas",
				"Added new option to Input Manager: Virtual Reality Input. This uses the center of the screen to interact with radial menu",
				"Updated the editor sections to help be more clear. Some options may have moved to other sections",
				"Updated editor script visually and added collapsible sections",
				"Added functionality and options for displaying a currently selected button",
				"Heavily modified the button interaction to allow for more customization",
				"Created a new C# script to hold the UltimateRadialButtonInfo class for easier modification",
				"Included in-engine documentation to the README file and removed links to outdated online documentation",
				"Added a new simple example scene to show the world space radial menu in action",
				"Included new options for inverting the axis of the controller input",
				// REMOVED //
				"Removed the Text Positioning Option: Relative to Icon",
				"Removed the options for disabling the icon and text when the button is disabled. This functionality still exists when using the Color Change option for the icon and text",
				"Removed the callback: OnRadialMenuButtonFound. A new callback with improved functionality is OnRadialButtonEnter",
				// ADDED METHODS //
				"Added a new function: RegisterToRadialMenu. This function now handles all the functionality for adding any sort of information to the radial menu",
				"Added a new function: ClearRadialButtonInformations. This function clears all the registered button information",
				"Added a new function: RemoveAllRadialButtons. This function deletes all of the radial buttons in the menu",
				"Added a new function: CreateEmptyRadialButton. This function creates a new button with no information attached to it",
				// DEPRECIATED //
				"Depreciated the UpdateSizeAndPlacement function. This name wasn't easy to find or understand. The new function name is: UpdatePositioning",
				"Depreciated the GetUltimateRadialButton function. The radial button can be referenced by it's index",
				"Depreciated the UpdateRadialButton functions. The new RegisterToRadialMenu function now handles the same functionality",
				"Depreciated the AddRadialButton functions. The new RegisterToRadialMenu function now handles the same functionality",
				"Depreciated the InsertRadialButton functions. The new RegisterToRadialMenu function now handles the same functionality",
				"Depreciated the ClearRadialButtons functions. Please use RemoveAllRadialButtons instead",
				"Depreciated the OnRadialMenuButtonFound callback action. The OnRadialButtonEnter callback action should be instead",
				"Depreciated the OnUpdateSizeAndPlacement callback action. The OnUpdatePositioning callback action should be instead",
				"Depreciated the EnableInteraction function. If you want to enable interaction on the radial menu simply use the Interactable variable",
				"Depreciated the DisableInteraction function. If you want to disable interaction on the radial menu simply use the Interactable variable",
				"Depreciated the ResetRadialButtonInformation function. If you want to clear the information on the radial button use the ClearButtonInformation function",
			},
		},
		// VERSION 1.1.0
		new VersionHistory ()
		{
			versionNumber = "1.1.0",
			changes = new string[]
			{
				"Simplified the editor script internally",
				"Removed AnimBool functionality from the inspector to avoid errors with Unity 2019+",
				"Improved internal control of input interaction",
				"Added new option for the initial state of the radial menu to allow it to be enabled at the start of the scene",
				"Simplified internal calculations when assigning a new radial button",
				"Renamed example scene files to better identify their purpose",
				"Added new example scene for placing the radial menu over a world object's position",
				"Added new public function: ResetPosition(). This function will reset the position of the Ultimate Radial Menu to the default position that was calculated",
				"Added new public function in the UltimateRadialButton class: AddCallback(). This function will subscribe the provided function to the radial button interaction",
				"Added new public function: DisableInteraction(). This function will disable interaction on the radial menu",
				"Added new public function: EnableInteraction(). This function will enable interaction on the radial menu",
				"Added new public static function: SetPosition(). This function calls the public SetPosition() function on the targeted Ultimate Radial Menu",
				"Added new public and public static functions: GetUltimateRadialButton(). This function will return the targeted Ultimate Radial Button",
				"Added a new option to the Ultimate Radial Menu Input Manager: Touch Input. This option enables touch input on the radial menu so that it can be used in mobile projects",
				"Added new script: UltimateRadialMenuReadme.cs",
				"Added new script: UltimateRadialMenuReadmeEditor.cs",
				"Added new file at the Ultimate Radial Menu root folder: README. This file has all the documentation and how to information",
				"Removed the UltimateRadialMenuWindow.cs file. This script can safely be removed from your project. All of that information and more is now located in the README file",
				"Removed the old README text file. All of that information is now located in the README file",
				"Added options for changing the scene gizmo colors for the Ultimate Radial Menu. These options are located in the new README file in Settings",
			},
		},
		// VERSION 1.0.5
		new VersionHistory ()
		{
			versionNumber = "1.0.5",
			changes = new string[]
			{
				"Renamed prefabs to help know what the object is in the hierarchy",
			},
		},
		// VERSION 1.0.4
		new VersionHistory ()
		{
			versionNumber = "1.0.4",
			changes = new string[]
			{
				"Updated the Ultimate Radial Menu editor to display a few more functions in the generated example code",
				"Modified the Input Manager script to have virtual functions, allowing custom input scripts to be implemented",
				"Added new public function: <b>SetPosition</b> to allow for easily changing the position of the radial menu on the screen",
			},
		},
		// VERSION 1.0.3
		new VersionHistory ()
		{
			versionNumber = "1.0.3",
			changes = new string[]
			{
				"Fixed the Ultimate Radial Menu Editor to display the correct script reference code",
				"Overall bug fixes to the scripts",
			},
		},
		// VERSION 1.0.2
		new VersionHistory ()
		{
			versionNumber = "1.0.2",
			changes = new string[]
			{
				"Modified Input Manager script to handle all of the input for the radial menu. It is also now placed on the EventSystem in the scene",
				"Added a button to the Ultimate Radial Menu inspector to select the input manager",
			},
		},
		// VERSION 1.0.1
		new VersionHistory ()
		{
			versionNumber = "1.0.1",
			changes = new string[]
			{
				"Modified Input Manager script to allow for no key to be used for enabling and disabling the menu",
			},
		},
		// VERSION 1.0
		new VersionHistory ()
		{
			versionNumber = "1.0",
			changes = new string[]
			{
				"Initial Release",
			},
		},
	};

	[HideInInspector]
	public List<int> pageHistory = new List<int>();
	[HideInInspector]
	public Vector2 scrollValue = new Vector2();
}