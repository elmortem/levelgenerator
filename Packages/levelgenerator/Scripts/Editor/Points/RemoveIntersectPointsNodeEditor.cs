using LevelGenerator.Points;
using UnityEngine;
using XNodeEditor;

namespace LevelGenerator.Editor.Points
{
	[NodeEditor.CustomNodeEditor(typeof(RemoveIntersectPointsNode))]
	public class RemoveIntersectPointsNodeEditor : NodeEditor
	{
		private RemoveIntersectPointsNode _node;
		
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();

			if(_node == null)
				_node = (RemoveIntersectPointsNode)target;

			var color = GUI.color;
			GUI.color = Color.blue;
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Results: " + _node.PointsCount);
			GUILayout.EndHorizontal();
			
			GUI.color = color;
		}
	}
}