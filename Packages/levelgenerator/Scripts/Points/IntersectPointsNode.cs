using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class IntersectPointsNode : BasePointsNode
	{
		[Input] public List<PointData> Points = new();
		[Input] public List<PointData> OtherPoints = new();
		public float Radius = 1f;
		public bool RemoveThemselves = true;
		public bool UseScale = true; 
		[Header("Gizmos")]
		public bool ShowResults = true;
		[Output] public List<PointData> Results = new();
		[Output] public List<PointData> NearPoints = new();
		
		private float _lastRadius;
		private bool _lastRemoveThemselves;
		private bool _lastUseScale;
		private List<PointData> _otherPoints = new();
		private List<PointData> _results;
		private List<PointData> _nearPoints;
		
		public int PointsCount => _results?.Count ?? 0;
		

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;
			
			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return _results ?? Results;
			}
			
			if (port.fieldName == nameof(NearPoints))
			{
				CalcResults();
				return _nearPoints ?? NearPoints;
			}
			
			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				_nearPoints = null;
				return;
			}

			if (!RemoveThemselves)
			{
				port = GetInputPort(nameof(OtherPoints));
				if (port == null || !port.IsConnected)
				{
					_results = null;
					_nearPoints = null;
					return;
				}
			}
			
			if(LockCalc && _results != null)
				return;
			if(!force && Mathf.Approximately(_lastRadius, Radius) && _lastRemoveThemselves == RemoveThemselves && _lastUseScale == UseScale && _results != null)
				return;

			var pointsList = GetInputValues(nameof(Points), Points);
			if(pointsList == null || pointsList.Length <= 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();
			
			if (_nearPoints == null)
				_nearPoints = new();
			else
				_nearPoints.Clear();

			_gizmosOptions = null;
			
			_lastRadius = Radius;
			_lastRemoveThemselves = RemoveThemselves;
			
			_otherPoints.Clear();
			var otherPointsList = GetInputValues(nameof(OtherPoints), OtherPoints);
			if (otherPointsList != null && otherPointsList.Length > 0)
			{
				foreach (var otherPoints in otherPointsList)
				{
					if(otherPoints != null && otherPoints.Count > 0)
						_otherPoints.AddRange(otherPoints);
				}
			}

			foreach (var points in pointsList)
			{
				if(points == null)
					continue;
				foreach (var point in points)
				{
					if (IsIntersects(point))
						_nearPoints.Add(point);
					else
						_results.Add(point);
				}
			}
		}

		private bool IsIntersects(PointData point)
		{
			var diameter = UseScale 
				? Radius * 2f * point.Scale.Avarage() 
				: Radius * 2f;
			
			if (RemoveThemselves)
			{
				foreach (var result in _results)
				{
					if (Vector3.Distance(point.Position, result.Position) < diameter)
						return true;
				}
			}

			foreach (var otherPoint in _otherPoints)
			{
				if (Vector3.Distance(point.Position, otherPoint.Position) < diameter)
					return true;
			}

			return false;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();

			if (ShowResults)
			{
				var resultsPort = GetOutputPort(nameof(Results));
				var results = (List<PointData>)GetValue(resultsPort);
				if (results == null || results.Count <= 0)
					return;
				
				GizmosUtility.DrawWirePoints(results, Radius, _gizmosOptions?.Color ?? Color.white, transform);
			}
			else
			{
				var resultsPort = GetOutputPort(nameof(NearPoints));
				var results = (List<PointData>)GetValue(resultsPort);
				if (results == null || results.Count <= 0)
					return;
				
				GizmosUtility.DrawPoints(_nearPoints, _gizmosOptions.PointSize, _gizmosOptions.DrawNormals,
					_gizmosOptions.DrawRotation, Color.red, transform);
			}
		}
#endif
	}
}