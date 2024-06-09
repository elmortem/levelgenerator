using System;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Noises
{
	[Serializable]
	public class NoiseData
	{
		public Vector2 Offset = Vector2.zero;
		public float Scale = 5f;
		
		public float GetValue(float x, float y)
		{
			return NoiseUtility.GetNoise(x, y, Offset.x, Offset.y, Scale);
		}
	}
}