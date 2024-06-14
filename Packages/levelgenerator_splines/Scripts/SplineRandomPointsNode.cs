using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;
using UnityEngine.Splines;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Splines
{
	public class SplineRandomPointsNode : BasePointsNode
	{
		[Input] public SplineContainerData SplineContainer;
		public int Count = 100;
		public int Seed = -1;
		[Output] public List<Vector3> Points;

		private int _lastCount;
		private int _lastSeed = -1;
		
		public int PointsCount => Points?.Count ?? 0;

		private void Awake()
		{
			if(Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}

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
			if(LockCalc && Points != null)
				return;
			if (!force && _lastSeed == Seed && _lastCount == Count && Points != null)
				return;
			
			if(Points == null)
				Points = new();
			else
				Points.Clear();

			var splineContainer = GetInputValue(nameof(SplineContainer), SplineContainer);
			if (splineContainer == null || splineContainer.Splines.Count <= 0)
				return;

			_gizmosOptions = null;

			_lastCount = Count;
			_lastSeed = Seed;
			
			var lastState = Random.state;
			Random.InitState(_lastSeed);
			
			for (int i = 0; i < Count; i++)
			{
				var spline = splineContainer.Splines[Random.Range(0, splineContainer.Splines.Count)];
				spline.Evaluate(Random.value, out var point, out var tangent, out var upVector);
				Points.Add(point);
			}
			
			Random.state = lastState;
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