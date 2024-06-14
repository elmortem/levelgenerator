using System.Collections.Generic;
using LevelGenerator.Instances;
using XNode;

namespace LevelGenerator.Splines
{
	public abstract class SplineInstanceNode<TInstanceData> : PreviewCalcNode where TInstanceData : InstanceData
	{
		[Input] public SplineContainerData SplineContainer;
		[Output] public List<TInstanceData> Results;

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return Results;
			}

			return null;
		}
	}
}