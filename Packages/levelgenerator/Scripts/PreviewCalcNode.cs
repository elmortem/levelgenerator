using XNode;

namespace LevelGenerator
{
	public abstract class PreviewCalcNode : PreviewNode
	{
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if(to.IsInput)
				CalcResults(true);
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if(port.IsInput)
				CalcResults(true);
		}

		protected override void ApplyChange()
		{
			CalcResults(true);
			base.ApplyChange();
		}
		
		protected abstract void CalcResults(bool force = false);
	}
}