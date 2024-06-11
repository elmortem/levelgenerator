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

			var color = GUI.color;
			GUI.color = Color.blue;
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Results: " + _node.PointsCount);
			GUILayout.EndHorizontal();
			
			GUI.color = color;
			
			EditorGUILayout.BeginHorizontal();
			_node.ShowPreview = EditorGUILayout.Toggle( _node.ShowPreview, GUILayout.Width(12), GUILayout.Height(12));
			EditorGUI.BeginChangeCheck();
			_node.LockCalc = EditorGUILayout.Toggle(_node.LockCalc, GUILayout.Width(12), GUILayout.Height(12));
			if (EditorGUI.EndChangeCheck())
			{
				_node.RaiseChanged();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
}