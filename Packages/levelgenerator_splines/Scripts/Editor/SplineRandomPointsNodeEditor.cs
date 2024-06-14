using LevelGenerator.Editor;
using XNodeEditor;

namespace LevelGenerator.Splines.Editor
{
	[CustomNodeEditor(typeof(SplineRandomPointsNode))]
	public class SplineRandomPointsNodeEditor : NodeEditor
	{
		private SplineRandomPointsNode _node;
		public override void OnBodyGUI()
		{
			if (_node == null)
				_node = (SplineRandomPointsNode)target;
			
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawResult("Results", _node.PointsCount);
			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}