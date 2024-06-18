using LevelGenerator.Editor;
using LevelGenerator.Splines.Points;
using XNodeEditor;

namespace LevelGenerator.Splines.Editor.Points
{
	[CustomNodeEditor(typeof(PointsNearSplineNode))]
	public class PointsNearSplineNodeEditor : NodeEditor
	{
		private PointsNearSplineNode _node;
		
		public override void OnBodyGUI()
		{
			if (_node == null)
				_node = (PointsNearSplineNode)target;
			
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawResult("Results", _node.PointsCount);
			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}