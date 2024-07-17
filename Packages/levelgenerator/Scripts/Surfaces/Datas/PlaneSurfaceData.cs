using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGenerator.Surfaces.Datas
{
	[Serializable]
	public class PlaneSurfaceData : BaseSurfaceData
	{
		public Vector3 Offset = Vector3.zero;
		public Vector3 Up = Vector3.up;
		public Vector2 Size = new(100, 100);
		[NodeEnum]
		public SurfaceNormalMode NormalNode;
		
		public override void GetPoints(List<PointData> points, GeneratePointMode mode, int count, int seed = 0)
		{
			if(count <= 0)
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
			if (points == null || points.Count <= 0)
				return;
			
			switch (mode)
			{
				case ProjectionPointMode.Normal:
					ProjectionPointsNormal(points, results);
					break;
				case ProjectionPointMode.ToCenter:
					ProjectionPointsToCenter(points, results);
					break;
				case ProjectionPointMode.Surface:
					ProjectionPointsSurface(points, results);
					break;
			}
		}

		public override bool Inside(PointData point)
		{
			var halfSize = Size / 2f;
			var inverseRotation = Quaternion.Inverse(Quaternion.FromToRotation(Vector3.up, Up));
			var localPoint = inverseRotation * (point.Position - Offset);

			return localPoint.x >= -halfSize.x && localPoint.x <= halfSize.x &&
			       localPoint.z >= -halfSize.y && localPoint.z <= halfSize.y;/* &&
			       Mathf.Approximately(localPoint.y, 0);*/
		}

		private void ProjectionPointsNormal(List<PointData> points, List<PointData> results)
		{
			foreach (var point in points)
			{
				var direction = point.Normal.normalized;
				var inverseRotation = Quaternion.Inverse(Quaternion.FromToRotation(Vector3.up, Up));
				var localPoint = inverseRotation * (point.Position - Offset);

				if (localPoint.x >= -Size.x / 2 && localPoint.x <= Size.x / 2 &&
				    localPoint.z >= -Size.y / 2 && localPoint.z <= Size.y / 2)
				{
					localPoint.y = 0;

					var worldPoint = Quaternion.FromToRotation(Vector3.up, Up) * localPoint + Offset;

					// Apply projection along the normal
					var projectedPoint = worldPoint + direction * localPoint.y;

					var p = new PointData
					{
						Position = projectedPoint,
						Normal = point.Normal,
						Scale = point.Scale
					};
					results.Add(p);
				}
			}
		}

		private void ProjectionPointsToCenter(List<PointData> points, List<PointData> results)
		{
			Debug.LogWarning("Not need supported for Plane.");
		}
		
		private void ProjectionPointsSurface(List<PointData> points, List<PointData> results)
		{
			foreach (var point in points)
			{
				var newPoint = point;
				var localPoint = Quaternion.Inverse(Quaternion.FromToRotation(Vector3.up, Up)) * (point.Position - Offset);

				if (localPoint.x >= -Size.x / 2 && localPoint.x <= Size.x / 2 &&
				    localPoint.z >= -Size.y / 2 && localPoint.z <= Size.y / 2)
				{
					localPoint.y = 0;

					var worldPoint = Quaternion.FromToRotation(Vector3.up, Up) * localPoint + Offset;

					newPoint.Position = worldPoint;
					newPoint.Normal =
						NormalUtility.GetNormal(NormalNode, worldPoint, Offset, point.Normal, Up.normalized);
					results.Add(newPoint);
				}
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
						y = Offset.y,
						z = -half.y + j * hstep + Offset.z
					};

					pos = rot * pos;
					var up = Up.normalized;

					var point = new PointData
					{
						Position = pos,
						Normal = NormalUtility.GetNormal(NormalNode, pos, Offset, up, up),
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
					y = Offset.y,
					z = -half.y + Random.Range(0, Size.y) + Offset.z
				};

				pos = rot * pos;
				var up = Up.normalized;
				
				var point = new PointData
				{
					Position = pos,
					Normal = NormalUtility.GetNormal(NormalNode, pos, Offset, Vector3.up, up),
					Scale = Vector3.one
				};
				points.Add(point);
			}
			Random.state = state;
		}

		public override void DrawGizmos(Transform transform)
		{
			GizmosUtility.DrawPlane(Size, Up, Offset, transform);
		}
	}
}