using System;
using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	[Obsolete("Use PercentPointsNode instead")]
	public class OldPercentPointsNode : PreviewCalcNode, IPointCount
	{
		[Input] public List<Vector3> Points = new();
		public float Percent = 0.5f;
		[Output] public List<Vector3> Results = new();
		[Output] public List<Vector3> RemovedPoints = new();

		private float _lastPercent = -1;
		private List<Vector3> _results;
		private List<Vector3> _removedPoints;

		public int PointsCount => _results?.Count ?? 0;
		
		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return _results;
			}
			else if (port.fieldName == nameof(RemovedPoints))
			{
				CalcResults();
				return _removedPoints;
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

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<Vector3>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;
			
			var gizmosOptions = GetGizmosOptions();

			GizmosUtility.DrawPoints(results, gizmosOptions.PointSize, transform, gizmosOptions);
		}
#endif
	}
}