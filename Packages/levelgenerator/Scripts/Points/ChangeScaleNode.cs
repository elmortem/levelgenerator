using System;
using System.Collections.Generic;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Points
{
	public enum ChangeScaleMode
	{
		Add,
		Mult,
		Set
	}

	[Serializable]
	public class ChangeScaleItem
	{
		[NodeEnum]
		public ChangeScaleMode Mode = ChangeScaleMode.Set;
		[FormerlySerializedAs("ScaleMin")] public Vector3 Min = Vector3.one;
		[FormerlySerializedAs("ScaleMax")] public Vector3 Max = Vector3.one;
		[FormerlySerializedAs("LockScale")] public bool Lock = true;
	}
	
	public class ChangeScaleNode : BasePointsNode
	{
		[Input] public List<PointData> Points = new();
		[NodeEnum]
		public ChangeScaleMode Mode = ChangeScaleMode.Set;
		public Vector3 ScaleMin = Vector3.one;
		public Vector3 ScaleMax = Vector3.one;
		public bool LockScale = true;
		public int Seed = -1;
		[Output] public List<PointData> Results = new()                ;

		private ChangeScaleMode _lastMode;
		private Vector3 _lastScaleMin;
		private Vector3 _lastScaleMax;
		private bool _lastLockScale;
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
			if (!force && _lastMode == Mode && _lastScaleMin == ScaleMin && _lastScaleMax == ScaleMax && _lastSeed == Seed && _results != null)
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
			_lastLockScale = LockScale;
			_lastSeed = Seed;

			var state = Random.state;
			Random.InitState(Seed);
			
			var equalScale = ScaleMin == ScaleMax;
			
			foreach (var points in pointsList)
			{
				foreach (var point in points)
				{
					var newPoint = point;
					
					var rndX = Random.Range(ScaleMin.x, ScaleMax.x);
					var scale = equalScale 
						? ScaleMin 
						: LockScale 
							? new Vector3(rndX, rndX, rndX)
							: new Vector3(rndX, Random.Range(ScaleMin.y, ScaleMax.y), Random.Range(ScaleMin.z, ScaleMax.z));
					
					if(Mode == ChangeScaleMode.Add)
						newPoint.Scale += scale;
					else if(Mode == ChangeScaleMode.Mult)
						newPoint.Scale = newPoint.Scale.Mult(scale);
					else if (Mode == ChangeScaleMode.Set)
						newPoint.Scale = scale;
					
					_results.Add(newPoint);
				}
			}

			Random.state = state;
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (LockScale)
			{
				ScaleMin.y = ScaleMin.z = ScaleMin.x;
				ScaleMax.y = ScaleMax.z = ScaleMax.x;
			}
		}

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