using System;

namespace LevelGenerator.Parameters.Types
{
	public interface IParameter
	{
		string GetName();
		object GetValue();
		Type GetParameterType();
	}
}