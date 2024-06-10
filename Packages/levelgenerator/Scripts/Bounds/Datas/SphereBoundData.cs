using UnityEngine;

namespace LevelGenerator.Bounds.Datas
{
	public class SphereBoundData : BoundData
	{
		public Vector3 Offset;
		public float Radius;
		public Vector3 Scale = Vector3.one;

		private Vector3 _min;
		private Vector3 _max;
		
		public override Vector3 Min => _min;
		public override Vector3 Max => _max;
		
		public override bool InBounds(Vector3 point)
		{
			return (point - Offset).sqrMagnitude <= Radius * Radius;
		}

		public void Calc()
		{
			_min = new Vector3(Offset.x - Radius * Scale.x, Offset.y - Radius * Scale.y, Offset.z - Radius * Scale.z);
			_max = new Vector3(Offset.x + Radius * Scale.x, Offset.y + Radius * Scale.y, Offset.z + Radius * Scale.z);
		}
	}
}