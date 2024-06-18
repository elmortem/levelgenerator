using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Surfaces.Datas
{
	public class MeshSphereProjection
	{
		private Mesh _mesh;
		private Vector3 _offset;
		private SurfaceNormalMode _normalMode;

		public MeshSphereProjection(Mesh mesh, Vector3 offset, SurfaceNormalMode normalMode)
		{
			_mesh = mesh;
			_offset = offset;
			_normalMode = normalMode;
		}

		public void GetRegularSurfacePoints(List<PointData> points, int count)
		{
			var vertices = _mesh.vertices;
			var triangles = _mesh.triangles;

			Vector3 centerOfMass = GetCenterOfMass(vertices);
			UnityEngine.Bounds bounds = _mesh.bounds;
			float radius = bounds.extents.magnitude + 5f;

			List<Vector3> spherePoints = GenerateSpherePoints(count, radius);

			foreach (var spherePoint in spherePoints)
			{
				Vector3 closestIntersection = Vector3.zero;
				float closestDistance = float.MaxValue;
				Vector3 direction = (centerOfMass - spherePoint).normalized;

				for (int i = 0; i < triangles.Length; i += 3)
				{
					Vector3 a = vertices[triangles[i]];
					Vector3 b = vertices[triangles[i + 1]];
					Vector3 c = vertices[triangles[i + 2]];

					Vector3 intersection;
					if (SegmentIntersectsTriangle(spherePoint, centerOfMass, a, b, c, out intersection))
					{
						float distance = Vector3.Distance(spherePoint, intersection);
						if (distance < closestDistance)
						{
							closestDistance = distance;
							closestIntersection = intersection;
						}
					}
				}

				if (closestDistance < float.MaxValue)
				{
					var normal = (closestIntersection - centerOfMass).normalized;
					var point = new PointData
					{
						Position = closestIntersection + _offset,
						Normal = NormalUtility.GetNormal(_normalMode, closestIntersection, _offset, normal, normal),
						Scale = Vector3.one
					};
					points.Add(point);
				}
			}
		}

		public void GetRegularVolumePoints(List<PointData> points, int count)
		{
			UnityEngine.Bounds bounds = _mesh.bounds;
			float sphereVolume = (4f / 3f) * Mathf.PI * Mathf.Pow(bounds.extents.magnitude, 3);
			float meshVolume = GetMeshVolume();
			float ratio = meshVolume / sphereVolume;
			int newCount = Mathf.CeilToInt(count * ratio);

			List<Vector3> spherePoints = GenerateRegularSpherePoints(newCount, bounds.extents.magnitude);

			foreach (var point in spherePoints)
			{
				if (IsPointInsideMesh(point))
				{
					points.Add(new PointData
					{
						Position = point + _offset,
						Normal = NormalUtility.GetNormal(_normalMode, point, _offset, Vector3.up, Vector3.up),
						Scale = Vector3.one
					});
				}
			}
		}

		public void GetRandomSurfacePoints(List<PointData> points, int count, int seed)
		{
			var vertices = _mesh.vertices;
			var triangles = _mesh.triangles;

			Vector3 centerOfMass = GetCenterOfMass(vertices);
			UnityEngine.Bounds bounds = _mesh.bounds;
			float radius = bounds.extents.magnitude + 5f;

			List<Vector3> spherePoints = GenerateRandomSpherePoints(count, radius, seed);

			foreach (var spherePoint in spherePoints)
			{
				Vector3 closestIntersection = Vector3.zero;
				float closestDistance = float.MaxValue;
				Vector3 direction = (centerOfMass - spherePoint).normalized;

				for (int i = 0; i < triangles.Length; i += 3)
				{
					Vector3 a = vertices[triangles[i]];
					Vector3 b = vertices[triangles[i + 1]];
					Vector3 c = vertices[triangles[i + 2]];

					Vector3 intersection;
					if (SegmentIntersectsTriangle(spherePoint, centerOfMass, a, b, c, out intersection))
					{
						float distance = Vector3.Distance(spherePoint, intersection);
						if (distance < closestDistance)
						{
							closestDistance = distance;
							closestIntersection = intersection;
						}
					}
				}

				if (closestDistance < float.MaxValue)
				{
					var normal = (closestIntersection - centerOfMass).normalized;
					var point = new PointData
					{
						Position = closestIntersection + _offset,
						Normal = NormalUtility.GetNormal(_normalMode, closestIntersection, _offset, normal, normal),
						Scale = Vector3.one
					};
					points.Add(point);
				}
			}
		}

		public void GetRandomVolumePoints(List<PointData> points, int count, int seed)
		{
			UnityEngine.Bounds bounds = _mesh.bounds;
			float sphereVolume = (4f / 3f) * Mathf.PI * Mathf.Pow(bounds.extents.magnitude, 3);
			float meshVolume = GetMeshVolume();
			float ratio = meshVolume / sphereVolume;
			int newCount = Mathf.CeilToInt(count * ratio);

			List<Vector3> spherePoints = GenerateRandomSpherePoints(newCount, bounds.extents.magnitude, seed);

			foreach (var point in spherePoints)
			{
				if (IsPointInsideMesh(point))
				{
					points.Add(new PointData
					{
						Position = point + _offset,
						Normal = NormalUtility.GetNormal(_normalMode, point, _offset, Vector3.up, Vector3.up),
						Scale = Vector3.one
					});
				}
			}
		}

		private Vector3 GetCenterOfMass(Vector3[] vertices)
		{
			Vector3 centerOfMass = Vector3.zero;
			foreach (var vertex in vertices)
			{
				centerOfMass += vertex;
			}

			return centerOfMass / vertices.Length;
		}

		private float GetMeshVolume()
		{
			var vertices = _mesh.vertices;
			var triangles = _mesh.triangles;
			float volume = 0f;

			for (int i = 0; i < triangles.Length; i += 3)
			{
				Vector3 a = vertices[triangles[i]];
				Vector3 b = vertices[triangles[i + 1]];
				Vector3 c = vertices[triangles[i + 2]];

				volume += SignedVolumeOfTriangle(a, b, c);
			}

			return Mathf.Abs(volume);
		}

		private float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			return Vector3.Dot(p1, Vector3.Cross(p2, p3)) / 6f;
		}

		private bool IsPointInsideMesh(Vector3 point)
		{
			var vertices = _mesh.vertices;
			var triangles = _mesh.triangles;
			int triangleCount = triangles.Length / 3;

			for (int i = 0; i < triangleCount; i++)
			{
				Vector3 a = vertices[triangles[i * 3]];
				Vector3 b = vertices[triangles[i * 3 + 1]];
				Vector3 c = vertices[triangles[i * 3 + 2]];

				if (Vector3.Dot(Vector3.Cross(b - a, point - a), Vector3.Cross(b - a, c - a)) > 0 &&
				    Vector3.Dot(Vector3.Cross(c - b, point - b), Vector3.Cross(c - b, a - b)) > 0 &&
				    Vector3.Dot(Vector3.Cross(a - c, point - c), Vector3.Cross(a - c, b - c)) > 0)
				{
					return true;
				}
			}

			return false;
		}

		private bool SegmentIntersectsTriangle(Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1, Vector3 v2,
			out Vector3 intersection)
		{
			intersection = Vector3.zero;
			Vector3 u = v1 - v0;
			Vector3 v = v2 - v0;
			Vector3 n = Vector3.Cross(u, v);
			Vector3 dir = p1 - p0;
			Vector3 w0 = p0 - v0;
			float a = -Vector3.Dot(n, w0);
			float b = Vector3.Dot(n, dir);

			if (Mathf.Abs(b) < 0.00001f) // Луч параллелен треугольнику
			{
				return false;
			}

			float r = a / b;
			if (r < 0.0f || r > 1.0f) // Пересечение вне отрезка
			{
				return false;
			}

			intersection = p0 + r * dir;

			// Проверка, находится ли точка пересечения внутри треугольника
			float uu = Vector3.Dot(u, u);
			float uv = Vector3.Dot(u, v);
			float vv = Vector3.Dot(v, v);
			Vector3 w = intersection - v0;
			float wu = Vector3.Dot(w, u);
			float wv = Vector3.Dot(w, v);
			float D = uv * uv - uu * vv;

			float s = (uv * wv - vv * wu) / D;
			if (s < 0.0f || s > 1.0f)
			{
				return false;
			}

			float t = (uv * wu - uu * wv) / D;
			if (t < 0.0f || (s + t) > 1.0f)
			{
				return false;
			}

			return true;
		}

		private List<Vector3> GenerateSpherePoints(int count, float radius)
		{
			List<Vector3> points = new List<Vector3>();
			float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
			float angleIncrement = Mathf.PI * 2 * goldenRatio;

			for (int i = 0; i < count; i++)
			{
				float t = (float)i / count;
				float inclination = Mathf.Acos(1 - 2 * t);
				float azimuth = angleIncrement * i;

				float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
				float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
				float z = Mathf.Cos(inclination);

				points.Add(new Vector3(x, y, z) * radius);
			}

			return points;
		}

		private List<Vector3> GenerateRegularSpherePoints(int count, float radius)
		{
			List<Vector3> points = new List<Vector3>();
			int perAxis = Mathf.CeilToInt(Mathf.Pow(count, 1f / 3f));
			float step = 2f * radius / (perAxis - 1);

			for (int x = 0; x < perAxis; x++)
			{
				for (int y = 0; y < perAxis; y++)
				{
					for (int z = 0; z < perAxis; z++)
					{
						Vector3 point = new Vector3(
							-radius + x * step,
							-radius + y * step,
							-radius + z * step
						);

						if (point.magnitude <= radius)
						{
							points.Add(point);
						}
					}
				}
			}

			return points;
		}

		private List<Vector3> GenerateRandomSpherePoints(int count, float radius, int seed)
		{
			List<Vector3> points = new List<Vector3>();
			var random = new System.Random(seed);

			for (int i = 0; i < count; i++)
			{
				float u = (float)random.NextDouble();
				float v = (float)random.NextDouble();
				float theta = 2 * Mathf.PI * u;
				float phi = Mathf.Acos(2 * v - 1);

				float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
				float y = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
				float z = radius * Mathf.Cos(phi);

				points.Add(new Vector3(x, y, z));
			}

			return points;
		}
	}
}