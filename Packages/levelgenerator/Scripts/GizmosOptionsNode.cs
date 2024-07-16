using LevelGenerator.NodeGizmos;
using XNode;

namespace LevelGenerator
{
	public class GizmosOptionsNode : GenNode
	{
		public GizmosOptions GizmosOptions = new();
		[Output] public GizmosOptions Result;

		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;

			if (port.fieldName == nameof(Result))
			{
				return GizmosOptions;
			}
			
			return null;
		}
	}
}