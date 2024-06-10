using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGenerator.Bounds.Datas
{
	[Serializable]
	public class BoundData
	{
		public virtual Vector3 Min { get; }
		public virtual Vector3 Max { get; }
		public float Volume => (Max - Min).sqrMagnitude;
		public virtual bool InBounds(Vector3 point) => true;

		public virtual IEnumerable<BoundData> GetBounds()
		{
			yield return this;
		}
	}
}