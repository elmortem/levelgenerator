using System.Linq;
using LevelGenerator.Bounds.Datas;
using UnityEngine.Serialization;
using XNode;

namespace LevelGenerator.Bounds
{
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
			if (port.fieldName == nameof(Result))
			{
				CalcResult();
				
				return _result;
			}

			return null;
		}
		
		private void CalcResult(bool force = false)
		{
			if (!force && _result != null)
				return;
				
			if (_result == null)
				_result = new CombineBoundData();
			_result.Includes = GetInputValues(nameof(Includes), Includes).ToList();
			_result.Excludes = GetInputValues(nameof(Excludes), Excludes).ToList();
			_result.Calc();
		}
	}
}