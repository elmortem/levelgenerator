using System;
using UnityEngine;

namespace LevelGenerator.Noises
{
	[Serializable]
	public class CircleNoiseData : NoiseData
	{
		public Vector2 Offset = Vector2.zero;
		public float Radius = 100f;
		public bool Invert;

		public override float GetValue(float x, float y)
		{
			Vector2 pos = new Vector2(x, y) + Offset;
			float distance = Vector2.Distance(pos, Vector2.zero);
			var value = Mathf.Clamp01(distance / Radius);
			return Invert ? 1f - value : value;
		}
	}
}