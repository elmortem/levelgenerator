using LevelGenerator.Bounds.Datas;
using UnityEngine;
using XNode;

namespace LevelGenerator.Bounds
{
	public class SphereBoundsNode : PreviewNode
	{
		public Vector3 Offset;
		public Vector3 Scale = Vector3.one;
		public float Radius;
		[Output] public BoundData Result;
		
		private SphereBoundData _result;

		public override object GetValue(NodePort port)
		{
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
			if(!force && _result != null)
				return;
			
			if(_result == null)
				_result = new SphereBoundData();
			
			_result.Offset = Offset;
			_result.Radius = Radius;
			_result.Scale = Scale;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(transform.position + Offset, Radius);
		}
#endif
	}
}