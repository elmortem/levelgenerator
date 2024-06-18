using System;
using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Bounds.Datas;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Points
{
	[Obsolete]
	public class OldRandomPointsNode : PreviewNode, IGizmosOptionsProvider
	{
		[Input(connectionType:ConnectionType.Override, typeConstraint = TypeConstraint.Inherited)] public BoundData BoundData;
		[Output] public List<Vector3> Points;
		public int Count = 100;
		public int Seed = -1;
		public int MaxIterations = 1000000;

		private int _lastSeed = -1;
		private int _lastCount = -1;
		private List<Vector3> _points;
		private GizmosOptions _gizmosOptions;
		
		public int PointsCount => _points?.Count ?? 0;

		private void Awake()
		{
			if (Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}

		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if (to.IsInput)
			{
				CalcPoints(true);
			}
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if (port.IsInput)
			{
				CalcPoints(true);
			}
		}

		protected override void ApplyChange()
		{
			CalcPoints(true);
			base.ApplyChange();
		}

		public override object GetValue(NodePort port) 
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Points))
			{
				CalcPoints();
				return _points;
			}
			
			return null;
		}

		private void CalcPoints(bool force = false)
		{
			var port = GetInputPort(nameof(BoundData));
			if (port == null || !port.IsConnected)
			{
				_points = null;
				return;
			}
			
			if(LockCalc && _points != null)
				return;
			if(!force && _lastSeed == Seed && _lastCount == Count && _points != null)
				return;

			if (_points == null)
				_points = new();
			else
				_points.Clear();
			
			var bound = GetInputValue(nameof(BoundData), BoundData);
			if(bound == null)
				return;

			_gizmosOptions = null;

			_lastSeed = Seed;
			_lastCount = Count;
			
			var lastState = Random.state;
			Random.InitState(_lastSeed);

			var volumeMax = 0f;
			var boundsList = bound.GetBounds().ToList();
			foreach (var boundItem in boundsList)
			{
				volumeMax += boundItem.Volume;
			}

			foreach (var boundItem in boundsList)
			{
				int boundCount = Mathf.RoundToInt(Count * boundItem.Volume / volumeMax);
				CalcPoints(boundItem, boundCount);
			}

			Random.state = lastState;
		}

		private void CalcPoints(BoundData bound, int count)
		{
			var min = bound.Min;
			var max = bound.Max;

			var genCount = 0;
			var _tryCount = 0;
			while(genCount < count && _tryCount++ < MaxIterations)
			{
				var point = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y),
					Random.Range(min.z, max.z));
				if (bound.InBounds(point))
				{
					_points.Add(point);
					genCount++;
				}
			}
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
			
			var pointsPort = GetOutputPort(nameof(Points));
			var points = (List<Vector3>)GetValue(pointsPort);
			if (points == null || points.Count <= 0)
				return;

			var pos = transform.position;
			
			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			var maxCount = Mathf.Min(10000, points.Count);
			for (int i = 0; i < maxCount; i++)
			{
				var point = points[i];
				Gizmos.DrawSphere(pos + point, _gizmosOptions?.PointSize ?? 0.2f);
			}
		}
#endif
	}
}