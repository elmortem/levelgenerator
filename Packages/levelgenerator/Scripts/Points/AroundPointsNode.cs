using System.Collections.Generic;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Points
{
	public class AroundPointsNode : PreviewCalcNode, IPointCount
	{
		[Input] public List<PointData> Points = new();
		public float RadiusMin = 0.5f;
		public float RadiusMax = 1f;
		public int CountMin = 1;
		public int CountMax = 5;
		public Vector3 AxesMult = new(1f, 0f, 1f);
		public int Seed = -1;
		[Output] public List<PointData> Results = new();
		
		private float _lastRadiusMin;
		private float _lastRadiusMax;
		private int _lastCountMin;
		private int _lastCountMax;
		private Vector3 _lastAxesMult;
		private int _lastSeed;
		private List<PointData> _results;
		
		public int PointsCount => _results?.Count ?? 0;

		private void Awake()
		{
			if (Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}

		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return _results ?? Results;
			}

			return null;
		}
		
		protected override void CalcResults(bool force = false)
		{
			if(LockCalc && _results != null)
				return;
			if (!force && Mathf.Approximately(_lastRadiusMin, RadiusMin) && Mathf.Approximately(_lastRadiusMax, RadiusMax) && _lastCountMin == CountMin && _lastCountMax == CountMax && _lastSeed == Seed && _results != null)
				return;

			if (_results == null)
				_results = new();
			else
				_results.Clear();

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;

			ResetGizmosOptions();

			var state = Random.state;
			_lastSeed = Seed;
			Random.InitState(_lastSeed);

			foreach (var points in pointsList)
			{
				for (int i = 0; i < points.Count; i++)
				{
					var count = Random.Range(CountMin, CountMax);
					for (int j = 0; j < count; j++)
					{
						var center = points[i].Position;
						var newPosition = center + Random.insideUnitSphere.Mult(AxesMult).normalized *
							Random.Range(RadiusMin, RadiusMax);
						_results.Add(new PointData
						{
							Position = newPosition,
							Normal = Vector3.up,
							Scale = Vector3.one,
							Angle = 0f
						});
					}
				}
			}

			Random.state = state;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<PointData>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, gizmosOptions, transform);
		}
#endif
	}
}