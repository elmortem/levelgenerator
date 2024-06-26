using LevelGenerator.Editor;
using LevelGenerator.Splines.Points;
using XNodeEditor;

namespace LevelGenerator.Splines.Editor.Points
{
	[CustomNodeEditor(typeof(SplineDistancePointsNode))]
	public class SplineDistancePointsNodeEditor : NodeEditor
	{
		private SplineDistancePointsNode _node;
		
		public override void OnBodyGUI()
		{
			if (_node == null)
				_node = (SplineDistancePointsNode)target;
			
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawResult("Results", _node.PointsCount);
			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}