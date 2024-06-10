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

			var color = GUI.color;
			GUI.color = Color.blue;
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Results: " + _node.PointsCount);
			GUILayout.EndHorizontal();
			
			GUI.color = color;
			
			EditorGUILayout.BeginHorizontal();
			_node.ShowPreview = EditorGUILayout.Toggle( _node.ShowPreview, GUILayout.Width(12), GUILayout.Height(12));
			_node.LockCalc = EditorGUILayout.Toggle(_node.LockCalc, GUILayout.Width(12), GUILayout.Height(12));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
}