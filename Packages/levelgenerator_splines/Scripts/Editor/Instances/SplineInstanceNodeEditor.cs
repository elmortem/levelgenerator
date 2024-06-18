using LevelGenerator.Editor;
using XNodeEditor;

namespace LevelGenerator.Splines.Editor.Instances
{
	public abstract class SplineInstanceNodeEditor<T> : NodeEditor where T : PreviewNode
	{
		private T _node;
		public override void OnBodyGUI()
		{
			if(_node == null)
				_node = (T)target;
			
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawLock(_node);
		}
	}
}