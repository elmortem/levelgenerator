using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Points
{
	public class SplineOffsetPointsNode : PreviewCalcNode, INodePointCount
	{
		[Input] public SplineContainerData SplineContainer;
		[FormerlySerializedAs("OffsetDistance")] public float Offset = 5f;
		public float PointDistance = 2f;
		public bool BothSides;
		public bool UpNormal;
		public bool NoRotation;
		[Output] public List<PointData> Points = new();

		private float _lastOffsetDistance;
		private float _lastPointDistance;
		private bool _lastBothSides;
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
			if (Mathf.Abs(Offset) < 0.0001f || PointDistance < 0.0001f)
				return;

			var port = GetInputPort(nameof(SplineContainer));
			if (port == null || !port.IsConnected)
			{
				_points = null;
				return;
			}

			if (LockCalc && _points != null)
				return;
			if (!force && Mathf.Approximately(_lastOffsetDistance, Offset) &&
			    Mathf.Approximately(_lastPointDistance, PointDistance) && _lastBothSides == BothSides && _lastUpNormal == UpNormal && _lastNoRotation == NoRotation && _points != null)
				return;
			
			var splineContainer = GetInputValue(nameof(SplineContainer), SplineContainer);
			if (splineContainer == null || splineContainer.Splines.Count <= 0)
			{
				_points = null;
				return;
			}

			if (_points == null)
				_points = new();
			else
				_points.Clear();

			ResetGizmosOptions();

			_lastOffsetDistance = Offset;
			_lastPointDistance = PointDistance;
			_lastBothSides = BothSides;
			_lastUpNormal = UpNormal;
			_lastNoRotation = NoRotation;

			foreach (var spline in splineContainer.Splines)
			{
				if (spline.Count <= 1)
					continue;

				var length = spline.GetLength();
				var step = PointDistance / length;
				for (float t = 0; t <= 1f; t += step)
				{
					spline.Evaluate(t, out var point, out var tangent, out var upVector);

					// Вычисляем точки с отступом
					var offsetDirection = math.normalize(math.cross(tangent, upVector));

					if (BothSides)
					{
						AddPoint(point + offsetDirection * Mathf.Abs(Offset), tangent, upVector);
						AddPoint(point - offsetDirection * Mathf.Abs(Offset), tangent, upVector);
					}
					else
					{
						var offsetPoint = point + offsetDirection * Offset;
						AddPoint(offsetPoint, tangent, upVector);
					}
				}
			}
		}
		
		private void AddPoint(Vector3 position, Vector3 tangent, Vector3 upVector)
		{
			_points.Add(new PointData
			{
				Position = position,
				Normal = UpNormal ? Vector3.up : upVector,
				Scale = Vector3.one,
				Angle = NoRotation ? 0f : Quaternion.LookRotation(tangent, upVector).eulerAngles.y
			});
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();

			var resultsPort = GetOutputPort(nameof(Points));
			var results = (List<PointData>)GetValue(resultsPort);
			if (results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, gizmosOptions, transform);
		}
#endif
	}
}