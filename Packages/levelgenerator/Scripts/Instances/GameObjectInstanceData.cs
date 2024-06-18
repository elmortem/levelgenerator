using System;
using LevelGenerator.Points;
using UnityEngine;

namespace LevelGenerator.Instances
{
	[Serializable]
	public class GameObjectInstanceData : InstanceData
	{
		public GameObject Prefab;
		public PointData Point;
	}
}