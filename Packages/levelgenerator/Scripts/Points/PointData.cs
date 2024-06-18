using System;
using UnityEngine;

namespace LevelGenerator.Points
{
	[Serializable]
	public struct PointData
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector3 Scale;
		public float Angle;
	}
}