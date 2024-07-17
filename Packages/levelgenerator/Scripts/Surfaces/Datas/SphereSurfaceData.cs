using System;
using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGenerator.Surfaces.Datas
{
    public enum SphereSurfaceNormalMode
    {
        Default,
        Up,
        ToCenter,
        FromCenter
    }
    
    [Serializable]
    public class SphereSurfaceData : BaseSurfaceData
    {
        public Vector3 Offset = Vector3.zero;
        public float Radius = 50f;
        [NodeEnum]
        public SphereSurfaceNormalMode NormalMode = SphereSurfaceNormalMode.Up;

        public override void GetPoints(List<PointData> points, GeneratePointMode mode, int count, int seed = 0)
        {
            if(count <= 0)
                return;
            
            switch (mode)
            {
                case GeneratePointMode.SurfaceRegular:
                    GetRegularSurfacePoints(points, count);
                    break;
                case GeneratePointMode.VolumeRegular:
                    GetRegularVolumePoints(points, count);
                    break;
                case GeneratePointMode.SurfaceRandom:
                    GetRandomSurfacePoints(points, count, seed);
                    break;
                case GeneratePointMode.VolumeRandom:
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

                Vector3 pos = new Vector3(x, y, z) * Radius + Offset;
                Vector3 normal = GetNormal(pos, new Vector3(x, y, z).normalized);

                var point = new PointData
                {
                    Position = pos,
                    Normal = normal,
                    Scale = Vector3.one
                };
                points.Add(point);
            }
        }

        private void GetRegularVolumePoints(List<PointData> points, int count)
        {
            int pointsPerAxis = Mathf.CeilToInt(Mathf.Pow(count, 1.0f / 3.0f));

            float step = 2.0f / (pointsPerAxis - 1);
            float radiusStep = Mathf.Pow(1.0f / (pointsPerAxis - 1), 1.0f / 3.0f);

            for (int i = 0; i < pointsPerAxis; i++)
            {
                for (int j = 0; j < pointsPerAxis; j++)
                {
                    for (int k = 0; k < pointsPerAxis; k++)
                    {
                        float u = i * step - 1.0f;
                        float v = j * step - 1.0f;
                        float w = k * step - 1.0f;

                        if (u * u + v * v + w * w <= 1.0f)
                        {
                            Vector3 pos = new Vector3(u, v, w) * Radius + Offset;

                            var point = new PointData
                            {
                                Position = pos,
                                Normal = GetNormal(pos, (pos - Offset).normalized),
                                Scale = Vector3.one
                            };
                            points.Add(point);
                        }
                    }
                }
            }
        }

        private void GetRandomSurfacePoints(List<PointData> points, int count, int seed)
        {
            var state = Random.state;
            Random.InitState(seed);

            for (int i = 0; i < count; i++)
            {
                float theta = Random.Range(0f, Mathf.PI * 2);
                float phi = Mathf.Acos(2 * Random.Range(0f, 1f) - 1);
                
                float x = Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = Mathf.Sin(phi) * Mathf.Sin(theta);
                float z = Mathf.Cos(phi);

                Vector3 pos = new Vector3(x, y, z) * Radius + Offset;
                Vector3 normal = GetNormal(pos, new Vector3(x, y, z).normalized);

                var point = new PointData
                {
                    Position = pos,
                    Normal = normal,
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

            for (int i = 0; i < count; i++)
            {
                float u = Random.Range(0f, 1f);
                float v = Random.Range(0f, 1f);
                float w = Random.Range(0f, 1f);

                float theta = u * Mathf.PI * 2;
                float phi = Mathf.Acos(2 * v - 1);
                float r = Mathf.Pow(w, 1f / 3f) * Radius;

                float x = r * Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = r * Mathf.Sin(phi) * Mathf.Sin(theta);
                float z = r * Mathf.Cos(phi);

                Vector3 pos = new Vector3(x, y, z) + Offset;
                Vector3 normal = GetNormal(pos, (pos - Offset).normalized);

                var point = new PointData
                {
                    Position = pos,
                    Normal = normal,
                    Scale = Vector3.one
                };
                points.Add(point);
            }

            Random.state = state;
        }

        private Vector3 GetNormal(Vector3 position, Vector3 defaultNormal)
        {
            switch (NormalMode)
            {
                case SphereSurfaceNormalMode.Default:
                    return defaultNormal;
                case SphereSurfaceNormalMode.ToCenter:
                    return (Offset - position).normalized;
                case SphereSurfaceNormalMode.FromCenter:
                    return (position - Offset).normalized;
                case SphereSurfaceNormalMode.Up:
                default:
                    return Vector3.up;
            }
        }

        public override void DrawGizmos(Transform transform)
        {
            Gizmos.DrawWireSphere(transform.position + Offset, Radius);
        }
    }
}