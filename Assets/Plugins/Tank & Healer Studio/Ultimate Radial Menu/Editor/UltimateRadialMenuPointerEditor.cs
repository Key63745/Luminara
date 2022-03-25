/* UltimateRadialMenuPointerEditor.cs */
/* Written by Kaz Crowe */
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor( typeof( UltimateRadialMenuPointer ) )]
public class UltimateRadialMenuPointerEditor : Editor
{
	UltimateRadialMenuPointer targ;

	// Properties //
	SerializedProperty pointerSize, rotationOffset;
	SerializedProperty targetingSpeed, snappingOption;
	SerializedProperty setSiblingIndex;
	SerializedProperty colorChange, changeOverTime;
	SerializedProperty fadeInDuration, fadeOutDuration;
	SerializedProperty normalColor, activeColor;

	SerializedProperty usePointerStyle;

	// EDITOR STYLES //
	GUIStyle collapsableSectionStyle = new GUIStyle();

	bool showDefaultInspector = false;

	int newStyleButtonCount = 2;
	bool duplicateButtonCount = false;


	void OnEnable ()
	{
		StoreReferences();
		Undo.undoRedoPerformed += UndoRedoCallback;
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
		targ = ( UltimateRadialMenuPointer )target;
		
		pointerSize = serializedObject.FindProperty( "pointerSize" );
		targetingSpeed = serializedObject.FindProperty( "targetingSpeed" );
		snappingOption = serializedObject.FindProperty( "snappingOption" );
		rotationOffset = serializedObject.FindProperty( "rotationOffset" );
		setSiblingIndex = serializedObject.FindProperty( "setSiblingIndex" );
		colorChange = serializedObject.FindProperty( "colorChange" );
		changeOverTime = serializedObject.FindProperty( "changeOverTime" );
		fadeInDuration = serializedObject.FindProperty( "fadeInDuration" );
		fadeOutDuration = serializedObject.FindProperty( "fadeOutDuration" );
		normalColor = serializedObject.FindProperty( "normalColor" );
		activeColor = serializedObject.FindProperty( "activeColor" );
		usePointerStyle = serializedObject.FindProperty( "usePointerStyle" );
		
		serializedObject.FindProperty( "radialMenu" ).objectReferenceValue = targ.GetComponentInParent<UltimateRadialMenu>();
		serializedObject.ApplyModifiedProperties();

		if( targ.pointerTransform == null )
		{
			serializedObject.FindProperty( "pointerTransform" ).objectReferenceValue = targ.GetComponent<RectTransform>();
			serializedObject.ApplyModifiedProperties();
		}

		if( targ.pointerImage == null && targ.GetComponent<Image>() )
		{
			serializedObject.FindProperty( "pointerImage" ).objectReferenceValue = targ.GetComponent<Image>();
			serializedObject.ApplyModifiedProperties();
		}

		if( targ.setSiblingIndex != UltimateRadialMenuPointer.SetSiblingIndex.Disabled )
		{
			if( targ.setSiblingIndex == UltimateRadialMenuPointer.SetSiblingIndex.First )
				targ.transform.SetAsFirstSibling();
			else
				targ.transform.SetAsLastSibling();
		}

		CheckForDuplicateButtonCount();
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
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		collapsableSectionStyle = new GUIStyle( EditorStyles.label ) { alignment = TextAnchor.MiddleCenter, onActive = new GUIStyleState() { textColor = Color.black } };
		collapsableSectionStyle.active.textColor = collapsableSectionStyle.normal.textColor;

		EditorGUILayout.Space();
		
		if( !targ.GetComponentInParent<UltimateRadialMenu>() )
		{
			EditorGUILayout.HelpBox( "Please place this Pointer game object inside an Ultimate Radial Menu.", MessageType.Warning );
			Repaint();
			return;
		}

		if( targ.GetComponent<UltimateRadialMenu>() )
		{
			EditorGUILayout.HelpBox( "This component cannot be placed on the Ultimate Radial Menu gameObject. Please create a UI Image as a child of the radial menu to use a pointer.", MessageType.Warning );
			Repaint();
			return;
		}

		if( !targ.GetComponent<RectTransform>() )
		{
			EditorGUILayout.HelpBox( "This object is not a UI Game Object and does not have a RectTransform component. Please ensure that you place this component on a UI Game Object.", MessageType.Warning );
			if( GUILayout.Button( "Attempt Fix" ) )
				targ.gameObject.AddComponent<RectTransform>();
			
			Repaint();
			return;
		}

		if( !targ.GetComponent<Image>() )
		{
			EditorGUILayout.HelpBox( "This object does not have a Image component to use for the pointer. Please make sure you place this component on an Image.", MessageType.Warning );
			if( GUILayout.Button( "Attempt Fix" ) )
			{
				targ.gameObject.AddComponent<CanvasRenderer>();
				targ.gameObject.AddComponent<Image>();
			}

			Repaint();
			return;
		}

		EditorGUILayout.LabelField( "Radial Menu: " + targ.radialMenu.gameObject.name );
		
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.Slider( pointerSize, 0.0f, 1.0f, new GUIContent( "Pointer Size", "The overall size of the pointer image." ) );
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( snappingOption, new GUIContent( "Snapping Option", "Determines how the pointer will snap to the current radial button." ) );
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();

		if( targ.snappingOption != UltimateRadialMenuPointer.SnappingOption.Instant )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider( targetingSpeed, 2.0f, 10.0f, new GUIContent( "Targeting Speed", "The speed at which the pointer will target the radial button." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( rotationOffset, new GUIContent( "Rotation Offset", "The offset to apply to the pointer transform." ) );
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( setSiblingIndex, new GUIContent( "Sibling Index", "The sibling index that this pointer should be at in the menu. Determines if it appearing above or below the radial buttons." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			if( targ.setSiblingIndex != UltimateRadialMenuPointer.SetSiblingIndex.Disabled )
			{
				if( targ.setSiblingIndex == UltimateRadialMenuPointer.SetSiblingIndex.First )
					targ.transform.SetAsFirstSibling();
				else
					targ.transform.SetAsLastSibling();
			}
		}

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( normalColor, new GUIContent( "Normal Color", "The normal color of the pointer image." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			if( targ.pointerImage != null )
			{
				Undo.RecordObject( targ.pointerImage, "Change Pointer Image Color" );
				targ.pointerImage.color = targ.normalColor;
			}
		}

		bool valueChanged = false;
		EditorGUILayout.BeginVertical( "Box" );
		if( DisplayCollapsibleBoxSection( "Color Change", "URMP_ColorChange", colorChange, ref valueChanged ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( activeColor, new GUIContent( "Active Color", "The active color of the pointer image." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( changeOverTime, new GUIContent( "Change Over Time", "Determines if the pointer image should change color over time or not." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginDisabledGroup( !targ.changeOverTime );
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( fadeInDuration, new GUIContent( "Fade In Duration", "The time is seconds for the pointer image to fade in." ) );
			EditorGUILayout.PropertyField( fadeOutDuration, new GUIContent( "Fade Out Duration", "The time in seconds for the pointer image to fade out." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Space();
		}
		EditorGUILayout.EndVertical();
		if( valueChanged )
		{
			if( targ.pointerImage == null )
				return;
			
			if( targ.colorChange )
			{
				normalColor.colorValue = targ.pointerImage.color;
				serializedObject.ApplyModifiedProperties();
			}
		}

		EditorGUILayout.BeginVertical( "Box" );
		if( DisplayCollapsibleBoxSection( "Pointer Style", "URMP_PointerStyle", usePointerStyle, ref valueChanged ) )
		{
			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();
			newStyleButtonCount = EditorGUILayout.IntField( newStyleButtonCount, GUILayout.Width( 50 ) );
			if( EditorGUI.EndChangeCheck() )
				CheckForDuplicateButtonCount();

			EditorGUI.BeginDisabledGroup( duplicateButtonCount );
			if( GUILayout.Button( "Create New Style", EditorStyles.miniButton ) )
			{
				GUI.FocusControl( "" );

				serializedObject.FindProperty( "PointerStyles" ).arraySize++;
				serializedObject.ApplyModifiedProperties();

				serializedObject.FindProperty( string.Format( "PointerStyles.Array.data[{0}].buttonCount", targ.PointerStyles.Count - 1 ) ).intValue = newStyleButtonCount;
				serializedObject.FindProperty( string.Format( "PointerStyles.Array.data[{0}].pointerSprite", targ.PointerStyles.Count - 1 ) ).objectReferenceValue = null;
				serializedObject.ApplyModifiedProperties();

				SortStylesList();
				newStyleButtonCount++;
				CheckForDuplicateButtonCount();
			}
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndHorizontal();

			for( int i = 0; i < targ.PointerStyles.Count; i++ )
			{
				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( targ.PointerStyles[ i ].buttonCount.ToString() + " Button Style ───────────────────────────────────────────" );
				if( GUILayout.Button( "×", collapsableSectionStyle, GUILayout.Width( 17 ) ) )
				{
					if( EditorUtility.DisplayDialog( "Ultimate Radial Menu Pointer - Warning", "You are about to delete the style for this button count. Are you sure you want to do this?", "Continue", "Cancel" ) )
					{
						serializedObject.FindProperty( "PointerStyles" ).DeleteArrayElementAtIndex( i );
						serializedObject.ApplyModifiedProperties();
						break;
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUI.indentLevel++;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( serializedObject.FindProperty( string.Format( "PointerStyles.Array.data[{0}].pointerSprite", i ) ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				EditorGUI.indentLevel--;
			}
		}
		GUILayout.Space( 1 );
		EditorGUILayout.EndVertical();
		if( valueChanged )
		{
			
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

		Repaint();
	}

	void CheckForDuplicateButtonCount ()
	{
		duplicateButtonCount = false;
		for( int i = 0; i < targ.PointerStyles.Count; i++ )
		{
			if( newStyleButtonCount == targ.PointerStyles[ i ].buttonCount )
			{
				duplicateButtonCount = true;
				break;
			}
		}
	}

	void SortStylesList ()
	{
		targ.PointerStyles = targ.PointerStyles.OrderBy( w => w.buttonCount ).ToList();
	}
}