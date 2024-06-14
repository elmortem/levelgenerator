using System.Collections.Generic;

namespace LevelGenerator.Instancers
{
	public interface IInstancerOwner
	{
		void SetInstancers(IEnumerable<IInstancer> instancers);
	}
}