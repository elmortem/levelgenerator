using System.Linq;
using LevelGenerator.Bounds.Datas;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace LevelGenerator.Bounds
{
	public class CombineBoundsNode : PreviewNode, IGizmosOptionsProvider
	{
		[Input] public BoundData Includes;
		[Input] public BoundData Excludes;
		[Output] public BoundData Result;

		private GizmosOptions _gizmosOptions;
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

			_gizmosOptions = null;
				
			if (_result == null)
				_result = new CombineBoundData();
			_result.Includes = GetInputValues(nameof(Includes), Includes).ToList();
			_result.Excludes = GetInputValues(nameof(Excludes), Excludes).ToList();
			_result.Calc();
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
			var result = GetInputValue(nameof(Result), Result);
			if (result == null)
				return;
			
			UpdateGizmosOptions();
			
			var center = result.Min + (result.Max - result.Min) * 0.5f;
			var size = result.Max - result.Min;
			
			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			Gizmos.DrawWireCube(transform.TransformPoint(center), size);
		}
#endif
	}
}