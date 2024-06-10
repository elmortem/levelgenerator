using UnityEngine;

namespace LevelGenerator.Utility
{
	public static class NoiseUtility
	{
		public static float GetPerlinNoise(float x, float y, float scale)
		{
			return GetPerlinNoise(x, y, 0f, 0f, scale);
		}
		
		public static float GetPerlinNoise(float x, float y, float offsetX, float offsetY, float scale)
		{
			var nx = x * scale + offsetX;
			var ny = y * scale + offsetY;

			return Mathf.PerlinNoise(nx, ny);
		}
	}
}