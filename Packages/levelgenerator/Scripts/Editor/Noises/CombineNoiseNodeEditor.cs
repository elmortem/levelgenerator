using LevelGenerator.Noises;
using XNodeEditor;

namespace LevelGenerator.Editor.Noises
{
	[CustomNodeEditor(typeof(CombineNoiseNode))]
	public class CombinePointsNodeEditor : NodeEditor
	{
		private CombineNoiseNode _node;
		public override void OnBodyGUI()
		{
			if(_node == null)
				_node = (CombineNoiseNode)target;
			
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawLock(_node);
		}
	}
}