using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Points;
using UnityEngine;

namespace LevelGenerator.Utility
{
	public static class GizmosUtility
	{
		public static void DrawWirePoints(List<Vector3> points, float radius, Transform transform, GizmosOptions gizmosOptions)
		{
			Gizmos.color = gizmosOptions?.Color ?? Color.white;

			Gizmos.matrix = transform.localToWorldMatrix;
			
			foreach (var point in points)
			{
				Gizmos.DrawWireSphere(point, radius);
			}

			Gizmos.matrix = Matrix4x4.identity;
		}

		public static void DrawPoints(List<Vector3> points, float radius, Transform transform, GizmosOptions gizmosOptions)
		{
			Gizmos.color = gizmosOptions?.Color ?? Color.white;

			Gizmos.matrix = transform.localToWorldMatrix;

			var maxCount = Mathf.Min(10000, points.Count);
			for (int i = 0; i < maxCount; i++)
			{
				var point = points[i];
				Gizmos.DrawSphere(point, radius);
			}

			Gizmos.matrix = Matrix4x4.identity;
		}

		public static void DrawPoint(Vector3 point, float radius, Transform transform, GizmosOptions gizmosOptions)
		{
			Gizmos.color = gizmosOptions?.Color ?? Color.white;

			Gizmos.matrix = transform.localToWorldMatrix;

			Gizmos.DrawSphere(point, radius);

			Gizmos.matrix = Matrix4x4.identity;
		}
		
		public static void DrawWirePoints(List<PointData> points, float radius, Color color, Transform transform)
		{
			Gizmos.color = color;

			Gizmos.matrix = transform.localToWorldMatrix;
			
			var maxCount = Mathf.Min(10000, points.Count);
			for (int i = 0; i < maxCount; i++)
			{
				var point = points[i];
				Gizmos.DrawWireSphere(point.Position, radius * point.Scale.Avarage());
			}

			Gizmos.matrix = Matrix4x4.identity;
		}

		public static void DrawPoints(List<PointData> points, GizmosOptions gizmosOptions, Transform transform)
		{
			DrawPoints(points, 
				gizmosOptions?.PointSize ?? 0.2f, 
				gizmosOptions?.DrawNormals ?? true, 
				gizmosOptions?.DrawRotation ?? true,
				gizmosOptions?.Color ?? Color.white, 
				transform);
		}

		public static void DrawPoints(List<PointData> points, float radius, bool drawNormals, bool drawRotation, Color color, Transform transform)
		{
			Gizmos.color = color;

			Gizmos.matrix = transform.localToWorldMatrix;

			var maxCount = Mathf.Min(10000, points.Count);
			for (int i = 0; i < maxCount; i++)
			{
				var point = points[i];
				Gizmos.DrawSphere(point.Position, radius);
        
				if(drawNormals)
					Gizmos.DrawLine(point.Position, point.Position + point.Normal * 2f);

				if (drawRotation)
				{
					Vector3 rotationVector = Quaternion.Euler(0, point.Angle, 0) * Vector3.forward;
					Gizmos.DrawLine(point.Position, point.Position + rotationVector);
				}
			}

			Gizmos.matrix = Matrix4x4.identity;
		}

		public static void DrawPlane(Vector2 size, Vector3 up, Vector3 offset, Transform transform)
		{
			var rotation = Quaternion.FromToRotation(Vector3.up, up);
			var halfSize = size / 2f;

			var corners = new Vector3[4];
			corners[0] = new Vector3(-halfSize.x, 0, -halfSize.y);
			corners[1] = new Vector3(-halfSize.x, 0, halfSize.y);
			corners[2] = new Vector3(halfSize.x, 0, halfSize.y);
			corners[3] = new Vector3(halfSize.x, 0, -halfSize.y);

			for (int i = 0; i < corners.Length; i++)
			{
				corners[i] = rotation * corners[i] + offset + transform.position;
			}

			Gizmos.DrawLine(corners[0], corners[1]);
			Gizmos.DrawLine(corners[1], corners[2]);
			Gizmos.DrawLine(corners[2], corners[3]);
			Gizmos.DrawLine(corners[3], corners[0]);

			//Gizmos.DrawLine(corners[0], corners[2]);
			//Gizmos.DrawLine(corners[1], corners[3]);
		}
	}
}