using System;
using SerializeReferenceEditor;

namespace LevelGenerator.Parameters.Types
{
	[Serializable, SRName("String")]
	public class StringParameter : IParameter
	{
		public string Name;
		public string Value;
			
		public string GetName() => Name;

		public object GetValue() => Value;
		
		public Type GetParameterType() => typeof(string);
	}
}