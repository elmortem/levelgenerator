using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class RemoveIntersectPointsNode : PreviewNode, IGizmosOptionsProvider
	{
		[Input] public List<Vector3> Points = new();
		[Input] public List<Vector3> OtherPoints = new();
		public float Radius = 1;
		public bool RemoveThemselves = true;
		[Output] public List<Vector3> Results = new();
		
		private List<Vector3> _otherPoints = new();
		private List<Vector3> _results;
		private GizmosOptions _gizmosOptions;
		
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

			_gizmosOptions = null;
			
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
			var results = (List<Vector3>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;
			
			var pos = transform.position;
			
			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			foreach (var point in results)
			{
				Gizmos.DrawWireSphere(pos + point, Radius);
			}
		}
#endif
	}
}