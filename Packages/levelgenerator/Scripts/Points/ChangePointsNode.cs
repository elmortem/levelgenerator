using System.Collections.Generic;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class ChangePointsNode : PreviewCalcNode
	{
		[Input] public List<PointData> Points = new();
		public int Seed = -1;
		public ChangePositionItem Position = new();
		public ChangeNormalItem Normal = new();
		public ChangeRotationItem Rotation = new();
		public ChangeScaleItem Scale = new();
		[Output] public List<PointData> Results = new();

		private int _lastSeed;
		
		private ChangePositionMode _lastPositionMode;
		private Vector3 _lastPositionMin;
		private Vector3 _lastPositionMax;
		
		private ChangeNormalMode _lastNormalMode;
		private Vector3 _lastNormalMin;
		private Vector3 _lastNormalMax;
		
		private ChangeRotationMode _lastRotationMode;
		private float _lastAngleMin;
		private float _lastAngleMax;
		
		private ChangeScaleMode _lastScaleMode;
		private Vector3 _lastScaleMin;
		private Vector3 _lastScaleMax;
		private bool _lastLockScale;
		
		private List<PointData> _results;

		private void Awake()
		{
			if(Seed == -1)
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
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			if (LockCalc && _results != null)
				return;
			if (!force && 
			    _lastPositionMode == Position.Mode && _lastPositionMin == Position.Min && _lastPositionMax == Position.Max && 
			    _lastNormalMode == Normal.Mode && _lastNormalMin == Normal.Min && _lastNormalMax == Normal.Max && 
			    _lastRotationMode == Rotation.Mode && Mathf.Approximately(_lastAngleMin, Rotation.Min) && Mathf.Approximately(_lastAngleMax, Rotation.Max) &&
			    _lastScaleMode == Scale.Mode && _lastScaleMin == Scale.Min && _lastScaleMax == Scale.Max && _lastLockScale == Scale.Lock &&
			    _lastSeed == Seed && _results != null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();

			ResetGizmosOptions();
			
			_lastSeed = Seed;
			
			_lastPositionMode = Position.Mode;
			_lastPositionMin = Position.Min;
			_lastPositionMax = Position.Max;
			
			_lastNormalMode = Normal.Mode;
			_lastNormalMin = Normal.Min;
			_lastNormalMax = Normal.Max;
			
			_lastRotationMode = Rotation.Mode;
			_lastAngleMin = Rotation.Min;
			_lastAngleMax = Rotation.Max;
			
			_lastScaleMode = Scale.Mode;
			_lastScaleMin = Scale.Min;
			_lastScaleMax = Scale.Max;
			_lastLockScale = Scale.Lock;

			var state = Random.state;
			Random.InitState(Seed);
			
			var equalPositions = Position.Min == Position.Max;
			var equalNormals = Normal.Min == Normal.Max;
			var equalAngle = Mathf.Approximately(Rotation.Min, Rotation.Max);
			var equalScale = Scale.Min == Scale.Max;
			
			foreach (var points in pointsList)
			{
				if(points == null)
					continue;
				
				foreach (var point in points)
				{
					var newPoint = point;
					
					//position
					var positionValue = equalPositions
						? Position.Min
						: new Vector3(
							Random.Range(Position.Min.x, Position.Max.x),
							Random.Range(Position.Min.y, Position.Max.y), 
							Random.Range(Position.Min.z, Position.Max.z));
					if(Position.Mode == ChangePositionMode.Add)
						newPoint.Position += positionValue;
					else if(Position.Mode == ChangePositionMode.Mult)
						newPoint.Position = newPoint.Position.Mult(positionValue);
					
					// normal
					var normal = equalNormals
						? Normal.Min
						: new Vector3(
							Random.Range(Normal.Min.x, Normal.Max.x),
							Random.Range(Normal.Min.y, Normal.Max.y), 
							Random.Range(Normal.Min.z, Normal.Max.z));
					if(Normal.Mode == ChangeNormalMode.Add)
						newPoint.Normal += normal;
					if(Normal.Mode == ChangeNormalMode.Mult)
						newPoint.Normal = newPoint.Normal.Mult(normal);
					else if (Normal.Mode == ChangeNormalMode.Set)
						newPoint.Normal = normal;
					newPoint.Normal.Normalize();
					
					// rotation
					var angle = equalAngle ? Rotation.Min : Random.Range(Rotation.Min, Rotation.Max);
					if(Rotation.Mode == ChangeRotationMode.Add)
						newPoint.Angle += angle;
					if(Rotation.Mode == ChangeRotationMode.Mult)
						newPoint.Angle *= angle;
					else if (Rotation.Mode == ChangeRotationMode.Set)
						newPoint.Angle = angle;
					
					// scale
					var rndX = Random.Range(Scale.Min.x, Scale.Max.x);
					var scale = equalScale 
						? Scale.Min 
						: Scale.Lock
							? new Vector3(rndX, rndX, rndX)
							: new Vector3(rndX, Random.Range(Scale.Min.y, Scale.Max.y), Random.Range(Scale.Min.z, Scale.Max.z));
					if(Scale.Mode == ChangeScaleMode.Add)
						newPoint.Scale += scale;
					else if(Scale.Mode == ChangeScaleMode.Mult)
						newPoint.Scale = newPoint.Scale.Mult(scale);
					else if (Scale.Mode == ChangeScaleMode.Set)
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

			if (Scale.Lock)
			{
				Scale.Min.y = Scale.Min.z = Scale.Min.x;
				Scale.Max.y = Scale.Max.z = Scale.Max.x;
			}
		}
		
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