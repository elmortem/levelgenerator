using LevelGenerator.Points;
using XNodeEditor;

namespace LevelGenerator.Editor.Points
{
	[CustomNodeEditor(typeof(SplitPointsByNoiseNode))]
	public class SplitPointsByNoiseNodeEditor : NodeEditor
	{
		private SplitPointsByNoiseNode _node;
		
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();

			if(_node == null)
				_node = (SplitPointsByNoiseNode)target;

			NodeEditorGUI.DrawResult("Results",_node.PointsCount);

			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}