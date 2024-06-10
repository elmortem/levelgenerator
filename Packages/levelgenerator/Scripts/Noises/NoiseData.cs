using System;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Noises
{
	[Serializable]
	public abstract class NoiseData
	{
		public abstract float GetValue(float x, float y); // => 0.5f;
	}
}