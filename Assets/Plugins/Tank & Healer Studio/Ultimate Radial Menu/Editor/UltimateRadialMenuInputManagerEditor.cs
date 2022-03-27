/* UltimateRadialMenuInputManagerEditor.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

[CustomEditor( typeof( UltimateRadialMenuInputManager ) )]
public class UltimateRadialMenuInputManagerEditor : Editor
{
	UltimateRadialMenuInputManager targ;
	bool mulitpleInputManagerError = false;

	private void OnEnable ()
	{
		mulitpleInputManagerError = FindObjectsOfType<UltimateRadialMenuInputManager>().Length > 1;

		targ = ( UltimateRadialMenuInputManager )target;
	}

	public override void OnInspectorGUI ()
	{
		if( !targ.gameObject.GetComponent<EventSystem>() )
		{
			EditorGUILayout.BeginVertical( "Box" );
			GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
			warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
			warningStyle.alignment = TextAnchor.MiddleCenter;
			EditorGUILayout.LabelField( "WARNING", warningStyle );

			GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
			EditorGUILayout.LabelField( "The Ultimate Radial Menu Input Manager needs to be located on the EventSystem in your scene.", labelStyle );
			
			EditorGUILayout.EndVertical();
		}
		else if( mulitpleInputManagerError )
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
					if( !allInputManagers[ i ].GetComponent<EventSystem>() )
						DestroyImmediate( allInputManagers[ i ] );
				}

				if( !FindObjectOfType<EventSystem>().gameObject.GetComponent<UltimateRadialMenuInputManager>() )
					FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();

				mulitpleInputManagerError = FindObjectsOfType<UltimateRadialMenuInputManager>().Length > 1;
			}

			EditorGUILayout.EndVertical();
		}
		else
		{
			base.OnInspectorGUI();
		}
	}
}