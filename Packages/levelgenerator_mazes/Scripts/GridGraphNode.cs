using System.Collections.Generic;
using LevelGenerator.Mazes.Graphs;
using LevelGenerator.Mazes.Utilities;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Mazes
{
	public class GridGraphNode :  PreviewCalcNode, IGizmosOptionsProvider
	{
		public int Width = 10;
		public int Height = 10;
		public float CellSize = 1f;
		[Header("Gizmos")]
		public GizmosOptions GizmosOptions = new();
		public bool ShowCenterPoints;
		[Output] public Graph Result;
		[Output] public List<PointData> CenterPoints;
		
		private int _lastWidth;
		private int _lastHeight;
		private float _lastCellSize = -1;
		private Graph _result;
		private List<PointData> _centerPoints;
		
		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Result))
			{
				CalcResults();
				return _result;
			}
			if (port.fieldName == nameof(CenterPoints))
			{
				CalcResults();
				return _centerPoints;
			}

			return null;
		}
		
		protected override void CalcResults(bool force = false)
		{
			if (LockCalc && _result != null && _centerPoints != null)
				return;
			if (!force && _result != null && _centerPoints != null && Mathf.Approximately(_lastCellSize, CellSize) && _lastWidth == Width && _lastHeight == Height)
				return;
			
			_lastCellSize = CellSize;
			_lastWidth = Width;
			_lastHeight = Height;

			if (_result == null)
				_result = new Graph();
			else
				_result.Clear();
			
			GraphBuilder.BuildGrid(_result, _lastWidth, _lastHeight, _lastCellSize);
			
			// center points
			if (_centerPoints == null)
				_centerPoints = new();
			else
				_centerPoints.Clear();
			
			var halfX = _lastWidth * _lastCellSize * 0.5f;
			var halfY = _lastHeight * _lastCellSize * 0.5f;

			for (int x = -1; x < _lastWidth; x++)
			{
				for (int y = -1; y < _lastHeight; y++)
				{
					float centerX = (x + 0.5f) * _lastCellSize - halfX;
					float centerY = (y + 0.5f) * _lastCellSize - halfY;
					_centerPoints.Add(new PointData
						{ Position = new Vector3(centerX, 0f, centerY), Normal = Vector3.up, Scale = Vector3.one });
				}
			}
		}

		public GizmosOptions GetGizmosOptions() => GizmosOptions;
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var graphPort = GetOutputPort(nameof(Result));
			var result = (Graph)GetValue(graphPort);
			if (result == null)
				return;
			
			var centerPointsPort = GetOutputPort(nameof(CenterPoints));
			var centerPoints = (List<PointData>)GetValue(centerPointsPort);
			if (centerPoints == null || centerPoints.Count <= 0)
				return;
			
			Gizmos.color = GizmosOptions.Color;
			Gizmos.matrix = transform.localToWorldMatrix;
			
			GraphGizmos.DrawGraph(result);
			
			Gizmos.matrix = Matrix4x4.identity;
			
			if (ShowCenterPoints)
			{
				GizmosUtility.DrawPoints(centerPoints, GizmosOptions.PointSize * 2f, false, false, Color.cyan, transform);
			}
		}
#endif
	}
}