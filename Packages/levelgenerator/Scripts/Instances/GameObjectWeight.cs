using System;
using UnityEngine;

namespace LevelGenerator.Instances
{
	[Serializable]
	public class GameObjectWeight
	{
		public GameObject Prefab;
		public float Weight = 1f;
	}
}