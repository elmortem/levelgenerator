using XNode;

namespace LevelGenerator
{
	public class GenNode : Node
	{
#if UNITY_EDITOR
		protected virtual bool IsChanged() => true;
		
		protected virtual void OnValidate()
		{
			if (IsChanged())
				ApplyChange();
		}
		
		private void ChildrenApplyChange()
		{
			foreach (var port in Ports)
			{
				if (port.IsOutput && port.Connection != null)
				{
					if(port.Connection.node is GenNode genNode)
						genNode.ApplyChange();
				}
			}
		}

		protected virtual void ApplyChange()
		{
			ChildrenApplyChange();
		}
#endif
	}
}