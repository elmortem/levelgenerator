using System;
using SerializeReferenceEditor;
using UnityEngine.Splines;

namespace LevelGenerator.Parameters.Types
{
	[Serializable, SRName("Spline")]
	public class SplineContainerParameter : IParameter
	{
		public string Name;
		public SplineContainer Value;

		public string GetName() => Name;

		public object GetValue() => Value;

		public Type GetParameterType() => typeof(SplineContainer);
	}
}