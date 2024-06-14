using System.Collections.Generic;
using LevelGenerator.Instances;

namespace LevelGenerator.Instancers
{
	public interface IInstancer
	{
		int ObjectsCount { get; }

		bool TryAddInstances(IEnumerable<InstanceData> instances);
		void RemoveAll();
		
		void RaiseChange();
	}
}