using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;

namespace LevelGenerator.Points
{
	public abstract class BasePointsNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		private GizmosOptions _gizmosOptions;
		
		private void UpdateGizmosOptions()
		{
			if (_gizmosOptions == null)
			{
				foreach (var provider in this.GetNodeInParent<IGizmosOptionsProvider>())
				{
					_gizmosOptions = provider.GetGizmosOptions();
					break;
				}
			}
		}
		
		public GizmosOptions GetGizmosOptions()
		{
			UpdateGizmosOptions();
			return _gizmosOptions;
		}
	}
}