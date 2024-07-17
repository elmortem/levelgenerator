using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Points
{
	public class SplineDistancePointsNode : PreviewCalcNode, INodePointCount
	{
		[Input] public SplineContainerData SplineContainer;
		public float Distance = 5f;
		public bool UpNormal;
		public bool NoRotation;
		[Output] public List<PointData> Points = new();

		private float _lastDistance;
		private bool _lastUpNormal;
		private bool _lastNoRotation;
		private List<PointData> _points;

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
			if (Distance < 0.0001f)
				return;
			
			var port = GetInputPort(nameof(SplineContainer));
			if (port == null || !port.IsConnected)
			{
				_points = null;
				return;
			}
			
			if(LockCalc && _points != null)
				return;
			if (!force && Mathf.Approximately(_lastDistance, Distance) && _lastUpNormal == UpNormal && _lastNoRotation == NoRotation && _points != null)
				return;
			
			if(_points == null)
				_points = new();
			else
				_points.Clear();

			var splineContainer = GetInputValue(nameof(SplineContainer), SplineContainer);
			if (splineContainer == null || splineContainer.Splines.Count <= 0)
				return;

			ResetGizmosOptions();

			_lastDistance = Distance;
			_lastUpNormal = UpNormal;
			_lastNoRotation = NoRotation;
			
			foreach (var spline in splineContainer.Splines)
			{
				if(spline.Count <= 1)
					continue;
				
				var step = Distance / spline.GetLength();
				for (float i = 0; i <= 1f; i += step)
				{
					spline.Evaluate(i, out var point, out var tangent, out var upVector);
					_points.Add(new PointData
					{
						Position = point,
						Normal = UpNormal ? Vector3.up :  upVector,
						Scale = Vector3.one,
						Angle = NoRotation ? 0f : Quaternion.LookRotation(tangent, upVector).eulerAngles.y
					});
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