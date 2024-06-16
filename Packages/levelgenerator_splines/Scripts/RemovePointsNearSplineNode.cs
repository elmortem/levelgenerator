using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines
{
	public class RemovePointsNearSplineNode : BasePointsNode
	{
		[Input] public List<Vector3> Points;
		[FormerlySerializedAs("SplineContainer")]
		[Input] public SplineContainerData SplineContainers;
		public float Distance = 1f;
		[Output] public List<Vector3> Results;
		[Output] public List<Vector3> RemovedPoints;

		private float _lastDistance;
		private List<Vector3> _results;
		private List<Vector3> _removedPoints;
		private Dictionary<Spline, UnityEngine.Bounds> _boundsCache = new();
		private List<Vector3> _pointsCache = new();

		public override object GetValue(NodePort port)
		{
			if (port == null)
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
			
			if(_removedPoints == null)
				_removedPoints = new();
			else
				_removedPoints.Clear();

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length <= 0)
				return;

			var splines = GetInputValues(nameof(SplineContainers), SplineContainers);
			if (splines == null || splines.Length <= 0)
				return;

			_lastDistance = Distance;
			
			_boundsCache.Clear();
			_pointsCache.Clear();

			foreach (var points in pointsList)
			{
				foreach (var point in points)
				{
					if(!CheckNearSpline(point, splines))
						_results.Add(point);
					else
						_removedPoints.Add(point);
				}
			}
		}

		private bool CheckNearSpline(Vector3 point, SplineContainerData[] splineContainers)
		{
			if (splineContainers == null)
				return false;

			// ultra optimize
			if (_pointsCache.Count <= 0)
			{
				foreach (var container in splineContainers)
				{
					if (container?.Splines == null || container.Splines.Count == 0)
						continue;

					foreach (var spline in container.Splines)
					{
						var splineLen = spline.GetLength();
						var count = Mathf.RoundToInt(splineLen / Distance * 1.5f) + 2;
						var step = 1f / count;

						for (int i = 0; i <= count; i++)
						{
							_pointsCache.Add(spline.EvaluatePosition(i * step));
						}
					}
				}
			}
			
			foreach (var pointCache in _pointsCache)
			{
				if ((point - pointCache).sqrMagnitude < Distance * Distance)
				{
					return true;
				}
			}

			// low optimize
			/*foreach (var container in splineContainers)
			{
				if (container?.Splines == null || container.Splines.Count == 0)
					continue;
				
				foreach (var spline in container.Splines)
				{
					if (!_boundsCache.TryGetValue(spline, out var bounds))
					{
						bounds = spline.GetBounds();
						bounds.Expand(Distance);
						_boundsCache[spline] = bounds;
					}

					if (bounds.Contains(point))
					{
						var dist = SplineUtility.GetNearestPoint(spline, point, out var nearest, out var t, 2, 1);
						if (dist <= Distance)
							return true;
					}
				}
			}*/

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

			if (_pointsCache.Count > 0)
			{
				Gizmos.color = _gizmosOptions?.Color ?? Color.white;
				foreach (var point in _pointsCache)
				{
					Gizmos.DrawWireSphere(point, Distance);
				}
			}
		}
#endif
	}
}