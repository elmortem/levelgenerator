using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelGenerator.NodeGizmos
{
	[Serializable]
	public class GizmosOptions
	{
		public float NoiseHeight = 10f;
		public int BoundPoints = 100;
		public bool DrawIncorrects = true;
		public Color Color = Color.white;
	}
}