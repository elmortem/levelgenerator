using LevelGenerator.Surfaces.Datas;
using UnityEngine;

namespace LevelGenerator.Utility
{
	public static class NormalUtility
	{
		public static Vector3 GetNormal(MeshSurfaceNormalMode mode, Vector3 position, Vector3 offset, Vector3 defaultNormal)
		{
			switch (mode)
			{
				case MeshSurfaceNormalMode.Default:
					return defaultNormal;
				case MeshSurfaceNormalMode.ToCenter:
					return (offset - position).normalized;
				case MeshSurfaceNormalMode.FromCenter:
					return (position - offset).normalized;
				case MeshSurfaceNormalMode.Up:
				default:
					return Vector3.up;
			}
		}
	}
}