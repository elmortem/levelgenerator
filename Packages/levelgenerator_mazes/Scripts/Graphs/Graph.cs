using System;
using System.Collections.Generic;
using Delone;
using UnityEngine;

namespace LevelGenerator.Mazes.Graphs
{
    [Serializable]
    public class GraphNode
    {
        public Vector2 Point { get; }
        public List<GraphEdge> Edges { get; }

        public GraphNode(Vector2 point)
        {
            Point = point;
            Edges = new List<GraphEdge>();
        }
    }

    [Serializable]
    public class GraphEdge
    {
        public GraphNode Node1 { get; }
        public GraphNode Node2 { get; }
        public float Weight { get; set; }

        public GraphEdge(GraphNode node1, GraphNode node2, float weight)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
        }
    }

    [Serializable]
    public class Graph
    {
        public List<GraphNode> Nodes { get; }
        public List<GraphEdge> Edges { get; }

        public Graph()
        {
            Nodes = new List<GraphNode>();
            Edges = new List<GraphEdge>();
        }
        
        public void Clear()
        {
            Nodes.Clear();
            Edges.Clear();
        }

        public GraphNode FindNode(Vector2 point)
        {
            return Nodes.Find(node => node.Point.Equals(point));
        }

        public GraphEdge FindEdge(GraphNode node1, GraphNode node2)
        {
            return Edges.Find(edge => 
                (edge.Node1 == node1 && edge.Node2 == node2) || 
                (edge.Node1 == node2 && edge.Node2 == node1));
        }
    }

    public static class GraphBuilder
    {
        public static void BuildGraph(Graph graph, List<Triangle> triangles)
        {
            foreach (var triangle in triangles)
            {
                foreach (var point in triangle.Points)
                {
                    if (graph.FindNode(point) == null)
                    {
                        graph.Nodes.Add(new GraphNode(point));
                    }
                }
            }

            foreach (var triangle in triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 p1 = triangle.Points[i];
                    Vector2 p2 = triangle.Points[(i + 1) % 3];

                    var node1 = graph.FindNode(p1);
                    var node2 = graph.FindNode(p2);

                    if (graph.FindEdge(node1, node2) == null)
                    {
                        var edge = new GraphEdge(node1, node2, 0.5f);
                        graph.Edges.Add(edge);
                        node1.Edges.Add(edge);
                        node2.Edges.Add(edge);
                    }
                }
            }
        }

        public static void BuildGrid(Graph graph, int width, int height, float cellSize)
        {
            GraphNode[][] gridNodes = new GraphNode[width][];
            for (int index = 0; index < width; index++)
            {
                gridNodes[index] = new GraphNode[height];
            }

            var halfX = width * cellSize * 0.5f;
            var halfY = height * cellSize * 0.5f;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var point = new Vector2(x * cellSize - halfX, y * cellSize - halfY);
                    var node = graph.FindNode(point);
                    if (node == null)
                    {
                        node = new GraphNode(point);
                        graph.Nodes.Add(node);
                    }
                    gridNodes[x][y] = node;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var node = gridNodes[x][y];

                    if (x < width - 1)
                    {
                        var rightNode = gridNodes[x + 1][y];
                        if (graph.FindEdge(node, rightNode) == null)
                        {
                            var edge = new GraphEdge(node, rightNode, 0.5f);
                            graph.Edges.Add(edge);
                            node.Edges.Add(edge);
                            rightNode.Edges.Add(edge);
                        }
                    }

                    if (y < height - 1)
                    {
                        var topNode = gridNodes[x][y + 1];
                        if (graph.FindEdge(node, topNode) == null)
                        {
                            var edge = new GraphEdge(node, topNode, 0.5f);
                            graph.Edges.Add(edge);
                            node.Edges.Add(edge);
                            topNode.Edges.Add(edge);
                        }
                    }
                }
            }
        }
    }
}