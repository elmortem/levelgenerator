using System;
using System.Collections.Generic;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Points
{
	public enum ChangePositionMode
	{
		Add,
		Mult
	}

	[Serializable]
	public class ChangePositionItem
	{
		public ChangePositionMode Mode = ChangePositionMode.Add;
		public Vector3 Min = new(-1f, -1f, -1f);
		public Vector3 Max = Vector3.one;
	}
	
	public class ChangePositionNode : BasePointsNode
	{
		[Input] public List<PointData> Points = new();
		public ChangePositionMode Mode = ChangePositionMode.Add;
		public Vector3 Min = new(-1f, -1f, -1f);
		public Vector3 Max = Vector3.one;
		public int Seed = -1;
		[Output] public List<PointData> Results = new();

		private ChangePositionMode _lastMode;
		private Vector3 _lastMin;
		private Vector3 _lastMax;
		private int _lastSeed = -1;
		private List<PointData> _results;

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
			if (!force && _lastMode == Mode && _lastMin == Min && _lastMax == Max && _lastSeed == Seed && _results != null)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();
			
			var points = GetInputValue(nameof(Points), Points);
			if(points == null)
				return;

			_gizmosOptions = null;
			
			_lastMode = Mode;
			_lastMin = Min;
			_lastMax = Max;
			_lastSeed = Seed;
			
			var lastState = Random.state;
			Random.InitState(_lastSeed);

			foreach (var point in points)
			{
				var p = point;
				if(Mode == ChangePositionMode.Add)
					p.Position += new Vector3(Random.Range(Min.x, Max.x), Random.Range(Min.y, Max.y), Random.Range(Min.z, Max.z));
				else if(Mode == ChangePositionMode.Mult)
					p.Position = p.Position.Mult(new Vector3(Random.Range(Min.x, Max.x), Random.Range(Min.y, Max.y), Random.Range(Min.z, Max.z)));
				_results.Add(p);
			}
			
			Random.state = lastState;
		}
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<PointData>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, _gizmosOptions, transform);
		}
#endif
	}
}