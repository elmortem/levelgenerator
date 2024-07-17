using LevelGenerator.Points;
using LevelGenerator.Splines.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Splines
{
	public class ChangeSplineNode : PreviewCalcNode
	{
		[Input(connectionType = ConnectionType.Override)] public SplineContainerData SplineContainer;
		public ChangePositionItem Position;
		public int Seed = -1;
		[Output] public SplineContainerData Result = new();

		private ChangePositionMode _lastMode;
		private Vector3 _lastMin;
		private Vector3 _lastMax;
		private int _lastSeed;
		private SplineContainerData _result;

		private void Awake()
		{
			if(Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Result))
			{
				CalcResults();
				return _result;
			}

			return null;
		}
		
		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(SplineContainer));
			if (port == null || !port.IsConnected)
			{
				_result = null;
				return;
			}
			
			if (LockCalc && _result != null)
				return;
			if(!force && _lastMode == Position.Mode && _lastMin == Position.Min && _lastMax == Position.Max && _lastSeed == Seed && _result != null)
				return;
			
			var splineContainer = GetInputValue(nameof(SplineContainer), SplineContainer);
			if (splineContainer == null || splineContainer.Splines.Count <= 0)
				return;

			if(_result == null)
				_result = new();
			else
				_result.Splines.Clear();
			
			ResetGizmosOptions();

			_lastMode = Position.Mode;
			_lastMin = Position.Min;
			_lastMax = Position.Max;
			_lastSeed = Seed;

			var state = Random.state;
			Random.InitState(Seed);
			
			foreach (var spline in splineContainer.Splines)
			{
				var positionValue = Position.Min == Position.Max 
					? (float3)Position.Min 
					: new float3(
						Random.Range(Position.Min.x, Position.Max.x),
						Random.Range(Position.Min.y, Position.Max.y), 
						Random.Range(Position.Min.z, Position.Max.z));
				
				var newSpline = new Spline();
				for (var i = 0; i < spline.Count; ++i)
				{
					var knot = spline[i];
					var pos = knot.Position;
					if (Position.Mode == ChangePositionMode.Add)
						pos += positionValue;
					else if(Position.Mode == ChangePositionMode.Mult)
						pos *= positionValue;
					var newKnot = new BezierKnot(pos , knot.TangentIn, knot.TangentOut, knot.Rotation);
					newSpline.Add(newKnot, spline.GetTangentMode(i));
				}
				_result.Splines.Add(newSpline);
			}

			Random.state = state;
		}
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultPort = GetOutputPort(nameof(Result));
			var result = (SplineContainerData)GetValue(resultPort);
			if(result == null)
				return;

			Gizmos.color = gizmosOptions.Color;
			SplinesGizmoUtility.DrawGizmos(result, transform);
		}
#endif
	}
}