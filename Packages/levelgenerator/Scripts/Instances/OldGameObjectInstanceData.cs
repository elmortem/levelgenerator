using System;
using LevelGenerator.Vectors;
using UnityEngine;

namespace LevelGenerator.Instances
{
	[Serializable]
	[Obsolete("Use GameObjectInstanceData instead")]
	public class OldGameObjectInstanceData : InstanceData
	{
		public GameObject Prefab;
		public VectorData Vector;
	}
}