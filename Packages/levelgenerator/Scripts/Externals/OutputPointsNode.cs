using System.Collections.Generic;
using LevelGenerator.Points;
using XNode;

namespace LevelGenerator.Externals
{
	public class OutputPointsNode : PreviewCalcNode, INodePointCount
	{
		[Input] public List<PointData> Points;
		public string Name;
		[Output] public List<PointData> Results;

		private List<PointData> _results;
		
		public int PointsCount => _results?.Count ?? 0;

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return _results;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			if (LockCalc && _results != null)
				return;
			if (!force && _results != null)
				return;

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();

			foreach (var points in pointsList)
			{
				if(points == null || points.Count <= 0)
					continue;
				
				_results.AddRange(points);
			}
		}
	}
}