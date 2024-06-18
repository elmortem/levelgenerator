using System;
using UnityEngine;

namespace LevelGenerator.Vectors
{
	[Serializable]
	[Obsolete]
	public struct VectorData
	{
		public Vector3 Point;
		public Vector3 Euler;
		public Vector3 Scale;
	}
}