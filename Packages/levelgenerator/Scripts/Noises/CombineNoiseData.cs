using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelGenerator.Noises
{
	public enum NoiseChangeMode
	{
		Add,
		Subtract,
		Average,
		Min,
		Max
	}
	
	public class CombineNoiseData : NoiseData
	{
		public List<NoiseData> Noises = new();
		public NoiseChangeMode Mode = NoiseChangeMode.Average;

		public override float GetValue(float x, float y)
		{
			var value = Noises.First().GetValue(x, y);
			for (var i = 1; i < Noises.Count; i++)
			{
				var noise = Noises[i];
				if (Mode == NoiseChangeMode.Add)
					value = Mathf.Clamp01(value + noise.GetValue(x, y));
				else if (Mode == NoiseChangeMode.Subtract)
					value = Mathf.Clamp01(value - noise.GetValue(x, y));
				else if (Mode == NoiseChangeMode.Average)
					value = Mathf.Clamp01((value + noise.GetValue(x, y)) * 0.5f);
				else if (Mode == NoiseChangeMode.Min)
					value = Mathf.Min(value, noise.GetValue(x, y));
				else if (Mode == NoiseChangeMode.Max)
					value = Mathf.Max(value, noise.GetValue(x, y));
			}

			return value;
		}
	}
}