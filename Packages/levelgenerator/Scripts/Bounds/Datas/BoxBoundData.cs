using System;
using UnityEngine;

namespace LevelGenerator.Bounds.Datas
{
	[Serializable]
	[Obsolete]
	public class BoxBoundData : BoundData
	{
		public UnityEngine.Bounds Bounds;

		public override Vector3 Min => Bounds.min;
		public override Vector3 Max => Bounds.max;

		public override bool InBounds(Vector3 point)
		{
			return Bounds.Contains(point);
		}
	}
}