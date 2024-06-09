using XNode;

namespace LevelGenerator
{
	public class GenNode : Node
	{
#if UNITY_EDITOR
		private void OnValidate()
		{
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