using System;
using System.Linq;
using LevelGenerator.Bounds.Datas;
using UnityEngine;
using XNode;

namespace LevelGenerator.Bounds
{
	[Obsolete]
	public class CombineBoundsNode : PreviewNode
	{
		[Input] public BoundData Includes;
		[Input] public BoundData Excludes;
		[Output] public BoundData Result;

		private CombineBoundData _result;

		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if(to.IsInput)
				CalcResult(true);
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if(port.IsInput)
				CalcResult(true);
		}

		protected override void ApplyChange()
		{
			CalcResult(true);
			base.ApplyChange();
		}

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
		
		private void CalcResult(bool force = false)
		{
			if(LockCalc && _result != null)
				return;
			if (!force && _result != null)
				return;

			ResetGizmosOptions();
				
			if (_result == null)
				_result = new CombineBoundData();
			_result.Includes = GetInputValues(nameof(Includes), Includes).ToList();
			_result.Excludes = GetInputValues(nameof(Excludes), Excludes).ToList();
			_result.Calc();
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var result = GetInputValue(nameof(Result), Result);
			if (result == null)
				return;
			
			var gizmosOptions = GetGizmosOptions();
			
			var center = result.Min + (result.Max - result.Min) * 0.5f;
			var size = result.Max - result.Min;
			
			Gizmos.color = gizmosOptions.Color;
			Gizmos.DrawWireCube(transform.TransformPoint(center), size);
		}
#endif
	}
}