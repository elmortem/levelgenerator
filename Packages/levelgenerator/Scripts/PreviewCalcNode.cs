using XNode;

namespace LevelGenerator
{
	public abstract class PreviewCalcNode : PreviewNode
	{
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if(to.IsInput && !LockCalc)
				CalcResults(true);
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if(port.IsInput && !LockCalc)
				CalcResults(true);
		}

		protected override void ApplyChange()
		{
			if (!LockCalc)
			{
				CalcResults(true);
				base.ApplyChange();
			}
		}
		
		protected abstract void CalcResults(bool force = false);
	}
}