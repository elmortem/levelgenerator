using System;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Points;
using LevelGenerator.Splines.Utilities;
using LevelGenerator.Utility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Splines
{
	public class RandomSplineNode : PreviewCalcNode
	{
		public Vector3 StartPoint = new(-50f, 0f, -50f);
		public Vector3 FinishPoint = new(50f, 0f, 50f);
		public Vector3 Up = new(0f, 1f, 0f);
		public int Segments = 10;
		public float HeightMin = 3f;
		public float HeightMax = 5f;
		public int Seed = -1;
		
		[Output] public SplineContainerData Result;

		private SplineContainerData _result;
		private Vector3 _lastStartPoint;
		private Vector3 _lastFinishPoint;
		private Vector3 _lastUp;
		private int _lastSegments;
		private float _lastHeightMin;
		private float _lastHeightMax;
		private int _lastSeed;

		private void Awake()
		{
			if (Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}

		public override object GetValue(NodePort port)
		{
			if(port == null)
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
			if(LockCalc && _result != null)
				return;
			if(!force 
			   && _lastStartPoint == StartPoint 
			   && _lastFinishPoint == FinishPoint 
			   && _lastUp == Up 
			   && _lastSegments == Segments 
			   && Mathf.Approximately(_lastHeightMin, HeightMin) 
			   && Mathf.Approximately(_lastHeightMax, HeightMax)
			   && _lastSeed == Seed 
			   && _result != null)
				return;

			Spline spline;
			if (_result == null)
			{
				_result = new SplineContainerData();
			}

			if (_result.Splines.Count <= 0)
			{
				spline = new Spline
				{
					Closed = false
				};
				_result.Splines.Add(spline);
			}
			else
			{
				spline = _result.Splines[0];
			}
			spline.Clear();

			var state = Random.state;
			Random.InitState(Seed);

			spline.Add(new BezierKnot(StartPoint, float3.zero, float3.zero), TangentMode.AutoSmooth);
			var dist = Vector3.Distance(StartPoint, FinishPoint);
			var step = dist / Segments;
			var direction = (FinishPoint - StartPoint).normalized;
			var perpDirection = Vector3.Cross(direction, Up).normalized;
			for (int i = 1; i < Segments; i++)
			{
				var point = StartPoint + direction * (step * i);
				
				var randomDistance = Random.Range(HeightMin, HeightMax);
				var randomOffset = perpDirection * randomDistance;
				if (Random.value > 0.5f)
				{
					randomOffset = -randomOffset;
				}

				point += randomOffset;
				
				spline.Add(new BezierKnot(point, float3.zero, float3.zero), TangentMode.AutoSmooth);
			}
			
			spline.Add(new BezierKnot(FinishPoint, float3.zero, float3.zero), TangentMode.AutoSmooth);

			Random.state = state;
			
			_lastStartPoint = StartPoint;
			_lastFinishPoint = FinishPoint;
			_lastUp = Up;
			_lastSegments = Segments;
			_lastHeightMin = HeightMin;
			_lastHeightMax = HeightMax;
			_lastSeed = Seed;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Result));
			var result = (SplineContainerData)GetValue(resultsPort);
			if(result == null)
				return;
				
			Gizmos.color = gizmosOptions.Color;
			SplinesGizmoUtility.DrawGizmos(result, transform);
			GizmosUtility.DrawPoint(StartPoint, gizmosOptions.PointSize, transform, gizmosOptions);
			GizmosUtility.DrawPoint(FinishPoint, gizmosOptions.PointSize, transform, gizmosOptions);
		}
#endif
	}
}