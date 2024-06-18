using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;

namespace LevelGenerator.Surfaces.Datas
{
    public class MeshBoxProjection
    {
        private Mesh _mesh;
        private Vector3 _offset;
        private SurfaceNormalMode _normalMode;

        public MeshBoxProjection(Mesh mesh, Vector3 offset, SurfaceNormalMode normalMode)
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
            bounds.Expand(5f);

            List<Vector3> cubePoints = GenerateCubePoints(count, bounds.size);

            foreach (var cubePoint in cubePoints)
            {
                Vector3 closestIntersection = Vector3.zero;
                float closestDistance = float.MaxValue;

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3 a = vertices[triangles[i]];
                    Vector3 b = vertices[triangles[i + 1]];
                    Vector3 c = vertices[triangles[i + 2]];

                    Vector3 intersection;
                    if (SegmentIntersectsTriangle(cubePoint, centerOfMass, a, b, c, out intersection))
                    {
                        float distance = Vector3.Distance(cubePoint, intersection);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestIntersection = intersection;
                        }
                    }
                }

                if (closestDistance < float.MaxValue)
                {
                    var point = new PointData
                    {
                        Position = closestIntersection + _offset,
                        Normal = GetNormal(closestIntersection, (closestIntersection - centerOfMass).normalized),
                        Scale = Vector3.one
                    };
                    points.Add(point);
                }
                
                var cPoint = new PointData
                {
                    Position = cubePoint + _offset,
                    Normal = GetNormal(cubePoint, (cubePoint - centerOfMass).normalized),
                    Scale = Vector3.one
                };
                points.Add(cPoint);
            }
        }

        public void GetRegularVolumePoints(List<PointData> points, int count)
        {
            UnityEngine.Bounds bounds = _mesh.bounds;
            bounds.Expand(5f);
            float boxVolume = bounds.size.x * bounds.size.y * bounds.size.z;
            float meshVolume = GetMeshVolume();
            float ratio = meshVolume / boxVolume;
            int newCount = Mathf.CeilToInt(count * ratio);

            List<Vector3> cubePoints = GenerateRegularCubePoints(newCount, bounds.size);

            foreach (var point in cubePoints)
            {
                if (IsPointInsideMesh(point))
                {
                    points.Add(new PointData
                    {
                        Position = point + _offset,
                        Normal = GetNormal(point, Vector3.zero),
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
            bounds.Expand(5f);

            List<Vector3> cubePoints = GenerateRandomCubePoints(count, bounds.size, seed);

            foreach (var cubePoint in cubePoints)
            {
                Vector3 closestIntersection = Vector3.zero;
                float closestDistance = float.MaxValue;

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3 a = vertices[triangles[i]];
                    Vector3 b = vertices[triangles[i + 1]];
                    Vector3 c = vertices[triangles[i + 2]];

                    Vector3 intersection;
                    if (SegmentIntersectsTriangle(cubePoint, centerOfMass, a, b, c, out intersection))
                    {
                        float distance = Vector3.Distance(cubePoint, intersection);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestIntersection = intersection;
                        }
                    }
                }

                if (closestDistance < float.MaxValue)
                {
                    var point = new PointData
                    {
                        Position = closestIntersection + _offset,
                        Normal = GetNormal(closestIntersection, (closestIntersection - centerOfMass).normalized),
                        Scale = Vector3.one
                    };
                    points.Add(point);
                }
            }
        }

        public void GetRandomVolumePoints(List<PointData> points, int count, int seed)
        {
            UnityEngine.Bounds bounds = _mesh.bounds;
            bounds.Expand(5f);
            float boxVolume = bounds.size.x * bounds.size.y * bounds.size.z;
            float meshVolume = GetMeshVolume();
            float ratio = meshVolume / boxVolume;
            int newCount = Mathf.CeilToInt(count * ratio);

            List<Vector3> cubePoints = GenerateRandomCubePoints(newCount, bounds.size, seed);

            foreach (var point in cubePoints)
            {
                if (IsPointInsideMesh(point))
                {
                    points.Add(new PointData
                    {
                        Position = point + _offset,
                        Normal = GetNormal(point, Vector3.zero),
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

            foreach (var triangle in triangles)
            {
                Vector3 a = vertices[triangle];
                Vector3 b = vertices[triangle + 1];
                Vector3 c = vertices[triangle + 2];

                if (Vector3.Dot(Vector3.Cross(b - a, point - a), Vector3.Cross(b - a, c - a)) > 0 &&
                    Vector3.Dot(Vector3.Cross(c - b, point - b), Vector3.Cross(c - b, a - b)) > 0 &&
                    Vector3.Dot(Vector3.Cross(a - c, point - c), Vector3.Cross(a - c, b - c)) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool SegmentIntersectsTriangle(Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1, Vector3 v2, out Vector3 intersection)
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

        private List<Vector3> GenerateCubePoints(int count, Vector3 size)
        {
            List<Vector3> points = new List<Vector3>();
            int perAxis = Mathf.CeilToInt(Mathf.Pow(count, 1f / 3f));
            Vector3 step = new Vector3(size.x / (perAxis - 1), size.y / (perAxis - 1), size.z / (perAxis - 1));

            for (int x = 0; x < perAxis; x++)
            {
                for (int y = 0; y < perAxis; y++)
                {
                    for (int z = 0; z < perAxis; z++)
                    {
                        points.Add(new Vector3(-size.x / 2 + x * step.x, -size.y / 2 + y * step.y, -size.z / 2 + z * step.z));
                    }
                }
            }

            return points;
        }

        private List<Vector3> GenerateRegularCubePoints(int count, Vector3 size)
        {
            List<Vector3> points = new List<Vector3>();
            int perAxis = Mathf.CeilToInt(Mathf.Pow(count, 1f / 3f));
            Vector3 step = new Vector3(size.x / (perAxis - 1), size.y / (perAxis - 1), size.z / (perAxis - 1));

            for (int x = 0; x < perAxis; x++)
            {
                for (int y = 0; y < perAxis; y++)
                {
                    for (int z = 0; z < perAxis; z++)
                    {
                        points.Add(new Vector3(-size.x / 2 + x * step.x, -size.y / 2 + y * step.y, -size.z / 2 + z * step.z));
                    }
                }
            }

            return points;
        }

        private List<Vector3> GenerateRandomCubePoints(int count, Vector3 size, int seed)
        {
            List<Vector3> points = new List<Vector3>();
            var random = new System.Random(seed);

            for (int i = 0; i < count; i++)
            {
                float x = (float)random.NextDouble() * size.x - size.x / 2;
                float y = (float)random.NextDouble() * size.y - size.y / 2;
                float z = (float)random.NextDouble() * size.z - size.z / 2;

                points.Add(new Vector3(x, y, z));
            }

            return points;
        }

        private Vector3 GetNormal(Vector3 position, Vector3 defaultNormal)
        {
            switch (_normalMode)
            {
                case SurfaceNormalMode.Default:
                    return defaultNormal;
                case SurfaceNormalMode.ToCenter:
                    return (_offset - position).normalized;
                case SurfaceNormalMode.FromCenter:
                    return (position - _offset).normalized;
                case SurfaceNormalMode.Up:
                default:
                    return Vector3.up;
            }
        }
    }
}