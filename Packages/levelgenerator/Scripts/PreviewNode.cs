using UnityEngine;

namespace LevelGenerator
{
	public abstract class PreviewNode : GenNode
	{
		public bool ShowPreview = true;
		//TODO support Editor for preview

#if UNITY_EDITOR
		public virtual void DrawGizmos(Transform transform)
		{
		}
#endif
	}
}