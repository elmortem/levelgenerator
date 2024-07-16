using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelGenerator.NodeGizmos
{
	[Serializable]
	public class GizmosOptions
	{
		public bool Override = false;
		public float NoiseHeight = 10f;
		public float NoiseSegment = 1f;
		public float NoiseSize = 0.1f;
		public int BoundPoints = 100;
		public bool DrawIncorrects = true;
		public Color Color = Color.white;
		public float PointSize = 0.3f;
		public bool DrawNormals = true;
		public bool DrawRotation = true;
	}
}