using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Points
{
	[MovedFrom("LevelGenerator.Splines")]
	[Obsolete("Use SplineDistancePointsNode instead")]
	public class OldSplineDistancePointsNode : BasePointsNode
	{
		[Input] public SplineContainerData SplineContainer;
		public float Distance = 5f;
		[Output] public List<Vector3> Points;

		private float _lastDistance;
		
		public int PointsCount => Points?.Count ?? 0;
		
		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;
			
			if (port.fieldName == nameof(Points))
			{
				CalcResults();
				return Points;
			}
			
			return null;
		}


		protected override void CalcResults(bool force = false)
		{
			if (Distance < 0.0001f)
				return;
			
			if(LockCalc && Points != null)
				return;
			if (!force && Mathf.Approximately(_lastDistance, Distance) && Points != null)
				return;
			
			if(Points == null)
				Points = new();
			else
				Points.Clear();

			var splineContainer = GetInputValue(nameof(SplineContainer), SplineContainer);
			if (splineContainer == null || splineContainer.Splines.Count <= 0)
				return;

			_gizmosOptions = null;

			_lastDistance = Distance;
			
			foreach (var spline in splineContainer.Splines)
			{
				if(spline.Count <= 1)
					continue;
				
				var step = Distance / spline.GetLength();
				for (float i = 0; i <= 1f; i += step)
				{
					spline.Evaluate(i, out var point, out var tangent, out var upVector);
					Points.Add(point);
				}
			}
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var resultsPort = GetOutputPort(nameof(Points));
			var results = (List<Vector3>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;
			
			UpdateGizmosOptions();

			var pos = transform.position;
			
			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			var maxCount = Mathf.Min(10000, results.Count);
			for (int i = 0; i < maxCount; i++)
			{
				var point = results[i];
				Gizmos.DrawSphere(pos + point, _gizmosOptions?.PointSize ?? 0.2f);
			}
		}
#endif
	}
}