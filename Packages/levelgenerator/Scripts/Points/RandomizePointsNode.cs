using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class RandomizePointsNode : BasePointsNode
	{
		[Input] public List<Vector3> Points;
		public Vector3 Min = new Vector3(-1f, -1f, -1f);
		public Vector3 Max = Vector3.one;
		public int Seed = -1;
		[Output] public List<Vector3> Results;

		private int _lastSeed = -1;

		private void Awake()
		{
			if (Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return Results;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				Results = null;
				return;
			}
			
			if (LockCalc && Results != null)
				return;
			if (!force && _lastSeed == Seed && Results != null)
				return;
			
			if (Results == null)
				Results = new();
			else
				Results.Clear();
			
			var points = GetInputValue(nameof(Points), Points);
			if(points == null)
				return;

			_gizmosOptions = null;
			
			_lastSeed = Seed;
			
			var lastState = Random.state;
			Random.InitState(_lastSeed);

			foreach (var point in points)
			{
				var p = point + new Vector3(Random.Range(Min.x, Max.x), Random.Range(Min.y, Max.y), Random.Range(Min.z, Max.z));
				Results.Add(p);
			}
			
			Random.state = lastState;
		}
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<Vector3>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;
			
			UpdateGizmosOptions();
			
			DrawPoints(results, _gizmosOptions?.PointSize ?? 0.2f, transform, _gizmosOptions);
		}
#endif
	}
}