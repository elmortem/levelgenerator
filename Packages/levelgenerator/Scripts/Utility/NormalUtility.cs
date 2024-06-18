using LevelGenerator.Surfaces.Datas;
using UnityEngine;

namespace LevelGenerator.Utility
{
	public static class NormalUtility
	{
		public static Vector3 GetNormal(SurfaceNormalMode mode, Vector3 position, Vector3 offset, Vector3 defaultNormal, Vector3 surfaceNormal)
		{
			switch (mode)
			{
				case SurfaceNormalMode.Default:
					return defaultNormal;
				case SurfaceNormalMode.Surface:
					return surfaceNormal;
				case SurfaceNormalMode.ToCenter:
					return (offset - position).normalized;
				case SurfaceNormalMode.FromCenter:
					return (position - offset).normalized;
				case SurfaceNormalMode.Up:
				default:
					return Vector3.up;
			}
		}
	}
}