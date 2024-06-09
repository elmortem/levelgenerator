using UnityEngine;

namespace LevelGenerator.Utility
{
	public static class ColorUtility
	{
		public static Color SetAlpha(this Color color, float alpha)
		{
			color.a = alpha;
			return color;
		}
	}
}