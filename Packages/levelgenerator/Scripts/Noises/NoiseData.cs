using System;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Noises
{
	[Serializable]
	public class NoiseData
	{
		public virtual float GetValue(float x, float y)
		{
			return (x + y) - Mathf.FloorToInt(x + y);
		}
	}
}