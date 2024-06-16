using System.Collections.Generic;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public enum ChangeScaleMode
	{
		Add,
		Mult,
		Set
	}
	
	public class ChangeScaleNode : BasePointsNode
	{
		[Input] public List<PointData> Points;
		public ChangeScaleMode Mode = ChangeScaleMode.Set;
		public Vector3 ScaleMin = Vector3.one;
		public Vector3 ScaleMax = Vector3.one;
		public int Seed = -1;
		[Output] public List<PointData> Results;

		private ChangeScaleMode _lastMode;
		private Vector3 _lastScaleMin;
		private Vector3 _lastScaleMax;
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
			if (!force && _lastScaleMin == ScaleMin && _lastScaleMax == ScaleMax && _lastSeed == Seed && _results != null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();

			_gizmosOptions = null;
			
			_lastScaleMin = ScaleMin;
			_lastScaleMax = ScaleMax;
			_lastSeed = Seed;

			var state = Random.state;
			Random.InitState(Seed);
			
			var equalScale = ScaleMin == ScaleMax;
			
			foreach (var points in pointsList)
			{
				foreach (var point in points)
				{
					var scale = equalScale ? ScaleMin : Vector3.Lerp(ScaleMin, ScaleMax, Random.Range(0f, 1f));
					
					var newPoint = point;
					if(Mode == ChangeScaleMode.Add)
						newPoint.Scale += scale;
					else if(Mode == ChangeScaleMode.Mult)
						newPoint.Scale = newPoint.Scale.Mult(scale);
					else if (Mode == ChangeScaleMode.Set)
						newPoint.Scale = scale;
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