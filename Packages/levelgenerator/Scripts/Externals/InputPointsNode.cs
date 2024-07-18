using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Externals
{
	public class InputPointsNode : PreviewCalcNode, INodePointCount
	{
		public NodeGraph Graph;
		public string Name;
		[Output] public List<PointData> Results;

		private string _lastName;
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
			if (Graph == null)
			{
				_results = null;
				return;
			}
			
			if (LockCalc && _results != null)
				return;
			if (!force && _results != null && _lastName == Name)
				return;

			var outputNode = Graph.nodes.FirstOrDefault(p => p is OutputPointsNode output && output.Name == Name);
			if (outputNode == null)
			{
				_results = null;
				return;
			}
			
			var points = (List<PointData>)outputNode.GetValue(GetOutputPort(nameof(Results)));
			if (points == null || points.Count <= 0)
			{
				_results = null;
				return;
			}

			if (_results == null)
				_results = new();
			else
				_results.Clear();

			_results.AddRange(points);
		}
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<PointData>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, gizmosOptions, transform);
		}
#endif
	}
}