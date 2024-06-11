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
			{
				ApplyChange();
			}
		}
		
		private void ChildrenApplyChange()
		{
			foreach (var port in Ports)
			{
				if (port.IsOutput && port.IsConnected)
				{
					foreach (var connection in port.GetConnections())
					{
						if(connection.node is GenNode genNode)
							genNode.ApplyChange();
					}
				}
			}
		}
		
		public void RaiseChanged()
		{
			ApplyChange();
		}

		protected virtual void ApplyChange()
		{
			ChildrenApplyChange();
		}
#endif
	}
}