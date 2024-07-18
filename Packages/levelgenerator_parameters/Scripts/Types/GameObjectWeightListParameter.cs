using System;
using System.Collections.Generic;
using LevelGenerator.Instances;
using SerializeReferenceEditor;

namespace LevelGenerator.Parameters.Types
{
	[Serializable, SRName("Game Object Weights")]
	public class GameObjectWeightListParameter :IParameter
	{
		public string Name = "Prefabs";
		public List<GameObjectWeight> Values = new();
		public string GetName() => Name;

		public object GetValue() => Values;

		public Type GetParameterType() => typeof(List<GameObjectWeight>);
	}
}