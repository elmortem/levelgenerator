using LevelGenerator.Instances;
using XNodeEditor;

namespace LevelGenerator.Editor.Instances
{
	[CustomNodeEditor(typeof(GameObjectsNode))]
	public class GameObjectsNodeEditor : NodeEditor
	{
		private GameObjectsNode _node;
		public override void OnBodyGUI()
		{
			if(_node == null)
				_node = (GameObjectsNode)target;
			
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawLock(_node);
		}
	}
}