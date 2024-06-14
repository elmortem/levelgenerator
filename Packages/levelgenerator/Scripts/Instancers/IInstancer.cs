using System.Collections.Generic;
using LevelGenerator.Instances;

namespace LevelGenerator.Instancers
{
	public interface IInstancer
	{
		int ObjectsCount { get; }
		
		void AddInstances(List<InstanceData> instances);
		void RemoveAll();
		
		void RaiseChange();
	}
}