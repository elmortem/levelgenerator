using UnityEngine;

namespace LevelGenerator.Utility
{
	public static class VectorUtility
	{
		public static Vector3 Mult(this Vector3 u, Vector3 v)
		{
			return new Vector3(u.x * v.x, u.y * v.y, u.z * v.z);
		}
	}
}