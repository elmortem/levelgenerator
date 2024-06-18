using UnityEditor;
using UnityEngine;

namespace LevelGenerator.Editor
{
	public static class NodeEditorGUI
	{
		public static void DrawPreviewAndLock(PreviewNode node)
		{
			EditorGUILayout.BeginHorizontal();
			node.ShowPreview = EditorGUILayout.Toggle(node.ShowPreview, GUILayout.Width(12), GUILayout.Height(12));
			EditorGUI.BeginChangeCheck();
			node.LockCalc = EditorGUILayout.Toggle(node.LockCalc, GUILayout.Width(12), GUILayout.Height(12));
			if (EditorGUI.EndChangeCheck())
			{
				node.RaiseChanged();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		
		public static void DrawLock(PreviewNode node)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			node.LockCalc = EditorGUILayout.Toggle(node.LockCalc, GUILayout.Width(12), GUILayout.Height(12));
			if (EditorGUI.EndChangeCheck())
			{
				node.RaiseChanged();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		public static void DrawResult(string title, object value)
		{
			var color = GUI.color;
			GUI.color = Color.blue;
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label($"{title}: {value}");
			GUILayout.EndHorizontal();
			
			GUI.color = color;
		}
	}
}