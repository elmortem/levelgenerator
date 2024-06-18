using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class PercentPointsNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		[Input] public List<PointData> Points = new();
		public float Percent = 0.5f;
		[Output] public List<PointData> Results = new();
		[Output] public List<PointData> RemovedPoints = new();

		private float _lastPercent = -1;
		private List<PointData> _results;
		private List<PointData> _removedPoints;
		private GizmosOptions _gizmosOptions;

		public int PointsCount => _results?.Count ?? 0;
		
		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return _results ?? Results;
			}
			else if (port.fieldName == nameof(RemovedPoints))
			{
				CalcResults();
				return _removedPoints ?? RemovedPoints;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				_removedPoints = null;
				return;
			}
			
			if(LockCalc && _results != null)
				return;
			if (!force && Mathf.Approximately(_lastPercent, Percent) && _results != null)
				return;

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();
			
			if (_removedPoints == null)
				_removedPoints = new();
			else
				_removedPoints.Clear();

			_lastPercent = Percent;

			foreach (var points in pointsList)
			{
				var count = Mathf.RoundToInt(points.Count * Percent);
				for (int i = 0; i < points.Count; i++)
				{
					if(i < count)
						_results.Add(points[i]);
					else
						_removedPoints.Add(points[i]);
				}
			}
		}
		
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

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<PointData>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, _gizmosOptions, transform);
		}
#endif
	}
}