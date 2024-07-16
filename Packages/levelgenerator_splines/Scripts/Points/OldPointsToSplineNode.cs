using System;
using System.Collections.Generic;
using LevelGenerator.Splines.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Points
{
	[Obsolete("Use PointsToSplineNode instead")]
	public class OldPointsToSplineNode : PreviewCalcNode
	{
		[Input] public List<Vector3> Points;
		[Output] public SplineContainerData Result;

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Result))
			{
				CalcResults();
				return Result;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				Result = null;
				return;
			}
			
			if (LockCalc && Result != null)
				return;

			ResetGizmosOptions();
			
			if(Result == null)
				Result = new();
			else
				Result.Splines.Clear();
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length <= 0)
				return;

			foreach (var points in pointsList)
			{
				var spline = new Spline();
				foreach (var point in points)
				{
					spline.Add(new BezierKnot(point, new float3(), new float3()), TangentMode.AutoSmooth);
				}
				Result.Splines.Add(spline);
			}
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var resultsPort = GetOutputPort(nameof(Result));
			var results = (SplineContainerData)GetValue(resultsPort);
			if(results == null)
				return;
			
			var gizmosOptions = GetGizmosOptions();
			
			Gizmos.color = gizmosOptions.Color;
			SplinesGizmoUtility.DrawGizmos(results, transform);
		}
#endif
	}
}