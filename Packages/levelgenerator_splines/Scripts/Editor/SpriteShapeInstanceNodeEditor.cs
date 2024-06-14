using LevelGenerator.Editor;
using XNodeEditor;

namespace LevelGenerator.Splines.Editor
{
	[CustomNodeEditor(typeof(SpriteShapeInstanceNode))]
	public class SpriteShapeInstanceNodeEditor : NodeEditor
	{
		public override void OnBodyGUI()
		{
			base.OnBodyGUI();
			
			NodeEditorGUI.DrawPreviewAndLock((SpriteShapeInstanceNode)target);
		}
	}
}