using System;
using System.Collections.Generic;
using LevelGenerator.Points;

namespace LevelGenerator.Surfaces.Datas
{
	public enum ProjectionPointMode
	{
		Normal,
		ToCenter,
		Surface
	}
	
	public enum SurfaceNormalMode
	{
		Default,
		Surface,
		Up,
		ToCenter,
		FromCenter
	}
	
	[Serializable]
	public class BaseSurfaceData
	{
		public virtual void GetPoints(List<PointData> points, SurfacePointMode mode, int count, int seed = 0)
		{
		}

		public virtual void ProjectionPoints(List<PointData> points, ProjectionPointMode mode, List<PointData> results)
		{
		}
	}
}