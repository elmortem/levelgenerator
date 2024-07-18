using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Splines.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Points
{
	public enum PointsToSplineMode
	{
		Sequence,
		Distance
	}
	
	public class PointsToSplineNode : PreviewCalcNode
	{
		[Input] public List<PointData> Points;
		[NodeEnum]
		public PointsToSplineMode Mode;
		[Output] public SplineContainerData Result = new();

		private SplineContainerData _result;
		private PointsToSplineMode _lastMode;

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Result))
			{
				CalcResults();
				return _result;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_result = null;
				return;
			}
			
			if (LockCalc && _result != null)
				return;
			if(!force && _lastMode == Mode && _result != null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length <= 0)
			{
				_result = null;
				return;
			}

			ResetGizmosOptions();
			_lastMode = Mode;
			
			if(_result == null)
				_result = new();
			else
				_result.Splines.Clear();

			if (Mode == PointsToSplineMode.Sequence)
			{
				CreateBySequence(pointsList);
			}
			else if (Mode == PointsToSplineMode.Distance)
			{
				CreateByDistance(pointsList);
			}
		}
		
		private void CreateBySequence(List<PointData>[] pointsList)
		{
			foreach (var points in pointsList)
			{
				if(points == null || points.Count <= 0)
					continue;
				
				var spline = new Spline();
				foreach (var point in points)
				{
					var knot = new BezierKnot(point.Position, new float3(), new float3());
					spline.Add(knot, TangentMode.AutoSmooth);
				}
				_result.Splines.Add(spline);
			}
		}

		private void CreateByDistance(List<PointData>[] pointsList)
		{
			var allPoints = new List<PointData>();

			// Собираем все точки из входных данных
			foreach (var points in pointsList)
			{
				if (points == null || points.Count <= 0)
					continue;

				allPoints.AddRange(points);
			}

			if (allPoints.Count <= 0)
				return;

			var spline = new Spline();

			var currentPoint = allPoints[0];
			allPoints.RemoveAt(0);
			spline.Add(new BezierKnot(currentPoint.Position, new float3(), new float3()), TangentMode.AutoSmooth);

			while (allPoints.Count > 0)
			{
				PointData nextPoint = default;
				float closestDistance = float.MaxValue;

				// Ищем ближайшую точку
				foreach (var point in allPoints)
				{
					float distance = Vector3.Distance(currentPoint.Position, point.Position);
					if (distance < closestDistance)
					{
						closestDistance = distance;
						nextPoint = point;
					}
				}

				if (closestDistance >= float.MaxValue)
					break;
				
				spline.Add(new BezierKnot(nextPoint.Position, new float3(), new float3()), TangentMode.AutoSmooth);
				allPoints.Remove(nextPoint);
				currentPoint = nextPoint;
			}

			_result.Splines.Add(spline);
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultPort = GetOutputPort(nameof(Result));
			var result = (SplineContainerData)GetValue(resultPort);
			if(result == null)
				return;

			Gizmos.color = gizmosOptions.Color;
			SplinesGizmoUtility.DrawGizmos(result, transform);
		}
#endif
	}
}