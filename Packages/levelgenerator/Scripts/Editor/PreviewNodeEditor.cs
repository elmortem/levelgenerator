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

			EditorGUILayout.BeginHorizontal();
			//new Rect(12, 12, 12, 12)
			_node.ShowPreview = EditorGUILayout.Toggle( _node.ShowPreview, GUILayout.Width(12), GUILayout.Height(12));
			//new Rect(12 + 12 + 6, 12, 12, 12)
			EditorGUI.BeginChangeCheck();
			_node.LockCalc = EditorGUILayout.Toggle(_node.LockCalc, GUILayout.Width(12), GUILayout.Height(12));
			if (EditorGUI.EndChangeCheck())
			{
				if(!_node.LockCalc)
					_node.RaiseChanged();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
}