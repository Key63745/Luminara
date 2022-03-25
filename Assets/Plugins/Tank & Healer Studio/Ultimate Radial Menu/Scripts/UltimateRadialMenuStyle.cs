/* UltimateRadialMenuStyle.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu( fileName = "Ultimate Radial Menu Style", menuName = "Tank and Healer Studio/Ultimate Radial Menu Style", order = 1 )]
public class UltimateRadialMenuStyle : ScriptableObject
{
	public int minButtonCount = 3;
	public int maxButtonCount = 12;

	[Serializable]
	public class RadialMenuStyle
	{
		public int buttonCount;
		public Sprite normalSprite, highlightedSprite;
		public Sprite pressedSprite, selectedSprite;
		public Sprite disabledSprite;
	}
	public List<RadialMenuStyle> RadialMenuStyles = new List<RadialMenuStyle>();
}