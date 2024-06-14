using System;
using LevelGenerator.Instances;
using UnityEngine.U2D;

namespace LevelGenerator.Splines
{
	[Serializable]
	public class SpriteShapeInstanceData : InstanceData
	{
		public string Name;
		public SplineContainerData SplineContainer;
		public SpriteShape SpriteShape;
		public float Height;
	}
}