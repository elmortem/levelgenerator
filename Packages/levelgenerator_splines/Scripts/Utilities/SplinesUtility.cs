
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace LevelGenerator.Splines.Utilities
{
	public static class SplinesUtility
	{
		public static bool IsInsideSpline(this Spline spline, Vector3 point)
		{
			var pos = (float3)point;
			SplineUtility.GetNearestPoint(spline, pos, out var splinePoint, out var t);
			spline.Evaluate(t, out _, out var tangent, out _);
 
			var cross = math.cross(math.up(), math.normalize(tangent));
			return math.dot(splinePoint - pos, cross) > 0;
		}
	}
}