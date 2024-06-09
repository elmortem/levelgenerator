using System;
using UnityEngine;

namespace LevelGenerator.Bounds.Datas
{
	[Serializable]
	public class BoundData
	{
		public virtual Vector3 Min { get; }
		public virtual Vector3 Max { get; }
		public virtual bool InBounds(Vector3 point) => true;
	}
}