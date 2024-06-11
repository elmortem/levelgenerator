using System;
using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Points
{
	public class AroundPointsNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		[Input] public List<Vector3> Points = new();
		public float RadiusMin = 0.5f;
		public float RadiusMax = 1f;
		public int CountMin = 1;
		public int CountMax = 5;
		public int Seed = -1;
		public Vector3 AxesMult = new Vector3(1f, 0f, 1f);
		[Output] public List<Vector3> Results = new();

		private int _lastSeed = -1;
		private List<Vector3> _results;
		private GizmosOptions _gizmosOptions;
		
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
				return _results;
			}

			return null;
		}
		
		protected override void CalcResults(bool force = false)
		{
			if(LockCalc && _results != null)
				return;
			if (!force && _lastSeed == Seed && _results != null)
				return;

			if (_results == null)
				_results = new();
			else
				_results.Clear();

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;

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
						var center = points[i];
						var point = center + Random.insideUnitSphere.Mult(AxesMult).normalized *
							Random.Range(RadiusMin, RadiusMax);
						_results.Add(point);
					}
				}
			}

			Random.state = state;
		}
		
		private void UpdateGizmosOptions()
		{
			if (_gizmosOptions == null)
			{
				foreach (var provider in this.GetNodeInParent<IGizmosOptionsProvider>())
				{
					_gizmosOptions = provider.GetGizmosOptions();
					break;
				}
			}
		}
		
		public GizmosOptions GetGizmosOptions()
		{
			UpdateGizmosOptions();
			return _gizmosOptions;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<Vector3>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

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