using System.Collections.Generic;
using System.Linq;

namespace LevelGenerator.Mazes.Graphs
{
	public static class MazeGenerator
	{
		public static Graph GenerateMaze(Graph graph)
		{
			if (graph.Nodes.Count == 0)
				return null;

			var mst = new Graph();
			var visitedNodes = new HashSet<GraphNode>();
			var edgeQueue = new List<GraphEdge>();

			var startNode = graph.Nodes[0];
			visitedNodes.Add(startNode);
			mst.Nodes.Add(new GraphNode(startNode.Point));

			edgeQueue.AddRange(startNode.Edges.OrderBy(e => e.Weight));

			while (edgeQueue.Count > 0)
			{
				var edge = edgeQueue[0];
				edgeQueue.RemoveAt(0);

				var nextNode = visitedNodes.Contains(edge.Node1) ? edge.Node2 : edge.Node1;

				if (visitedNodes.Contains(nextNode))
					continue;

				visitedNodes.Add(nextNode);
				mst.Nodes.Add(new GraphNode(nextNode.Point));

				var mstNode1 = mst.FindNode(edge.Node1.Point);
				var mstNode2 = mst.FindNode(edge.Node2.Point);

				var mstEdge = new GraphEdge(mstNode1, mstNode2, edge.Weight);
				mst.Edges.Add(mstEdge);
				mstNode1.Edges.Add(mstEdge);
				mstNode2.Edges.Add(mstEdge);

				foreach (var nextEdge in nextNode.Edges)
				{
					if (!visitedNodes.Contains(nextEdge.Node1) || !visitedNodes.Contains(nextEdge.Node2))
					{
						edgeQueue.Add(nextEdge);
					}
				}

				edgeQueue = edgeQueue.OrderBy(e => e.Weight).ToList();
			}

			return mst;
		}
	}
}