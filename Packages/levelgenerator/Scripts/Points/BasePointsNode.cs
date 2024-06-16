using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Points
{
	public abstract class BasePointsNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		protected GizmosOptions _gizmosOptions;
		
		protected void UpdateGizmosOptions()
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