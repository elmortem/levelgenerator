using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGenerator.Surfaces.Datas
{
	[Serializable]
	public class TerrainSurfaceData : BaseSurfaceData
	{
		public TerrainData Terrain;
		public Vector3 Offset = Vector3.zero;
		[NodeEnum]
		public SurfaceNormalMode NormalNode;

		public override void GetPoints(List<PointData> points, GeneratePointMode mode, int count, int seed = 0)
		{
			if (Terrain == null)
			{
				Debug.LogWarning("Terrain is not assigned.");
				return;
			}

			if (count <= 0)
				return;

			switch (mode)
			{
				case GeneratePointMode.SurfaceRegular:
				case GeneratePointMode.VolumeRegular:
					GetRegularPoints(points, count);
					break;
				case GeneratePointMode.SurfaceRandom:
				case GeneratePointMode.VolumeRandom:
					GetRandomPoints(points, count, seed);
					break;
			}
		}
		
		public override void ProjectionPoints(List<PointData> points, ProjectionPointMode mode, List<PointData> results)
		{
		    var terrainData = Terrain;
		    var terrainSize = terrainData.size;
		    var heightmapWidth = terrainData.heightmapResolution;
		    var heightmapHeight = terrainData.heightmapResolution;
		    var heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

		    foreach (var point in points)
		    {
		        var newPoint = point;
		        int terrainX = 0, terrainZ = 0;
		        float height = 0f;

		        switch (mode)
		        {
		            case ProjectionPointMode.Normal:
		                var direction = point.Normal.normalized;
		                var projectedPoint = newPoint.Position + direction * (terrainSize.y - newPoint.Position.y) / direction.y;
		                terrainX = Mathf.Clamp((int)((projectedPoint.x - Offset.x) / terrainSize.x * heightmapWidth), 0, heightmapWidth - 1);
		                terrainZ = Mathf.Clamp((int)((projectedPoint.z - Offset.z) / terrainSize.z * heightmapHeight), 0, heightmapHeight - 1);
		                height = heights[terrainZ, terrainX] * terrainSize.y + Offset.y;
		                break;

		            case ProjectionPointMode.Surface:
		                terrainX = Mathf.Clamp((int)((point.Position.x - Offset.x) / terrainSize.x * heightmapWidth), 0, heightmapWidth - 1);
		                terrainZ = Mathf.Clamp((int)((point.Position.z - Offset.z) / terrainSize.z * heightmapHeight), 0, heightmapHeight - 1);
		                height = heights[terrainZ, terrainX] * terrainSize.y + Offset.y;
		                break;
		        }

		        if (terrainX >= 0 && terrainX < heightmapWidth && terrainZ >= 0 && terrainZ < heightmapHeight)
		        {
		            newPoint.Position = new Vector3(point.Position.x, height, point.Position.z);
		            var terrainNormal = terrainData.GetInterpolatedNormal((float)terrainX / heightmapWidth, (float)terrainZ / heightmapHeight);
		            newPoint.Normal = NormalUtility.GetNormal(NormalNode, newPoint.Position, Offset, point.Normal, terrainNormal);

		            results.Add(newPoint);
		        }
		    }
		}

		private void GetRegularPoints(List<PointData> points, int count)
		{
			var terrainData = Terrain;
			var terrainSize = terrainData.size;
			var heightmapWidth = terrainData.heightmapResolution;
			var heightmapHeight = terrainData.heightmapResolution;
			var heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

			var k = terrainSize.z / terrainSize.x;
			var kk = Mathf.Sqrt(count);

			var wc = Mathf.Max(1, Mathf.RoundToInt(kk * k));
			var hc = Mathf.Max(1, Mathf.RoundToInt(count / (float)wc));

			var wstep = terrainSize.x / wc;
			var hstep = terrainSize.z / hc;

			for (int i = 0; i <= wc; i++)
			{
				for (int j = 0; j <= hc; j++)
				{
					var terrainX = Mathf.Clamp((int)(i * wstep / terrainSize.x * heightmapWidth), 0, heightmapWidth - 1);
					var terrainZ = Mathf.Clamp((int)(j * hstep / terrainSize.z * heightmapHeight), 0, heightmapHeight - 1);
					var height = heights[terrainZ, terrainX] * terrainSize.y;

					var pos = new Vector3
					{
						x = i * wstep + Offset.x,
						y = height + Offset.y,
						z = j * hstep + Offset.z
					};

					var point = new PointData
					{
						Position = pos,
						Normal = terrainData.GetInterpolatedNormal((float)terrainX / heightmapWidth, (float)terrainZ / heightmapHeight),
						Scale = Vector3.one
					};
					points.Add(point);
				}
			}
		}

		private void GetRandomPoints(List<PointData> points, int count, int seed)
		{
			var terrainData = Terrain;
			var terrainSize = terrainData.size;
			var heightmapWidth = terrainData.heightmapResolution;
			var heightmapHeight = terrainData.heightmapResolution;
			var heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

			var state = Random.state;
			Random.InitState(seed);

			for (int i = 0; i < count; i++)
			{
				var terrainX = Mathf.Clamp(Random.Range(0, heightmapWidth), 0, heightmapWidth - 1);
				var terrainZ = Mathf.Clamp(Random.Range(0, heightmapHeight), 0, heightmapHeight - 1);
				var height = heights[terrainZ, terrainX] * terrainSize.y;

				var pos = new Vector3
				{
					x = terrainX * terrainSize.x / heightmapWidth + Offset.x,
					y = height + Offset.y,
					z = terrainZ * terrainSize.z / heightmapHeight + Offset.z
				};

				var point = new PointData
				{
					Position = pos,
					Normal = terrainData.GetInterpolatedNormal((float)terrainX / heightmapWidth, (float)terrainZ / heightmapHeight),
					Scale = Vector3.one
				};
				points.Add(point);
			}
			Random.state = state;
		}
	}
}