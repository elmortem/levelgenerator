using LevelGenerator.Bounds.Datas;
using LevelGenerator.NodeGizmos;
using UnityEngine;
using XNode;

namespace LevelGenerator.Bounds
{
	public class SphereBoundsNode : PreviewNode, IGizmosOptionsProvider
	{
		public Vector3 Offset;
		public Vector3 Scale = Vector3.one;
		public float Radius;
		[Header("Gizmos")] 
		public GizmosOptions GizmosOptions;
		[Output] public BoundData Result;
		
		private SphereBoundData _result;
		
		public GizmosOptions GetGizmosOptions() => GizmosOptions;

		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Result))
			{
				CalcResult();
				return _result;
			}
			
			return null;
		}

		protected override void ApplyChange()
		{
			CalcResult(true);
			base.ApplyChange();
		}

		private void CalcResult(bool force = false)
		{
			if(LockCalc && _result != null)
				return;
			if(!force && _result != null)
				return;
			
			if(_result == null)
				_result = new SphereBoundData();
			
			_result.Offset = Offset;
			_result.Radius = Radius;
			_result.Scale = Scale;
			_result.Calc();
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			Gizmos.color = GizmosOptions.Color;
			Gizmos.DrawWireSphere(transform.position + Offset, Radius);
		}
#endif
	}
}