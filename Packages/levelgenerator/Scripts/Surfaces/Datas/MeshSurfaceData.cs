using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;

namespace LevelGenerator.Surfaces.Datas
{
    public enum MeshSurfaceNormalMode
    {
        Default,
        Up,
        ToCenter,
        FromCenter
    }

    public enum MeshProjectionMode
    {
        Sphere,
        Box,
        Vertices,
        Polygons
    }

    [Serializable]
    public class MeshSurfaceData : BaseSurfaceData
    {
        public Mesh Mesh;
        public Vector3 Offset = Vector3.zero;
        public MeshSurfaceNormalMode NormalMode = MeshSurfaceNormalMode.Default;
        public MeshProjectionMode ProjectionMode = MeshProjectionMode.Sphere;

        public override void GetPoints(List<PointData> points, SurfacePointMode mode, int count, int seed = 0)
        {
            if (Mesh == null)
            {
                Debug.LogWarning("Mesh is not assigned.");
                return;
            }

            if (count <= 0)
                return;

            switch (mode)
            {
                case SurfacePointMode.SurfaceRegular:
                    GetSurfacePoints(points, count);
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

        private void GetSurfacePoints(List<PointData> points, int count)
        {
            switch (ProjectionMode)
            {
                case MeshProjectionMode.Sphere:
                    new SphereProjection(Mesh, Offset, NormalMode).GetRegularSurfacePoints(points, count);
                    break;
                case MeshProjectionMode.Box:
                    new BoxProjection(Mesh, Offset, NormalMode).GetRegularSurfacePoints(points, count);
                    break;
                case MeshProjectionMode.Vertices:
                    new VerticesProjection(Mesh, Offset, NormalMode).GetSurfacePoints(points, count);
                    break;
                case MeshProjectionMode.Polygons:
                    new PolygonsProjection(Mesh, Offset, NormalMode).GetSurfacePoints(points, count);
                    break;
            }
        }

        private void GetRegularVolumePoints(List<PointData> points, int count)
        {
            switch (ProjectionMode)
            {
                case MeshProjectionMode.Sphere:
                    new SphereProjection(Mesh, Offset, NormalMode).GetRegularVolumePoints(points, count);
                    break;
                case MeshProjectionMode.Box:
                    new BoxProjection(Mesh, Offset, NormalMode).GetRegularVolumePoints(points, count);
                    break;
            }
        }

        private void GetRandomSurfacePoints(List<PointData> points, int count, int seed)
        {
            switch (ProjectionMode)
            {
                case MeshProjectionMode.Sphere:
                    new SphereProjection(Mesh, Offset, NormalMode).GetRandomSurfacePoints(points, count, seed);
                    break;
                case MeshProjectionMode.Box:
                    new BoxProjection(Mesh, Offset, NormalMode).GetRandomSurfacePoints(points, count, seed);
                    break;
                case MeshProjectionMode.Vertices:
                    new VerticesProjection(Mesh, Offset, NormalMode).GetRandomSurfacePoints(points, count, seed);
                    break;
                case MeshProjectionMode.Polygons:
                    new PolygonsProjection(Mesh, Offset, NormalMode).GetRandomSurfacePoints(points, count, seed);
                    break;
            }
        }

        private void GetRandomVolumePoints(List<PointData> points, int count, int seed)
        {
            switch (ProjectionMode)
            {
                case MeshProjectionMode.Sphere:
                    new SphereProjection(Mesh, Offset, NormalMode).GetRandomVolumePoints(points, count, seed);
                    break;
                case MeshProjectionMode.Box:
                    new BoxProjection(Mesh, Offset, NormalMode).GetRandomVolumePoints(points, count, seed);
                    break;
            }
        }
    }
}