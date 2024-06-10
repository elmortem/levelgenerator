using UnityEngine;

namespace LevelGenerator
{
	public abstract class PreviewNode : GenNode
	{
		[HideInInspector]
		public bool ShowPreview = true;
		[HideInInspector]
		public bool LockCalc;
		//TODO support Editor for preview

#if UNITY_EDITOR
		public virtual void DrawGizmos(Transform transform)
		{
		}
#endif
	}
}