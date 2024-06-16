using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;

namespace LevelGenerator.Points
{
	public abstract class BasePointsNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		protected GizmosOptions _gizmosOptions;
		
		protected void UpdateGizmosOptions()
		{
			if (_gizmosOptions == null)
			{
				foreach (var provider in this.GetNodeInParent<IGizmosOptionsProvider>())
				{
					_gizmosOptions = provider.GetGizmosOptions();
					break;
				}
			}
		}
		
		public GizmosOptions GetGizmosOptions()
		{
			UpdateGizmosOptions();
			return _gizmosOptions;
		}

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
	}
}