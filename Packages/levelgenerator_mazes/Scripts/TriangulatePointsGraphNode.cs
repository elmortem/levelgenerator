using System.Collections.Generic;
using Delone;
using LevelGenerator.Mazes.Graphs;
using LevelGenerator.Mazes.Utilities;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace LevelGenerator.Mazes
{
	public class TriangulatePointsGraphNode : BasePointsNode
	{
		[Input] public List<PointData> Points;
		public float MinDistance = 100f;
		public float MinRation = 0.3f;
		[Output] public Graph Graph;
		[Output] public List<PointData> CenterPoints;
		[Header("Gizmos")]
		public bool ShowCenterPoints;
		
		private float _lastMinDistance;
		private float _lastMinRatio;
		private Graph _graph;
		private List<PointData> _centerPoints;

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Graph))
			{
				CalcResults();
				return _graph;
			}
			if (port.fieldName == nameof(CenterPoints))
			{
				CalcResults();
				return _centerPoints;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_graph = null;
				_centerPoints = null;
				return;
			}

			if (LockCalc && _graph != null)
				return;
			if (!force && _graph != null && _centerPoints != null && Mathf.Approximately(_lastMinDistance, MinDistance) && Mathf.Approximately(_lastMinRatio, MinRation))
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length <= 0)
				return;

			_gizmosOptions = null;
			_lastMinDistance = MinDistance;
			_lastMinRatio = MinRation;

			var triPoints = new List<Vector2>();
			float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
			foreach (var points in pointsList)
			{
				foreach (var point in points)
				{
					var p = new Vector2(point.Position.x, point.Position.z);
					triPoints.Add(p);

					if (p.x < minX)
						minX = p.x;
					if (p.x > maxX)
						maxX = p.x;
					if (p.y < minY)
						minY = p.y;
					if (p.y > maxY)
						maxY = p.y;
				}
			}
			
			var minPoint = new Vector2(minX - 10f, minY - 10f);
			var maxPoint = new Vector2(maxX + 10f, maxY + 10f);

			var triangulation = new Triangulation(triPoints, minPoint, maxPoint);
			triangulation.Calc();
			
			// filter min/max triangles
			if(_graph == null)
				_graph = new Graph();
			else
				_graph.Clear();
			GenerateGraph(triangulation.Triangles, minX, minY, maxX, maxY);

			// center points
			if (_centerPoints == null)
				_centerPoints = new();
			else
				_centerPoints.Clear();

			foreach (var triangle in triangulation.Triangles)
			{
				var p = triangle.Centroid;
				_centerPoints.Add(new PointData { Position = new Vector3(p.x, 0, p.y), Normal = Vector3.up, Scale = Vector3.one });
			}
		}
		
		private void GenerateGraph(List<Triangle> triangles, float minX, float minY, float maxX, float maxY)
		{
			var trianglesList = new List<Triangle>();
			var distances = new List<float>();
			foreach (var triangle in triangles)
			{
				var ok = true;
				foreach (var point in triangle.Points)
				{
					if (point.x <= minX || point.x >= maxX || point.y <= minY || point.y >= maxY)
					{
						ok = false;
						break;
					}
				}

				if (ok)
				{
					if ((triangle.Points[0] - triangle.Points[1]).Magnitude() > _lastMinDistance ||
					    (triangle.Points[1] - triangle.Points[2]).Magnitude() > _lastMinDistance ||
					    (triangle.Points[2] - triangle.Points[0]).Magnitude() > _lastMinDistance)
					{
						ok = false;
					}
				}
				
				if (ok)
				{
					distances.Clear();
					distances.Add((triangle.Points[0] - triangle.Points[1]).Magnitude());
					distances.Add((triangle.Points[1] - triangle.Points[2]).Magnitude());
					distances.Add((triangle.Points[2] - triangle.Points[0]).Magnitude());

					distances.Sort();
					if (distances[0] / distances[2] < _lastMinRatio)
					{
						ok = false;
					}
				}
				
				if(!ok)
					continue;
				
				trianglesList.Add(triangle);
			}

			GraphBuilder.BuildGraph(_graph, trianglesList);
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var graphPort = GetOutputPort(nameof(Graph));
			var result = (Graph)GetValue(graphPort);
			if (result == null)
				return;
			
			var centerPointsPort = GetOutputPort(nameof(CenterPoints));
			var centerPoints = (List<PointData>)GetValue(centerPointsPort);
			if (centerPoints == null || centerPoints.Count <= 0)
				return;
			
			UpdateGizmosOptions();

			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			Gizmos.matrix = transform.localToWorldMatrix;
			
			GraphGizmos.DrawGraph(result);
			
			Gizmos.matrix = Matrix4x4.identity;

			if (ShowCenterPoints)
			{
				GizmosUtility.DrawPoints(centerPoints, _gizmosOptions?.PointSize * 2f ?? 1f, false, false, Color.cyan, transform);
			}
		}
#endif
	}
}