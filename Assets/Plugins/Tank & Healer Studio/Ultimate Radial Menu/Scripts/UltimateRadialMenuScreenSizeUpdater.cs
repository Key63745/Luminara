/* UltimateRadialMenuScreenSizeUpdater.cs */
/* Written by Kaz Crowe */
using UnityEngine.EventSystems;

public class UltimateRadialMenuScreenSizeUpdater : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange ()
	{
		UltimateRadialMenu[] allRadialMenus = GetComponentsInChildren<UltimateRadialMenu>();

		for( int i = 0; i < allRadialMenus.Length; i++ )
			allRadialMenus[ i ].UpdatePositioning();
	}
}