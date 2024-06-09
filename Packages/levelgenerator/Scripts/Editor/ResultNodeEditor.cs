using UnityEngine;
using XNodeEditor;

namespace LevelGenerator.Editor
{
	[CustomNodeEditor(typeof(ResultNode))]
	public class ResultNodeEditor : NodeEditor
	{
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();
			
			if(GUILayout.Button("Generate"))
			{
				(target as ResultNode)?.Generate();
			}
			if(GUILayout.Button("Clear"))
			{
				(target as ResultNode)?.Clear();
			}
		}
	}
}