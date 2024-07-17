using System;
using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	[Obsolete("Use IntersectPointsNode instead")]
	public class OldRemoveIntersectPointsNode : PreviewNode, INodePointCount
	{
		[Input] public List<Vector3> Points = new();
		[Input] public List<Vector3> OtherPoints = new();
		public float Radius = 1;
		public bool RemoveThemselves = true;
		[Output] public List<Vector3> Results = new();
		
		private readonly List<Vector3> _otherPoints = new();
		private List<Vector3> _results;
		
		public int PointsCount => _results?.Count ?? 0;
		
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if (to.IsInput)
			{
				CalcPoints(true);
			}
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if (port.IsInput)
			{
				CalcPoints(true);
			}
		}

		protected override void ApplyChange()
		{
			CalcPoints(true);
			base.ApplyChange();
		}

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;
			
			if (port.fieldName == nameof(Results))
			{
				CalcPoints();
				return _results;
			}
			
			return null;
		}

		private void CalcPoints(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			if(LockCalc && _results != null)
				return;
			if(!force && _results != null)
				return;

			var pointsList = GetInputValues(nameof(Points), Points);
			if(pointsList == null || pointsList.Length <= 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();

			ResetGizmosOptions();
			
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
						continue;

					_results.Add(point);
				}
			}
		}

		private bool IsIntersects(Vector3 point)
		{
			if (RemoveThemselves)
			{
				foreach (var result in _results)
				{
					if (Vector3.Distance(point, result) < Radius * 2f)
						return true;
				}
			}

			foreach (var otherPoint in _otherPoints)
			{
				if (Vector3.Distance(point, otherPoint) < Radius * 2f)
					return true;
			}

			return false;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<Vector3>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;
			
			GizmosUtility.DrawWirePoints(results, Radius, transform, gizmosOptions);
		}
#endif
	}
}