using System.Collections.Generic;
using LevelGenerator.Points;

namespace LevelGenerator.Surfaces.Datas
{
	public abstract class BaseSurfaceData
	{
		public abstract void GetPoints(List<PointData> points, SurfacePointMode mode, int count, int seed = 0);
	}
}