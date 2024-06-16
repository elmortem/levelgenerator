using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Surfaces.Datas
{
	public class PolygonsProjection
    {
        private Mesh _mesh;
        private Vector3 _offset;
        private MeshSurfaceNormalMode _normalMode;

        public PolygonsProjection(Mesh mesh, Vector3 offset, MeshSurfaceNormalMode normalMode)
        {
            _mesh = mesh;
            _offset = offset;
            _normalMode = normalMode;
        }

        public void GetSurfacePoints(List<PointData> points, int count)
        {
            var vertices = _mesh.vertices;
            var normals = _mesh.normals;
            var triangles = _mesh.triangles;
            int step = Mathf.Max(1, (triangles.Length / 3) / count);

            for (int i = 0; i < triangles.Length; i += 3 * step)
            {
                Vector3 a = vertices[triangles[i]];
                Vector3 b = vertices[triangles[i + 1]];
                Vector3 c = vertices[triangles[i + 2]];

                Vector3 centroid = (a + b + c) / 3;
                Vector3 normal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]) / 3;

                var point = new PointData
                {
                    Position = centroid + _offset,
                    Normal = NormalUtility.GetNormal(_normalMode, centroid, _offset, normal),
                    Scale = Vector3.one
                };
                points.Add(point);
            }
        }

        public void GetRandomSurfacePoints(List<PointData> points, int count, int seed)
        {
            var state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);

            var vertices = _mesh.vertices;
            var normals = _mesh.normals;
            var triangles = _mesh.triangles;
            HashSet<int> usedIndices = new HashSet<int>();

            while (points.Count < count && usedIndices.Count < (triangles.Length / 3))
            {
                int triIndex = UnityEngine.Random.Range(0, triangles.Length / 3) * 3;
                if (!usedIndices.Contains(triIndex))
                {
                    Vector3 a = vertices[triangles[triIndex]];
                    Vector3 b = vertices[triangles[triIndex + 1]];
                    Vector3 c = vertices[triangles[triIndex + 2]];

                    Vector3 centroid = (a + b + c) / 3;
                    Vector3 normal = (normals[triangles[triIndex]] + normals[triangles[triIndex + 1]] + normals[triangles[triIndex + 2]]) / 3;

                    var point = new PointData
                    {
                        Position = centroid + _offset,
                        Normal = NormalUtility.GetNormal(_normalMode, centroid, _offset, normal),
                        Scale = Vector3.one
                    };
                    points.Add(point);
                    usedIndices.Add(triIndex);
                }
            }

            UnityEngine.Random.state = state;
        }
    }
}