using System.Collections.Generic;
using LevelGenerator.Surfaces.Datas;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class SurfaceSelectPointsNode : PreviewCalcNode
	{
		[Input(connectionType = ConnectionType.Override)] public BaseSurfaceData Surface;
		[Input] public List<PointData> Points;
		[Header("Gizmos")]
		public bool ShowInside = true;
		public bool ShowOutside;
		[Output] public List<PointData> InsidePoints;
		[Output] public List<PointData> OutsidePoints;

		private List<PointData> _insidePoints;
		private List<PointData> _outsidePoints;

		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(InsidePoints))
			{
				CalcResults();
				return _insidePoints;
			}
			
			if (port.fieldName == nameof(OutsidePoints))
			{
				CalcResults();
				return _outsidePoints;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Surface));
			if (port == null || !port.IsConnected)
			{
				_insidePoints = null;
				_outsidePoints = null;
				return;
			}
			
			port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_insidePoints = null;
				_outsidePoints = null;
				return;
			}
			
			if(LockCalc && _insidePoints != null && _outsidePoints != null)
				return;
			if (!force && _insidePoints != null && _outsidePoints != null)
				return;

			var surface = GetInputValue<BaseSurfaceData>(nameof(Surface));
			if(surface == null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if (_insidePoints == null)
				_insidePoints = new();
			else
				_insidePoints.Clear();
			
			if (_outsidePoints == null)
				_outsidePoints = new();
			else
				_outsidePoints.Clear();

			foreach (var points in pointsList)
			{
				if(points == null || points.Count <= 0)
					continue;
				
				for (int i = 0; i < points.Count; i++)
				{
					if(surface.Inside(points[i]))
						_insidePoints.Add(points[i]);
					else
						_outsidePoints.Add(points[i]);
				}
			}
		}
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var surface = GetInputValue<BaseSurfaceData>(nameof(Surface));
			if (surface != null)
			{
				Gizmos.color = gizmosOptions.Color;
				surface.DrawGizmos(transform);
			}

			if (ShowInside)
			{
				var resultsPort = GetOutputPort(nameof(InsidePoints));
				var results = (List<PointData>)GetValue(resultsPort);
				if (results == null || results.Count <= 0)
					return;

				GizmosUtility.DrawPoints(results, gizmosOptions, transform);
			}
			
			if (ShowOutside)
			{
				var resultsPort = GetOutputPort(nameof(OutsidePoints));
				var results = (List<PointData>)GetValue(resultsPort);
				if (results == null || results.Count <= 0)
					return;

				var pointColor = !ShowInside ? gizmosOptions.Color : Color.white - gizmosOptions.Color;
				pointColor.a = 1f;
				GizmosUtility.DrawPoints(results, gizmosOptions.PointSize, gizmosOptions.DrawNormals, gizmosOptions.DrawRotation, pointColor, transform);
			}
		}
#endif
	}
}