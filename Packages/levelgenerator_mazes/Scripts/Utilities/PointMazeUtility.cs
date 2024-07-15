using UnityEngine;
using UnityEngine.Assertions;

namespace LevelGenerator.Mazes.Utilities
{
	public static class PointMazeUtility
	{
		public static bool InCircle(float x1, float y1, float x2, float y2, float x3, float y3, float px, float py)
		{
			// reduce the computational complexity by substracting the last row of the matrix
			// ref: https://www.cs.cmu.edu/~quake/robust.html
			float p1p_x = x1 - px;
			float p1p_y = y1 - py;

			float p2p_x = x2 - px;
			float p2p_y = y2 - py;

			float p3p_x = x3 - px;
			float p3p_y = y3 - py;

			double p1p = p1p_x * p1p_x + p1p_y * p1p_y;
			double p2p = p2p_x * p2p_x + p2p_y * p2p_y;
			double p3p = p3p_x * p3p_x + p3p_y * p3p_y;

			// determinant of matrix, see paper for the reference
			double res = p1p_x * (p2p_y * p3p - p2p * p3p_y)
			             - p1p_y * (p2p_x * p3p - p2p * p3p_x)
			             + p1p * (p2p_x * p3p_y - p2p_y * p3p_x);

			Assert.IsTrue(System.Math.Abs(res) < double.MaxValue / 100d);

			return res < 0f;
		}
	}
}