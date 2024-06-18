using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.Splines;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Splines.Points
{
	public class SplineRandomPointsNode : BasePointsNode
	{
		[Input] public SplineContainerData SplineContainer;
		public int Count = 100;
		public int Seed = -1;
		public bool UpNormal;
		public bool NoRotation;
		[Output] public List<PointData> Points;

		private int _lastCount;
		private int _lastSeed = -1;
		private List<PointData> _points;
		
		public int PointsCount => _points?.Count ?? 0;

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
				return _points;
			}
			
			return null;
		}


		protected override void CalcResults(bool force = false)
		{
			if(LockCalc && _points != null)
				return;
			if (!force && _lastSeed == Seed && _lastCount == Count && _points != null)
				return;
			
			if(_points == null)
				_points = new();
			else
				_points.Clear();

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
				_points.Add(new PointData
				{
					Position = point,
					Normal = UpNormal ? Vector3.up :  upVector,
					Scale = Vector3.one,
					Angle = NoRotation ? 0f : Quaternion.LookRotation(tangent, upVector).eulerAngles.y
				});
			}
			
			Random.state = lastState;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Points));
			var results = (List<PointData>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, _gizmosOptions, transform);
		}
#endif
	}
}