using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Points;
using LevelGenerator.Splines.Utilities;
using LevelGenerator.Utility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Points
{
	public class PointsGridInsideSplineNode : PreviewCalcNode, INodeInfo, INodePointCount
	{
		[Input] public SplineContainerData SplineContainer;
		public float CellWidth = 100;
		public float CellHeight = 100;
		[Output] public List<PointData> Points;

		private float _lastCellWidth;
		private float _lastCellHeight;
		private SplineContainerData _splineContainerCache;
		private List<PointData> _points;

		public bool HasNodeInfo()
		{
			return _splineContainerCache == null || _splineContainerCache.Splines.Count <= 0 ||!_splineContainerCache.Splines.Any(p => p.Closed);
		}

		public string GetNodeInfo()
		{
			return "No one closed spline.";
		}
		
		public int PointsCount => _points?.Count ?? 0;

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;
			
			if (port.fieldName == nameof(Points))
			{
				CalcResults();
				return _points;
			}
			
			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(SplineContainer));
			if (port == null || !port.IsConnected)
			{
				_points = null;
				return;
			}
			
			if (LockCalc && _points != null)
				return;
			if (!force && Mathf.Approximately(_lastCellWidth, CellWidth) && Mathf.Approximately(_lastCellHeight, CellHeight) && _points != null)
				return;

			_splineContainerCache = null;
			var splineContainer = GetInputValue(nameof(SplineContainer), SplineContainer);
			if (splineContainer == null || splineContainer.Splines.Count <= 0)
				return;

			_splineContainerCache = splineContainer;
			
			ResetGizmosOptions();
			
			_lastCellWidth = CellWidth;
			_lastCellHeight = CellHeight;
			
			if (_points == null)
				_points = new();
			else
				_points.Clear();
			
			foreach (var spline in splineContainer.Splines)
			{
				if(!spline.Closed)
					continue;

				float3 splineUp = float3.zero;
				for (var i = 0; i < spline.Count; i++)
				{
					splineUp += spline.GetCurveUpVector(i, 0f);
				}
				
				var bounds = spline.GetBounds();

				var widthCount = Mathf.FloorToInt(bounds.size.x / CellWidth);
				var heightCount = Mathf.FloorToInt(bounds.size.z / CellHeight);
				
				for (int i = 0; i < widthCount; i++)
				{
					for (int j = 0; j < heightCount; j++)
					{
						var point = new Vector3(bounds.min.x + i * CellWidth + CellWidth * 0.5f, bounds.center.y,
							bounds.min.z + j * CellHeight + CellHeight * 0.5f);
						if (spline.IsInsideSpline(point))
						{
							_points.Add(new PointData { Position = point, Normal = splineUp, Scale = Vector3.one });
						}
					}
				}
			}
		}
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Points));
			var results = (List<PointData>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, gizmosOptions, transform);
		}
#endif
	}
}