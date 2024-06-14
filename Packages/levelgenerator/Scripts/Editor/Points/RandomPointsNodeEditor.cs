using LevelGenerator.Points;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace LevelGenerator.Editor.Points
{
	[CustomNodeEditor(typeof(RandomPointsNode))]
	public class RandomPointsNodeEditor : NodeEditor
	{
		private RandomPointsNode _node;
		
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();

			if(_node == null)
				_node = (RandomPointsNode)target;

			NodeEditorGUI.DrawResult("Results",_node.PointsCount);

			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}