using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Splines.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Points
{
	public class PointsToSplineNode : BasePointsNode
	{
		[Input] public List<PointData> Points;
		[Output] public SplineContainerData Result = new();

		private SplineContainerData _result;

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

			_gizmosOptions = null;
			
			if(_result == null)
				_result = new();
			else
				_result.Splines.Clear();
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length <= 0)
				return;

			foreach (var points in pointsList)
			{
				var spline = new Spline();
				foreach (var point in points)
				{
					var knot = new BezierKnot(point.Position, new float3(), new float3());
					spline.Add(knot, TangentMode.AutoSmooth);
				}
				_result.Splines.Add(spline);
			}
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();
			
			var resultPort = GetOutputPort(nameof(Result));
			var result = (SplineContainerData)GetValue(resultPort);
			if(result == null)
				return;

			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			SplinesGizmoUtility.DrawGizmos(result, transform);
		}
#endif
	}
}