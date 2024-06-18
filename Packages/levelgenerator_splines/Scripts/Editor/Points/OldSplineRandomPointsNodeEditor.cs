using LevelGenerator.Editor;
using LevelGenerator.Splines.Points;
using XNodeEditor;

namespace LevelGenerator.Splines.Editor.Points
{
	[CustomNodeEditor(typeof(OldSplineRandomPointsNode))]
	public class OldSplineRandomPointsNodeEditor : NodeEditor
	{
		private OldSplineRandomPointsNode _node;
		public override void OnBodyGUI()
		{
			if (_node == null)
				_node = (OldSplineRandomPointsNode)target;
			
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawResult("Results", _node.PointsCount);
			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}