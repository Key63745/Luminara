/* UltimateRadialMenuInputManagerEditor.cs */
/* Written by Kaz Crowe */
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

[CustomEditor( typeof( UltimateRadialMenuInputManager ) )]
public class UltimateRadialMenuInputManagerEditor : Editor
{
	UltimateRadialMenuInputManager targ;
	bool multipleInputManagerError = false;
	bool worldSpaceMenus = false;
	bool uniqueInputManager = false;

	// EDITOR STYLES //
	GUIStyle collapsableSectionStyle = new GUIStyle();

	// DEVELOPMENT MODE //
	bool showDefaultInspector = false;

	private void OnEnable ()
	{
		bool instanceInputManager = false;
		UltimateRadialMenuInputManager[] allInputManagers = FindObjectsOfType<UltimateRadialMenuInputManager>();
		for( int i = 0; i < allInputManagers.Length; i++ )
		{
			// If this input manager is on a radial menu, then continue to the next index.
			if( allInputManagers[ i ].GetComponent<UltimateRadialMenu>() )
				continue;

			// If we have already found an official input manager...
			if( instanceInputManager )
			{
				// Then set the error bool and break the loop.
				multipleInputManagerError = true;
				break;
			}

			// Since this input manager is not on a radial menu component, then this would be our official one.
			instanceInputManager = true;
		}

		targ = ( UltimateRadialMenuInputManager )target;

		worldSpaceMenus = false;
		UltimateRadialMenu[] allMenus = FindObjectsOfType<UltimateRadialMenu>();
		for( int i = 0; i < allMenus.Length; i++ )
		{
			if( allMenus[ i ].IsWorldSpaceRadialMenu )
			{
				worldSpaceMenus = true;
				break;
			}
		}

		uniqueInputManager = targ.GetComponent<UltimateRadialMenu>();

		if( uniqueInputManager )
		{
			serializedObject.FindProperty( "keyboardInput" ).boolValue = false;
			serializedObject.FindProperty( "controllerInput" ).boolValue = true;
			serializedObject.FindProperty( "touchInput" ).boolValue = false;
			serializedObject.FindProperty( "virtualRealityInput" ).boolValue = false;
			serializedObject.FindProperty( "customInput" ).boolValue = false;
			serializedObject.ApplyModifiedProperties();
		}
	}

	bool DisplayCollapsibleBoxSection ( string sectionTitle, string editorPref, SerializedProperty enabledProp )
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

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		if( !targ.gameObject.GetComponent<EventSystem>() && !targ.gameObject.GetComponent<UltimateRadialMenu>() )
		{
			EditorGUILayout.BeginVertical( "Box" );
			GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
			warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
			warningStyle.alignment = TextAnchor.MiddleCenter;
			EditorGUILayout.LabelField( "WARNING", warningStyle );

			GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
			EditorGUILayout.LabelField( "The Ultimate Radial Menu Input Manager needs to be located on either the EventSystem in your scene, or on each individual Ultimate Radial Menu object if you want the input to be unique.", labelStyle );
			
			EditorGUILayout.EndVertical();
		}
		else if( multipleInputManagerError )
		{
			EditorGUILayout.BeginVertical( "Box" );
			GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
			warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
			warningStyle.alignment = TextAnchor.MiddleCenter;
			EditorGUILayout.LabelField( "WARNING", warningStyle );

			GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
			EditorGUILayout.LabelField( "There are multiple Ultimate Radial Menu Input Managers in the scene. This is likely because of a earlier version of the Ultimate Radial Menu. Click the button below to fix this.", labelStyle );

			if( GUILayout.Button( "Fix Input Manager" ) )
			{
				UltimateRadialMenuInputManager[] allInputManagers = FindObjectsOfType<UltimateRadialMenuInputManager>();
				for( int i = 0; i < allInputManagers.Length; i++ )
				{
					if( !allInputManagers[ i ].GetComponent<EventSystem>() && !allInputManagers[ i ].GetComponent<UltimateRadialMenu>() )
						DestroyImmediate( allInputManagers[ i ] );
				}

				if( !FindObjectOfType<EventSystem>().gameObject.GetComponent<UltimateRadialMenuInputManager>() )
					FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();

				multipleInputManagerError = FindObjectsOfType<UltimateRadialMenuInputManager>().Length > 1;
			}

			EditorGUILayout.EndVertical();
		}
		else
		{
			collapsableSectionStyle = new GUIStyle( EditorStyles.label ) { alignment = TextAnchor.MiddleCenter, onActive = new GUIStyleState() { textColor = Color.black } };
			collapsableSectionStyle.active.textColor = collapsableSectionStyle.normal.textColor;

			// INTERACT SETTINGS //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "invokeAction" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "onMenuRelease" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "disableOnInteract" ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// END INTERACT SETTINGS //

			EditorGUILayout.Space();

			if( uniqueInputManager )
			{
				EditorGUILayout.BeginVertical( "Box" );
				collapsableSectionStyle.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField( "Unique Controller Input", collapsableSectionStyle );
				collapsableSectionStyle.fontStyle = FontStyle.Normal;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "horizontalAxisController" ), new GUIContent( "Horizontal Axis" ) );
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "verticalAxisController" ), new GUIContent( "Vertical Axis" ) );
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "interactButtonController" ), new GUIContent( "Interact Button" ) );
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableWithController" ) );
				if( targ.enableWithController )
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableButtonController" ), new GUIContent( "Enable Button" ) );
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "invertHorizontal" ) );
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "invertVertical" ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				EditorGUILayout.EndVertical();
			}
			else
			{
				// MOUSE AND KEYBOARD SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Mouse & Keyboard Input", "URMIM_KeyboardInput", serializedObject.FindProperty( "keyboardInput" ) ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "mouseButtonIndex" ) );
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableWithKeyboard" ) );
					if( targ.enableWithKeyboard )
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableButtonKeyboard" ), new GUIContent( "Enable Button" ) );
						EditorGUI.indentLevel--;
					}
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();
				// END MOUSE AND KEYBOARD SETTINGS //

				// CONTROLLER SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Controller Input", "URMIM_ControllerInput", serializedObject.FindProperty( "controllerInput" ) ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "horizontalAxisController" ), new GUIContent( "Horizontal Axis" ) );
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "verticalAxisController" ), new GUIContent( "Vertical Axis" ) );
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "interactButtonController" ), new GUIContent( "Interact Button" ) );
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableWithController" ) );
					if( targ.enableWithController )
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableButtonController" ), new GUIContent( "Enable Button" ) );
						EditorGUI.indentLevel--;
					}
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "invertHorizontal" ) );
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "invertVertical" ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();
				// END CONTROLLER SETTINGS //

				// TOUCH SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Touch Input", "URMIM_TouchInput", serializedObject.FindProperty( "touchInput" ) ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableWithTouch" ) );
					if( targ.enableWithTouch )
					{
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "dynamicPositioning" ) );
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "activationRadius" ) );
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "activationHoldTime" ) );

						if( worldSpaceMenus && targ.touchInput && targ.dynamicPositioning )
							EditorGUILayout.HelpBox( "The Touch Input Dynamic Positioning will not work with world space radial menus.", MessageType.Warning );
					}
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();
				// END TOUCH SETTINGS //

				// VIRTUAL REALITY SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Virtual Reality Input", "URMIM_VirtualRealityInput", serializedObject.FindProperty( "virtualRealityInput" ) ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "interactButtonVirtualReality" ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();
				// END VIRTUAL REALITY SETTINGS //

				// CUSTOM SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Custom Input", "URMIM_CustomInput", serializedObject.FindProperty( "customInput" ) ) )
				{
					GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
					warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
					warningStyle.alignment = TextAnchor.MiddleCenter;
					EditorGUILayout.LabelField( "WARNING", warningStyle );

					GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
					EditorGUILayout.LabelField( "Your custom input logic should be placed inside of a different script that inherits from the UltimateRadialMenuInputManager class.", labelStyle );

					GUIStyle style = new GUIStyle( GUI.skin.label ) { richText = true, wordWrap = true };
					EditorGUILayout.LabelField( "Please check out our <b><color=blue>Video Tutorials</color></b> to learn more!", style );
					var rect = GUILayoutUtility.GetLastRect();
					EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
					if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
						Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu" );
				}
				EditorGUILayout.EndVertical();
				// END CUSTOM SETTINGS //
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

					if( targ.customInput )
					{
						EditorGUILayout.BeginVertical( "Box" );
						GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
						warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
						warningStyle.alignment = TextAnchor.MiddleCenter;
						EditorGUILayout.LabelField( "WARNING", warningStyle );

						GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
						EditorGUILayout.LabelField( "Your custom input logic should be placed inside of a different script that inherits from the UltimateRadialMenuInputManager class.", labelStyle );

						GUIStyle style = new GUIStyle( GUI.skin.label ) { richText = true, wordWrap = true };
						EditorGUILayout.LabelField( "Please check out our <b><color=blue>Video Tutorials</color></b> to learn more!", style );
						var rect = GUILayoutUtility.GetLastRect();
						EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
						if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
							Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu" );
						EditorGUILayout.EndVertical();
					}
				}
			}

			EditorGUILayout.Space();
		}

		Repaint();
	}

	private void OnSceneGUI ()
	{
		if( targ.touchInput && targ.enableWithTouch && EditorPrefs.GetBool( "URMIM_TouchInput" ) )
		{
			UltimateRadialMenu[] allRadialMenus = FindObjectsOfType<UltimateRadialMenu>();
			for( int i = 0; i < allRadialMenus.Length; i++ )
			{
				RectTransform trans = allRadialMenus[ i ].transform.GetComponent<RectTransform>();
				Vector3 center = allRadialMenus[ i ].BasePosition;
				center.z = trans.position.z;
				Handles.DrawWireDisc( center, trans.transform.forward, ( ( trans.sizeDelta.x / 2 ) * trans.GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x ) * targ.activationRadius );
			}
		}

		SceneView.RepaintAll();
	}
}