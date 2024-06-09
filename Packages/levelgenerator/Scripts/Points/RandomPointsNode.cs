using System.Collections.Generic;
using LevelGenerator.Bounds.Datas;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Points
{
	public class RandomPointsNode : PreviewNode, IGizmosOptionsProvider
	{
		[Input(connectionType:ConnectionType.Override, typeConstraint = TypeConstraint.Inherited)] public BoundData BoundData;
		[Output] public List<Vector3> Points;
		public int Count = 100;
		public int Seed = -1;
		public int MaxIterations = 1000000;

		private int _lastSeed = -1;
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
			if (port.fieldName == nameof(Points))
			{
				CalcPoints();
				return _points;
			}
			
			return null;
		}

		private void CalcPoints(bool force = false)
		{
			if(!force && _lastSeed == Seed && _points != null && _points.Count == Count)
				return;

			if (_points == null)
				_points = new();
			else
				_points.Clear();
			
			var bounds = GetInputValue(nameof(BoundData), BoundData);
			if(bounds == null)
				return;

			_gizmosOptions = null;

			_lastSeed = Seed;
			var lastState = Random.state;
			Random.InitState(_lastSeed);
			var min = bounds.Min;
			var max = bounds.Max;
			
			var _tryCount = 0;
			while(_points.Count < Count && _tryCount++ < MaxIterations)
			{
				var point = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y),
					Random.Range(min.z, max.z));
				if(bounds.InBounds(point))
					_points.Add(point);
			}
			Random.state = lastState;
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

			var pos = transform.position;
			
			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			foreach (var point in points)
			{
				Gizmos.DrawSphere(pos + point, 0.2f);
			}
		}
#endif
	}
}