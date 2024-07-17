using LevelGenerator.Points;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace LevelGenerator.Editor
{
	[CustomNodeEditor(typeof(PreviewNode))]
	public class PreviewNodeEditor : NodeEditor
	{
		private readonly GUIContent _showPreviewContent = new("Preview", "Show Preview");
		private readonly GUIContent _lockCalcContent = new("Lock", "Lock Calc (cache)");
		
		private PreviewNode _node;

		public override void OnBodyGUI()
		{
			base.OnBodyGUI();

			if (_node == null)
				_node = (PreviewNode)target;

			if (_node is INodePointCount pointCount)
			{
				NodeEditorGUI.DrawResult("Points", pointCount.PointsCount);
			}
			
			if (_node is INodeInfo nodeInfo && nodeInfo.HasNodeInfo())
			{
				NodeEditorGUI.DrawInfo(nodeInfo.GetNodeInfo());
			}
			
			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}