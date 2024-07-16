using System;
using System.Collections.Generic;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Points
{
	public enum ChangeRotationMode
	{
		Add,
		Mult,
		Set
	}

	[Serializable]
	public class ChangeRotationItem
	{
		[NodeEnum]
		public ChangeRotationMode Mode = ChangeRotationMode.Set;
		[FormerlySerializedAs("AngleMin")] public float Min = 0f;
		[FormerlySerializedAs("AngleMax")] public float Max = 360f;
	}
	
	public class ChangeRotationNode : PreviewCalcNode
	{
		[Input] public List<PointData> Points = new();
		[NodeEnum]
		public ChangeRotationMode Mode = ChangeRotationMode.Set;
		public float AngleMin = 0f;
		public float AngleMax = 360f;
		public int Seed = -1;
		[Output] public List<PointData> Results = new();

		private ChangeRotationMode _lastMode;
		private float _lastAngleMin;
		private float _lastAngleMax;
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
				return _results ?? Results;
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
			if (!force && _lastMode == Mode && Mathf.Approximately(_lastAngleMin, AngleMin) && Mathf.Approximately(_lastAngleMax, AngleMax) && _lastSeed == Seed && _results != null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();

			ResetGizmosOptions();
			
			_lastMode = Mode;
			_lastAngleMin = AngleMin;
			_lastAngleMax = AngleMax;
			_lastSeed = Seed;

			var state = Random.state;
			Random.InitState(Seed);
			
			var equalAngle = Mathf.Approximately(AngleMin, AngleMax);
			
			foreach (var points in pointsList)
			{
				foreach (var point in points)
				{
					var newPoint = point;
					
					var angle = equalAngle ? AngleMin : Random.Range(AngleMin, AngleMax);
					if(Mode == ChangeRotationMode.Add)
						newPoint.Angle += angle;
					if(Mode == ChangeRotationMode.Mult)
						newPoint.Angle *= angle;
					else if (Mode == ChangeRotationMode.Set)
						newPoint.Angle = angle;
					
					_results.Add(newPoint);
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