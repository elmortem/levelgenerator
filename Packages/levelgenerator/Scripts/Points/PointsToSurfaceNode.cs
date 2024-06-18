using System.Collections.Generic;
using LevelGenerator.Surfaces.Datas;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class PointsToSurfaceNode : BasePointsNode
	{
		[Input(connectionType = ConnectionType.Override)] public BaseSurfaceData Surface = new();
		[Input] public List<PointData> Points = new();
		[NodeEnum]
		public ProjectionPointMode ProjectionMode;
		[Output] public List<PointData> Results = new();

		private ProjectionPointMode _lastProjectionMode;
		private List<PointData> _results;
		
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
			
			port = GetInputPort(nameof(Surface));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			if (LockCalc && _results != null)
				return;
			if (!force && _lastProjectionMode == ProjectionMode && _results != null)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if(pointsList == null || pointsList.Length <= 0)
				return;
			
			var surface = GetInputValue(nameof(Surface), Surface);
			if(surface == null)
				return;
			
			_gizmosOptions = null;

			_lastProjectionMode = ProjectionMode;

			foreach (var points in pointsList)
			{
				surface.ProjectionPoints(points, ProjectionMode, _results);	
			}
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<PointData>)GetValue(resultsPort);
			if (results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, _gizmosOptions, transform);
		}
#endif
	}
}