using UnityEngine;

namespace LevelGenerator.Bounds.Datas
{
	public class SphereBoundData : BoundData
	{
		public Vector3 Offset;
		public float Radius;
		public Vector3 Scale = Vector3.one;
		
		public override Vector3 Min => new Vector3(Offset.x - Radius * Scale.x, Offset.y - Radius * Scale.y, Offset.z - Radius * Scale.z);
		public override Vector3 Max => new Vector3(Offset.x + Radius * Scale.x, Offset.y + Radius * Scale.y, Offset.z + Radius * Scale.z);
		public override bool InBounds(Vector3 point)
		{
			return (point - Offset).sqrMagnitude <= Radius * Radius;
		}
	}
}