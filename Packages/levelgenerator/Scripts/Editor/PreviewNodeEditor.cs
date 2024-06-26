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

			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}