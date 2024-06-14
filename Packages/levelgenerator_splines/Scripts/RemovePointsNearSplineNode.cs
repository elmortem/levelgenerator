using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines
{
	public class RemovePointsNearSplineNode : BasePointsNode
	{
		[Input] public List<Vector3> Points;
		[Input] public SplineContainerData SplineContainer;
		public float Distance = 1f;
		[Output] public List<Vector3> Results;

		private float _lastDistance;
		private List<Vector3> _results;

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

			if (Distance < 0.0001f)
				return;

			if (LockCalc && _results != null)
				return;
			if (!force && Mathf.Approximately(_lastDistance, Distance) && _results != null)
				return;

			if (_results == null)
				_results = new();
			else
				_results.Clear();

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length <= 0)
				return;

			var splines = GetInputValues(nameof(SplineContainer), SplineContainer);
			if (splines == null || splines.Length <= 0)
				return;

			_lastDistance = Distance;

			foreach (var points in pointsList)
			{
				foreach (var point in points)
				{
					if(!CheckNearSpline(point, splines))
						_results.Add(point);
				}
			}
		}

		private bool CheckNearSpline(Vector3 point, SplineContainerData[] splineContainers)
		{
			if (splineContainers == null)
				return false;

			var dv = new Vector3(Distance, Distance, Distance);
			
			foreach (var container in splineContainers)
			{
				if (container == null)
					continue;
				if (container.Splines == null || container.Splines.Count <= 0)
					continue;
				
				foreach (var spline in container.Splines)
				{
					var bounds = spline.GetBounds();
					bounds.SetMinMax(bounds.min - dv, bounds.max + dv);
					if (!bounds.Contains(point))
						return false;
					
					var dist = SplineUtility.GetNearestPoint(spline, point, out var nearest, out var t);
					if (dist <= Distance)
						return true;
				}
			}

			return false;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<Vector3>)GetValue(resultsPort);
			if (results == null || results.Count <= 0)
				return;
			
			UpdateGizmosOptions();
			
			DrawPoints(results, _gizmosOptions?.PointSize ?? 0.2f, transform, _gizmosOptions);
		}
#endif
	}
}