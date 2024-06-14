using UnityEngine;
using XNodeEditor;

namespace LevelGenerator.Editor
{
	[CustomNodeEditor(typeof(ResultNode))]
	public class ResultNodeEditor : NodeEditor
	{
		private ResultNode _node;
		
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();
			
			if(_node == null)
				_node = (ResultNode)target;
			
			if(GUILayout.Button("Generate"))
			{
				_node.Generate();
			}
			if(GUILayout.Button("Clear"))
			{
				_node.Clear();
			}
			
			NodeEditorGUI.DrawResult("Objects",_node.ObjectsCount);
		}
	}
}