using System;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Noises
{
	[Serializable]
	public class PerlinNoiseData : NoiseData
	{
		public Vector2 Offset = Vector2.zero;
		public float Scale = 5f;
		
		public override float GetValue(float x, float y)
		{
			return NoiseUtility.GetPerlinNoise(x, y, Offset.x, Offset.y, Scale);
		}
	}
}