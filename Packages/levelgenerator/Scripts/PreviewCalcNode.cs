using XNode;

namespace LevelGenerator
{
	public abstract class PreviewCalcNode : PreviewNode
	{
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			base.OnCreateConnection(from, to);
			
			if(!LockCalc && to.IsInput && to.node == this)
				ApplyChange();
		}

		public override void OnRemoveConnection(NodePort port)
		{
			base.OnRemoveConnection(port);

			if (!LockCalc && port.IsInput && port.node == this)
				ApplyChange();
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