using System;
using SerializeReferenceEditor;
using UnityEngine;

namespace LevelGenerator.Parameters.Types
{
	[Serializable, SRName("GameObject")]
	public class GameObjectParameter : IParameter
	{
		public string Name;
		public GameObject Value;
			
		public string GetName() => Name;

		public object GetValue() => Value;
		
		public Type GetParameterType() => typeof(GameObject);
	}
}