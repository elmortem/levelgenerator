using LevelGenerator.Points;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace LevelGenerator.Editor.Points
{
	[CustomNodeEditor(typeof(AroundPointsNode))]
	public class AroundPointsNodeEditor : NodeEditor
	{
		private AroundPointsNode _node;
		
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();

			if(_node == null)
				_node = (AroundPointsNode)target;

			NodeEditorGUI.DrawResult("Results",_node.PointsCount);

			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}