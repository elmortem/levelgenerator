using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Points
{
	public class PointsNearSplineNode : PreviewCalcNode
	{
		[Input] public List<PointData> Points;
		[FormerlySerializedAs("SplineContainer")]
		[Input] public SplineContainerData SplineContainers;
		public float Distance = 1f;
		[Header("Gizmos")]
		public bool ShowFarPoints = true;
		public bool ShowNearZone = true;
		[Output] public List<PointData> FarPoints;
		[Output] public List<PointData> NearPoints;

		private float _lastDistance;
		private List<PointData> _farPoints;
		private List<PointData> _nearPoints;
		private readonly List<Vector3> _pointsCache = new();
		
		public int PointsCount => ShowFarPoints ? _farPoints?.Count ?? 0 : _nearPoints?.Count ?? 0;

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(FarPoints))
			{
				CalcResults();
				return _farPoints;
			}
			else if (port.fieldName == nameof(NearPoints))
			{
				CalcResults();
				return _nearPoints;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_farPoints = null;
				_nearPoints = null;
				_pointsCache.Clear();
				return;
			}
			
			port = GetInputPort(nameof(SplineContainers));
			if (port == null || !port.IsConnected)
			{
				_farPoints = null;
				_nearPoints = null;
				_pointsCache.Clear();
				return;
			}

			if (Distance < 0.0001f)
				return;

			if (LockCalc && _farPoints != null && _nearPoints != null)
				return;
			if (!force && Mathf.Approximately(_lastDistance, Distance) && _farPoints != null && _nearPoints != null)
				return;

			if (_farPoints == null)
				_farPoints = new();
			else
				_farPoints.Clear();
			
			if(_nearPoints == null)
				_nearPoints = new();
			else
				_nearPoints.Clear();

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length <= 0)
				return;

			var splines = GetInputValues(nameof(SplineContainers), SplineContainers);
			if (splines == null || splines.Length <= 0)
				return;

			ResetGizmosOptions();
			_lastDistance = Distance;
			
			_pointsCache.Clear();

			foreach (var points in pointsList)
			{
				if(points == null)
					continue;
				
				foreach (var point in points)
				{
					if(!CheckNearSpline(point, splines))
						_farPoints.Add(point);
					else
						_nearPoints.Add(point);
				}
			}
		}

		private bool CheckNearSpline(PointData point, SplineContainerData[] splineContainers)
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
			
			var sqrDist = Distance * Distance;
			foreach (var pointCache in _pointsCache)
			{
				if ((point.Position - pointCache).sqrMagnitude < sqrDist)
				{
					return true;
				}
			}

			return false;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();

			if (ShowFarPoints)
			{
				var farPointsPort = GetOutputPort(nameof(FarPoints));
				var farPoints = (List<PointData>)GetValue(farPointsPort);
				if (farPoints == null || farPoints.Count <= 0)
					return;

				GizmosUtility.DrawPoints(farPoints, gizmosOptions, transform);
			}
			else
			{
				var nearPointsPort = GetOutputPort(nameof(NearPoints));
				var nearPoints = (List<PointData>)GetValue(nearPointsPort);
				if (nearPoints == null || nearPoints.Count <= 0)
					return;

				GizmosUtility.DrawPoints(nearPoints, gizmosOptions, transform);
			}

			if (ShowNearZone && _pointsCache.Count > 0)
			{
				Gizmos.color = gizmosOptions.Color;
				foreach (var point in _pointsCache)
				{
					Gizmos.DrawWireSphere(point, Distance);
				}
			}
		}
#endif
	}
}