using System;
using LevelGenerator.Vectors;
using UnityEngine;

namespace LevelGenerator.Instances
{
	[Serializable]
	public struct InstanceData
	{
		public GameObject Prefab;
		public VectorData Vector;
	}
}