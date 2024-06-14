using System;
using LevelGenerator.Vectors;
using UnityEngine;

namespace LevelGenerator.Instances
{
	[Serializable]
	public class GameObjectInstanceData : InstanceData
	{
		public GameObject Prefab;
		public VectorData Vector;
	}
}