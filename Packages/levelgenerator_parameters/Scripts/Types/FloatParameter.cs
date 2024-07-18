using System;
using SerializeReferenceEditor;

namespace LevelGenerator.Parameters.Types
{
	[Serializable, SRName("Float")]
	public class FloatParameter : IParameter
	{
		public string Name;
		public float Value;
			
		public string GetName() => Name;

		public object GetValue() => Value;
		
		public Type GetParameterType() => typeof(float);
	}
}