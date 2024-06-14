using LevelGenerator.Points;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace LevelGenerator.Editor.Points
{
	[CustomNodeEditor(typeof(PercentPointsNode))]
	public class PercentPointsNodeEditor : NodeEditor
	{
		private PercentPointsNode _node;
		
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();

			if(_node == null)
				_node = (PercentPointsNode)target;

			NodeEditorGUI.DrawResult("Results",_node.PointsCount);

			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}