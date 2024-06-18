using LevelGenerator.Editor;
using LevelGenerator.Splines.Points;
using XNodeEditor;

namespace LevelGenerator.Splines.Editor.Points
{
	[CustomNodeEditor(typeof(OldSplineDistancePointsNode))]
	public class OldSplineDistancePointsNodeEditor : NodeEditor
	{
		private OldSplineDistancePointsNode _node;
		
		public override void OnBodyGUI()
		{
			if (_node == null)
				_node = (OldSplineDistancePointsNode)target;
			
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawResult("Results", _node.PointsCount);
			NodeEditorGUI.DrawPreviewAndLock(_node);
		}
	}
}