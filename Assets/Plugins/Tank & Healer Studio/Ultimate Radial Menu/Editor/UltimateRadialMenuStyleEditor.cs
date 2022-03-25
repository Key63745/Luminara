/* UltimateRadialMenuStyleEditor.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor( typeof( UltimateRadialMenuStyle ) )]
public class UltimateRadialMenuStyleEditor : Editor
{
	UltimateRadialMenuStyle targ;
	GUIStyle collapsableSectionStyle;
	SerializedProperty minButtonCount, maxButtonCount;
	List<SerializedProperty> buttonCount, normalSprite, highlightedSprite, pressedSprite, selectedSprite, disabledSprite;
	static List<int> ShowStyleIndexes = new List<int>();


	private void OnEnable ()
	{
		targ = ( UltimateRadialMenuStyle )target;

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
		minButtonCount = serializedObject.FindProperty( "minButtonCount" );
		maxButtonCount = serializedObject.FindProperty( "maxButtonCount" );

		buttonCount = new List<SerializedProperty>();
		normalSprite = new List<SerializedProperty>();
		highlightedSprite = new List<SerializedProperty>();
		pressedSprite = new List<SerializedProperty>();
		selectedSprite = new List<SerializedProperty>();
		disabledSprite = new List<SerializedProperty>();

		for( int i = 0; i < targ.RadialMenuStyles.Count; i++ )
		{
			buttonCount.Add( serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].buttonCount", i ) ) );
			normalSprite.Add( serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].normalSprite", i ) ) );
			highlightedSprite.Add( serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].highlightedSprite", i ) ) );
			pressedSprite.Add( serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].pressedSprite", i ) ) );
			selectedSprite.Add( serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].selectedSprite", i ) ) );
			disabledSprite.Add( serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].disabledSprite", i ) ) );
		}
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		collapsableSectionStyle = new GUIStyle( EditorStyles.label ) { alignment = TextAnchor.MiddleLeft, onActive = new GUIStyleState() { textColor = Color.black }, richText = true };
		collapsableSectionStyle.active.textColor = collapsableSectionStyle.normal.textColor;
		
		if( targ.RadialMenuStyles.Count == 0 )
		{
			EditorGUILayout.BeginVertical( "Box" );

			EditorGUILayout.LabelField( "Style Generator", EditorStyles.boldLabel );

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( minButtonCount, new GUIContent( "Min Button Count", "The minimum number of buttons allowed on the radial menu." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( minButtonCount.intValue <= 2 )
					minButtonCount.intValue = 2;

				if( minButtonCount.intValue >= maxButtonCount.intValue )
					minButtonCount.intValue = maxButtonCount.intValue - 1;

				serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( maxButtonCount, new GUIContent( "Max Button Count", "The maximum number of buttons allowed for this style." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( maxButtonCount.intValue <= minButtonCount.intValue )
					maxButtonCount.intValue = minButtonCount.intValue + 1;

				serializedObject.ApplyModifiedProperties();
			}

			if( GUILayout.Button( "Generate Style", EditorStyles.miniButton ) )
			{
				int arrayIndex = 0;
				for( int i = targ.minButtonCount; i <= targ.maxButtonCount; i++ )
				{
					serializedObject.FindProperty( "RadialMenuStyles" ).arraySize++;
					serializedObject.ApplyModifiedProperties();

					serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].buttonCount", arrayIndex ) ).intValue = i;
					serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].normalSprite", arrayIndex ) ).objectReferenceValue = null;
					serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].highlightedSprite", arrayIndex ) ).objectReferenceValue = null;
					serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].pressedSprite", arrayIndex ) ).objectReferenceValue = null;
					serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].selectedSprite", arrayIndex ) ).objectReferenceValue = null;
					serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].disabledSprite", arrayIndex ) ).objectReferenceValue = null;
					serializedObject.ApplyModifiedProperties();
					
					arrayIndex++;
				}
				StoreReferences();
			}

			EditorGUILayout.EndVertical();
		}
		else
		{
			EditorGUILayout.LabelField( "Button Count Styles", EditorStyles.boldLabel );

			EditorGUI.BeginDisabledGroup( targ.minButtonCount <= 2 );
			if( GUILayout.Button( targ.minButtonCount <= 2 ? "Cannot Add Button Style" : "Add " + ( targ.minButtonCount - 1 ).ToString() + " Button Style", EditorStyles.miniButton ) )
			{
				serializedObject.FindProperty( "RadialMenuStyles" ).InsertArrayElementAtIndex( 0 );
				serializedObject.ApplyModifiedProperties();

				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].buttonCount", 0 ) ).intValue = targ.minButtonCount - 1;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].normalSprite", 0 ) ).objectReferenceValue = null;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].highlightedSprite", 0 ) ).objectReferenceValue = null;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].pressedSprite", 0 ) ).objectReferenceValue = null;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].selectedSprite", 0 ) ).objectReferenceValue = null;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].disabledSprite", 0 ) ).objectReferenceValue = null;
				minButtonCount.intValue--;
				serializedObject.ApplyModifiedProperties();

				//ShowMore.Insert( 0, false );

				//SortStylesList();
			}
			EditorGUI.EndDisabledGroup();

			for( int i = 0; i < targ.RadialMenuStyles.Count; i++ )
			{
				string normalSpriteState = targ.RadialMenuStyles[ i ].normalSprite != null ? "<b>•</b>" : "○";
				string highlightedSpriteState = targ.RadialMenuStyles[ i ].highlightedSprite != null ? "<b>•</b>" : "○";
				string pressedSpriteState = targ.RadialMenuStyles[ i ].pressedSprite != null ? "<b>•</b>" : "○";
				string selectedSpriteState = targ.RadialMenuStyles[ i ].selectedSprite != null ? "<b>•</b>" : "○";
				string disabledSpriteState = targ.RadialMenuStyles[ i ].disabledSprite != null ? "<b>•</b>" : "○";
				string assignedSpritesIndication = normalSpriteState + highlightedSpriteState + pressedSpriteState + selectedSpriteState + disabledSpriteState;

				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( ( ShowStyleIndexes.Contains( i ) ? "▼" : "►" ) + "Button Count " + targ.RadialMenuStyles[ i ].buttonCount.ToString() + "	" + assignedSpritesIndication, collapsableSectionStyle ) )
				{
					if( !ShowStyleIndexes.Contains( i ) )
						ShowStyleIndexes.Add( i );
					else
						ShowStyleIndexes.Remove( i );
				}
				if( ( targ.RadialMenuStyles[ i ].buttonCount == targ.minButtonCount || targ.RadialMenuStyles[ i ].buttonCount == targ.maxButtonCount ) && targ.RadialMenuStyles.Count > 1 )
				{
					if( GUILayout.Button( "×", collapsableSectionStyle, GUILayout.Width( 17 ) ) )
					{
						if( EditorUtility.DisplayDialog( "Ultimate Radial Menu Style - Warning", "You are about to delete the style for this button count. Are you sure you want to do this?", "Continue", "Cancel" ) )
						{
							serializedObject.FindProperty( "RadialMenuStyles" ).DeleteArrayElementAtIndex( i );
							serializedObject.ApplyModifiedProperties();

							minButtonCount.intValue = targ.RadialMenuStyles[ 0 ].buttonCount;
							maxButtonCount.intValue = targ.RadialMenuStyles[ targ.RadialMenuStyles.Count - 1 ].buttonCount;
							serializedObject.ApplyModifiedProperties();
							
							if( ShowStyleIndexes.Contains( i ) )
								ShowStyleIndexes.Remove( i );

							break;
						}
					}
				}
				EditorGUILayout.EndHorizontal();

				if( ShowStyleIndexes.Contains( i ) )
				{
					EditorGUI.BeginChangeCheck();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel( normalSpriteState + " Normal Sprite", collapsableSectionStyle, collapsableSectionStyle );
					EditorGUILayout.PropertyField( normalSprite[ i ], GUIContent.none );
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel( highlightedSpriteState + " Highlighted Sprite", collapsableSectionStyle, collapsableSectionStyle );
					EditorGUILayout.PropertyField( highlightedSprite[ i ], GUIContent.none );
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel( pressedSpriteState + " Pressed Sprite", collapsableSectionStyle, collapsableSectionStyle );
					EditorGUILayout.PropertyField( pressedSprite[ i ], GUIContent.none );
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel( selectedSpriteState + " Selected Sprite", collapsableSectionStyle, collapsableSectionStyle );
					EditorGUILayout.PropertyField( selectedSprite[ i ], GUIContent.none );
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel( disabledSpriteState + " Disabled Sprite", collapsableSectionStyle, collapsableSectionStyle );
					EditorGUILayout.PropertyField( disabledSprite[ i ], GUIContent.none );
					EditorGUILayout.EndHorizontal();

					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();
			}

			if( GUILayout.Button( "Add Button Count " + ( targ.maxButtonCount + 1 ).ToString(), EditorStyles.miniButton ) )
			{
				serializedObject.FindProperty( "RadialMenuStyles" ).arraySize++;
				serializedObject.ApplyModifiedProperties();

				int lastIndex = targ.RadialMenuStyles.Count - 1;

				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].buttonCount", lastIndex ) ).intValue = targ.maxButtonCount + 1;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].normalSprite", lastIndex ) ).objectReferenceValue = null;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].highlightedSprite", lastIndex ) ).objectReferenceValue = null;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].pressedSprite", lastIndex ) ).objectReferenceValue = null;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].selectedSprite", lastIndex ) ).objectReferenceValue = null;
				serializedObject.FindProperty( string.Format( "RadialMenuStyles.Array.data[{0}].disabledSprite", lastIndex ) ).objectReferenceValue = null;
				maxButtonCount.intValue++;
				serializedObject.ApplyModifiedProperties();

				StoreReferences();
			}
		}

		Repaint();
	}
}