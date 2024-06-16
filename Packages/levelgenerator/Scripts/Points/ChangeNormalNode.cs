using System.Collections.Generic;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public enum ChangeNormalMode
	{
		Add,
		Mult,
		Set
	}
	
	public class ChangeNormalNode : BasePointsNode
	{
		[Input] public List<PointData> Points;
		public ChangeNormalMode Mode = ChangeNormalMode.Set;
		public Vector3 NormalMin = Vector3.up;
		public Vector3 NormalMax = Vector3.up;
		public int Seed = -1;
		[Output] public List<PointData> Results;

		private ChangeNormalMode _lastMode;
		private Vector3 _lastNormalMin;
		private Vector3 _lastNormalMax;
		private int _lastSeed;
		private List<PointData> _results;

		private void Awake()
		{
			if(Seed == -1)
				Seed = Random.Range(0, int.MaxValue);
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
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			if (LockCalc && _results != null)
				return;
			if (!force && _lastMode == Mode && _lastNormalMin == NormalMin && _lastNormalMax == NormalMax && _lastSeed == Seed && _results != null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();

			_gizmosOptions = null;
			
			_lastNormalMin = NormalMin;
			_lastNormalMax = NormalMax;
			_lastSeed = Seed;

			var state = Random.state;
			Random.InitState(Seed);
			
			var equalNormals = NormalMin == NormalMax;
			
			foreach (var points in pointsList)
			{
				foreach (var point in points)
				{
					var normal = equalNormals ? NormalMin : Vector3.Lerp(NormalMin, NormalMax, Random.Range(0f, 1f));
					
					var newPoint = point;
					if(Mode == ChangeNormalMode.Add)
						newPoint.Normal += normal;
					if(Mode == ChangeNormalMode.Mult)
						newPoint.Normal = newPoint.Normal.Mult(normal);
					else if (Mode == ChangeNormalMode.Set)
						newPoint.Normal = normal;
					
					newPoint.Normal.Normalize();
					_results.Add(point);
				}
			}

			Random.state = state;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<PointData>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;
			
			UpdateGizmosOptions();

			GizmosUtility.DrawPoints(results, _gizmosOptions?.PointSize ?? 0.2f, transform, _gizmosOptions);
		}
#endif
	}
}