using UnityEngine;

namespace LevelGenerator.Utility
{
	public static class VectorUtility
	{
		public static Vector3 Mult(this Vector3 u, Vector3 v)
		{
			return new Vector3(u.x * v.x, u.y * v.y, u.z * v.z);
		}
		
		public static Vector3 SwapYZ(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.y);
		}
		
		public static float Avarage(this Vector3 v)
		{
			return (v.x + v.y + v.z) / 3f;
		}
	}
}