using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;

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
		public virtual void GetPoints(List<PointData> points, GeneratePointMode mode, int count, int seed = 0)
		{
		}

		public virtual void ProjectionPoints(List<PointData> points, ProjectionPointMode mode, List<PointData> results)
		{
		}
		
		public virtual bool Inside(PointData point)
		{
			return false;
		}

		public virtual void DrawGizmos(Transform transform)
		{
		}
	}
}