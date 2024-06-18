using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGenerator.Surfaces.Datas
{
	public enum BoxSurfaceNormalMode
	{
		Default,
		Up,
		ToCenter,
		FromCenter
	}
	
	[Serializable]
	public class BoxSurfaceData : BaseSurfaceData
	{
		public Vector3 Offset = Vector3.zero;
		public Vector3 Size = new(100f, 100f, 100f);
		[NodeEnum]
		public BoxSurfaceNormalMode NormalMode = BoxSurfaceNormalMode.Up;
		
		public override void GetPoints(List<PointData> points, SurfacePointMode mode, int count, int seed = 0)
		{
			if(count <= 0)
				return;
			
			switch (mode)
			{
				case SurfacePointMode.SurfaceRegular:
					GetRegularSurfacePoints(points, count);
					break;
				case SurfacePointMode.VolumeRegular:
					GetRegularVolumePoints(points, count);
					break;
				case SurfacePointMode.SurfaceRandom:
					GetRandomSurfacePoints(points, count, seed);
					break;
				case SurfacePointMode.VolumeRandom:
					GetRandomVolumePoints(points, count, seed);
					break;
			}
		}

		public override void ProjectionPoints(List<PointData> points, ProjectionPointMode mode, List<PointData> results)
		{
			Debug.Log("Not supported yet.");
			results.AddRange(points);
		}

		private void GetRegularSurfacePoints(List<PointData> points, int count)
		{
			int pointsPerFace = Mathf.CeilToInt(count / 6.0f);
			int pointsPerAxis = Mathf.CeilToInt(Mathf.Sqrt(pointsPerFace));
    
			float stepX = Size.x / (pointsPerAxis - 1);
			float stepY = Size.y / (pointsPerAxis - 1);
			float stepZ = Size.z / (pointsPerAxis - 1);

			var half = Size / 2f;

			for (int i = 0; i < pointsPerAxis; i++)
			{
				for (int j = 0; j < pointsPerAxis; j++)
				{
					AddPoint(points, new Vector3(-half.x, -half.y + j * stepY, -half.z + i * stepZ), Vector3.left); // -X face
					AddPoint(points, new Vector3(half.x, -half.y + j * stepY, -half.z + i * stepZ), Vector3.right); // +X face
					AddPoint(points, new Vector3(-half.x + i * stepX, -half.y, -half.z + j * stepZ), Vector3.down); // -Y face
					AddPoint(points, new Vector3(-half.x + i * stepX, half.y, -half.z + j * stepZ), Vector3.up); // +Y face
					AddPoint(points, new Vector3(-half.x + i * stepX, -half.y + j * stepY, -half.z), Vector3.back); // -Z face
					AddPoint(points, new Vector3(-half.x + i * stepX, -half.y + j * stepY, half.z), Vector3.forward); // +Z face
				}
			}
		}

		private void GetRegularVolumePoints(List<PointData> points, int count)
		{
			int pointsPerAxis = Mathf.CeilToInt(Mathf.Pow(count, 1.0f / 3.0f));

			float stepX = Size.x / (pointsPerAxis - 1);
			float stepY = Size.y / (pointsPerAxis - 1);
			float stepZ = Size.z / (pointsPerAxis - 1);

			var half = Size / 2f;

			for (int i = 0; i < pointsPerAxis; i++)
			{
				for (int j = 0; j < pointsPerAxis; j++)
				{
					for (int k = 0; k < pointsPerAxis; k++)
					{
						var pos = new Vector3
						{
							x = -half.x + i * stepX + Offset.x,
							y = -half.y + j * stepY + Offset.y,
							z = -half.z + k * stepZ + Offset.z
						};

						var point = new PointData
						{
							Position = pos,
							Normal = GetNormal(pos, Vector3.up),
							Scale = Vector3.one
						};
						points.Add(point);
					}
				}
			}
		}

		private void GetRandomSurfacePoints(List<PointData> points, int count, int seed)
		{
			var state = Random.state;
			Random.InitState(seed);

			var half = Size / 2f;

			for (int i = 0; i < count; i++)
			{
				Vector3 pos = Vector3.zero;
				Vector3 normal = Vector3.zero;
				int face = Random.Range(0, 6);

				switch (face)
				{
					case 0: // -X face
						pos = new Vector3(-half.x, Random.Range(-half.y, half.y), Random.Range(-half.z, half.z));
						normal = Vector3.left;
						break;
					case 1: // +X face
						pos = new Vector3(half.x, Random.Range(-half.y, half.y), Random.Range(-half.z, half.z));
						normal = Vector3.right;
						break;
					case 2: // -Y face
						pos = new Vector3(Random.Range(-half.x, half.x), -half.y, Random.Range(-half.z, half.z));
						normal = Vector3.down;
						break;
					case 3: // +Y face
						pos = new Vector3(Random.Range(-half.x, half.x), half.y, Random.Range(-half.z, half.z));
						normal = Vector3.up;
						break;
					case 4: // -Z face
						pos = new Vector3(Random.Range(-half.x, half.x), Random.Range(-half.y, half.y), -half.z);
						normal = Vector3.back;
						break;
					case 5: // +Z face
						pos = new Vector3(Random.Range(-half.x, half.x), Random.Range(-half.y, half.y), half.z);
						normal = Vector3.forward;
						break;
				}

				var point = new PointData
				{
					Position = pos + Offset,
					Normal = GetNormal(pos, normal),
					Scale = Vector3.one
				};
				points.Add(point);
			}
			
			Random.state = state;
		}

		private void GetRandomVolumePoints(List<PointData> points, int count, int seed)
		{
			var state = Random.state;
			Random.InitState(seed);
			
			var half = Size / 2f;
			
			for (int i = 0; i < count; i++)
			{
				var pos = new Vector3
				{
					x = -half.x + Random.Range(0f, Size.x) + Offset.x,
					y = -half.y + Random.Range(0f, Size.y) + Offset.y,
					z = -half.z + Random.Range(0f, Size.z) + Offset.z
				};
				
				var point = new PointData
				{
					Position = pos,
					Normal = GetNormal(pos, Vector3.up),
					Scale = Vector3.one
				};
				points.Add(point);
			}
			Random.state = state;
		}
		
		private void AddPoint(List<PointData> points, Vector3 pos, Vector3 normal)
		{
			var point = new PointData
			{
				Position = pos + Offset,
				Normal = GetNormal(pos, normal),
				Scale = Vector3.one
			};
			points.Add(point);
		}
		
		private Vector3 GetNormal(Vector3 position, Vector3 defaultNormal)
		{
			switch (NormalMode)
			{
				case BoxSurfaceNormalMode.Default:
					return defaultNormal;
				case BoxSurfaceNormalMode.ToCenter:
					return (Offset - position).normalized;
				case BoxSurfaceNormalMode.FromCenter:
					return (position - Offset).normalized;
				case BoxSurfaceNormalMode.Up:
				default:
					return Vector3.up;
			}
		}
	}
}