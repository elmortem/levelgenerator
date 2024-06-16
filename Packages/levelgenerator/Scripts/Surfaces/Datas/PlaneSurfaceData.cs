using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGenerator.Surfaces.Datas
{
	[Serializable]
	public class PlaneSurfaceData : BaseSurfaceData
	{
		public Vector3 Up = Vector3.up;
		public Vector3 Offset = Vector3.zero;
		public Vector2 Size = new Vector2(100, 100);
		
		public override void GetPoints(List<PointData> points, SurfacePointMode mode, int count, int seed = 0)
		{
			if(count <= 0)
				return;
			
			switch (mode)
			{
				case SurfacePointMode.SurfaceRegular:
				case SurfacePointMode.VolumeRegular:
					GetRegularPoints(points, count);
					break;
				case SurfacePointMode.SurfaceRandom:
				case SurfacePointMode.VolumeRandom:
					GetRandomPoints(points, count, seed);
					break;
			}
		}
		
		private void GetRegularPoints(List<PointData> points, int count)
		{
			var k = Size.y / Size.x;

			var kk = Mathf.Sqrt(count);
			
			var wc = Mathf.Max(1, Mathf.RoundToInt(kk * k));
			var hc = Mathf.Max(1, Mathf.RoundToInt(count / (float)wc));

			var half = Size / 2f;
			var wstep = Size.x / wc;
			var hstep = Size.y / hc;
			
			var rot = Quaternion.FromToRotation(Vector3.up, Up);
			
			for (int i = 0; i <= wc; i++)
			{
				for (int j = 0; j <= hc; j++)
				{
					var pos = new Vector3
					{
						x = -half.x + i * wstep + Offset.x,
						y = 0f,
						z = -half.y + j * hstep + Offset.y
					};

					var point = new PointData
					{
						Position = rot * pos,
						Normal = Up.normalized,
						Scale = Vector3.one
					};
					points.Add(point);
				}
			}
		}
		
		private void GetRandomPoints(List<PointData> points, int count, int seed)
		{
			var state = Random.state;
			Random.InitState(seed);
			
			var half = Size / 2f;
			var rot = Quaternion.FromToRotation(Vector3.up, Up);
			
			for (int i = 0; i < count; i++)
			{
				var pos = new Vector3
				{
					x = -half.x + Random.Range(0, Size.x) + Offset.x,
					y = 0f,
					z = -half.y + Random.Range(0, Size.y) + Offset.y
				};
				
				var point = new PointData
				{
					Position = rot * pos,
					Normal = Up,
					Scale = Vector3.one
				};
				points.Add(point);
			}
			Random.state = state;
		}
	}
}