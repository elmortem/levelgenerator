using System.Collections.Generic;
using LevelGenerator.Instances;

namespace LevelGenerator.Instancers
{
	public interface IInstancer
	{
		void AddInstances(List<InstanceData> instances);
		void RemoveAll();
	}
}