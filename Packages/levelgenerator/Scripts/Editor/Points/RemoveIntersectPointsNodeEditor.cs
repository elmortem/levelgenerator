using LevelGenerator.Points;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace LevelGenerator.Editor.Points
{
	[CustomNodeEditor(typeof(RemoveIntersectPointsNode))]
	public class RemoveIntersectPointsNodeEditor : NodeEditor
	{
		private RemoveIntersectPointsNode _node;
		
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();

			if(_node == null)
				_node = (RemoveIntersectPointsNode)target;

			NodeEditorGUI.DrawResult("Results",_node.PointsCount);

			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}