using LevelGenerator.Mazes.Graphs;
using UnityEngine;

namespace LevelGenerator.Mazes.Utilities
{
	public static class GraphGizmos
	{
		public static void DrawGraph(Graph graph)
		{
			if (graph == null)
				return;

			foreach (var edge in graph.Edges)
			{
				Vector3 start = new Vector3(edge.Node1.Point.x, 0f, edge.Node1.Point.y);
				Vector3 end = new Vector3(edge.Node2.Point.x, 0f, edge.Node2.Point.y);
				Gizmos.DrawLine(start, end);
			}
		}
	}
}