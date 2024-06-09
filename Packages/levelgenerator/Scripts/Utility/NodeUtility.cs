using System.Collections.Generic;
using XNode;

namespace LevelGenerator.Utility
{
	public static class NodeUtility
	{
		public static IEnumerable<T> GetNodeInChildren<T>(this Node node)
		{
			foreach (var port in node.Ports)
			{
				if (port.IsOutput && port.Connection != null)
				{
					if(port.Connection.node is T item)
						yield return item;
					
					foreach (var itemNode in port.Connection.node.GetNodeInChildren<T>())
					{
						yield return itemNode;
					}
				}
			}
		}
		
		public static IEnumerable<T> GetNodeInParent<T>(this Node node)
		{
			foreach (var port in node.Ports)
			{
				if (port.IsInput && port.Connection != null)
				{
					if (port.Connection.node is T item)
						yield return item;
					
					foreach (var itemNode in port.Connection.node.GetNodeInParent<T>())
					{
						yield return itemNode;
					}
				}
			}
		}
	}
}