using System.Collections.Generic;
using UnityEngine;

namespace LevelGenerator.Bounds.Datas
{
	public class CombineBoundData : BoundData
	{
		public List<BoundData> Includes = new();
		public List<BoundData> Excludes = new();

		private Vector3 _min;
		private Vector3 _max;

		public void Calc()
		{
			_min = Vector3.zero;
			_max = Vector3.zero;
			
			if(Includes.Count <= 0)
				return;

			_min = Includes[0].Min;
			_max = Includes[0].Max;

			for (int i = 1; i < Includes.Count; i++)
			{
				_min.x = Mathf.Min(_min.x, Includes[i].Min.x);
				_min.y = Mathf.Min(_min.y, Includes[i].Min.y);
				_min.z = Mathf.Min(_min.z, Includes[i].Min.z);
				_max.x = Mathf.Max(_max.x, Includes[i].Max.x);
				_max.y = Mathf.Max(_max.y, Includes[i].Max.y);
				_max.z = Mathf.Max(_max.z, Includes[i].Max.z);
			}
		}

		public override Vector3 Min => _min;

		public override Vector3 Max => _max;
		public override bool InBounds(Vector3 point)
		{
			foreach (var bound in Excludes)
			{
				if (bound.InBounds(point))
					return false;
			}
			
			foreach (var bound in Includes)
			{
				if (bound.InBounds(point))
					return true;
			}

			return false;
		}
	}
}