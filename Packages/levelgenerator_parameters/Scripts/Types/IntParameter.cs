using System;
using SerializeReferenceEditor;

namespace LevelGenerator.Parameters.Types
{
	[Serializable, SRName("Int")]
	public class IntParameter : IParameter
	{
		public string Name;
		public int Value;
			
		public string GetName() => Name;

		public object GetValue() => Value;
		
		public Type GetParameterType() => typeof(int);
	}
}