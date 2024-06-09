using LevelGenerator.Noises;
using UnityEngine;

namespace LevelGenerator.Bounds.Datas
{
	public class NoiseBoundData : BoundData
	{
		public NoiseData Data;
		public BoundData BoundData;
		public float MinValue = 0.5f;
		public float MaxValue = 1f;
		public bool Is2D;

		public override Vector3 Min => BoundData.Min;
		public override Vector3 Max => BoundData.Max;
		public override bool InBounds(Vector3 point)
		{
			if (BoundData.InBounds(point))
			{
				var value = Data.GetValue(point.x, Is2D ? point.y : point.z);
				return value >= MinValue && value <= MaxValue;
			}

			return false;
		}
	}
}