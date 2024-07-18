using System;
using SerializeReferenceEditor;
using UnityEngine;

namespace LevelGenerator.Parameters.Types
{
	[Serializable, SRName("Vector3")]
	public class Vector3Parameter : IParameter
	{
		public string Name;
		public Vector3 Value;

		public string GetName() => Name;

		public object GetValue() => Value;

		public Type GetParameterType() => typeof(Vector3);
	}
}