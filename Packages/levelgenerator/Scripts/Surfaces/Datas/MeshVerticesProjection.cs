using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Surfaces.Datas
{
	public class MeshVerticesProjection
    {
        private Mesh _mesh;
        private Vector3 _offset;
        private SurfaceNormalMode _normalMode;

        public MeshVerticesProjection(Mesh mesh, Vector3 offset, SurfaceNormalMode normalMode)
        {
            _mesh = mesh;
            _offset = offset;
            _normalMode = normalMode;
        }

        public void GetSurfacePoints(List<PointData> points, int count)
        {
            var vertices = _mesh.vertices;
            var normals = _mesh.normals;
            int step = Mathf.Max(1, vertices.Length / count);

            for (int i = 0; i < vertices.Length; i += step)
            {
                var point = new PointData
                {
                    Position = vertices[i] + _offset,
                    Normal = NormalUtility.GetNormal(_normalMode, vertices[i], _offset, normals[i], normals[i]),
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
            HashSet<int> usedIndices = new HashSet<int>();

            while (points.Count < count && usedIndices.Count < vertices.Length)
            {
                int index = UnityEngine.Random.Range(0, vertices.Length);
                if (!usedIndices.Contains(index))
                {
                    var point = new PointData
                    {
                        Position = vertices[index] + _offset,
                        Normal = NormalUtility.GetNormal(_normalMode, vertices[index], _offset, normals[index], normals[index]),
                        Scale = Vector3.one
                    };
                    points.Add(point);
                    usedIndices.Add(index);
                }
            }

            UnityEngine.Random.state = state;
        }
    }
}