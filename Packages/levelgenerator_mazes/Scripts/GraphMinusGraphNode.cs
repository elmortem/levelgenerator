using Delone;
using LevelGenerator.Mazes.Graphs;
using LevelGenerator.Mazes.Utilities;
using LevelGenerator.Points;
using UnityEngine;
using XNode;

namespace LevelGenerator.Mazes
{
	public class GraphMinusGraphNode : PreviewCalcNode
	{
		[Input] public Graph Graph;
		[Input] public Graph Minus;
		[Output] public Graph Result;

		private Graph _result;
		
		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Result))
			{
				CalcResults();
				return _result;
			}

			return null;
		}
		
		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Graph));
			if (port == null || !port.IsConnected)
			{
				_result = null;
				return;
			}
			
			port = GetInputPort(nameof(Minus));
			if (port == null || !port.IsConnected)
			{
				_result = null;
				return;
			}
			
			if(LockCalc && _result != null && _result.Edges.Count > 0)
				return;
			if(!force && _result != null && _result.Edges.Count > 0)
				return;
			
			var graph = GetInputValue(nameof(Graph), Graph);
			if (graph == null)
				return;
			
			var minus = GetInputValue(nameof(Minus), Minus);
			if (minus == null)
				return;
			
			ResetGizmosOptions();

			if (_result == null)
				_result = new();
			else
				_result.Clear();
			
			foreach (var node in graph.Nodes)
			{
				_result.Nodes.Add(new GraphNode(node.Point));
			}

			foreach (var edge in graph.Edges)
			{
				bool intersects = false;

				foreach (var minusEdge in minus.Edges)
				{
					if (EdgesIntersect(edge, minusEdge))
					{
						intersects = true;
						break;
					}
				}

				if (!intersects)
				{
					var node1 = _result.FindNode(edge.Node1.Point);
					var node2 = _result.FindNode(edge.Node2.Point);

					if (node1 != null && node2 != null)
					{
						var newEdge = new GraphEdge(node1, node2, edge.Weight);
						_result.Edges.Add(newEdge);
						node1.Edges.Add(newEdge);
						node2.Edges.Add(newEdge);
					}
				}
			}
		}
		
		private bool EdgesIntersect(GraphEdge edge1, GraphEdge edge2)
		{
			return Arc.ArcIntersect(new Arc(edge1.Node1.Point, edge1.Node2.Point), new Arc(edge2.Node1.Point, edge2.Node2.Point));
		}
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultPort = GetOutputPort(nameof(Result));
			var result = (Graph)GetValue(resultPort);
			if(result == null)
				return;

			Gizmos.color = gizmosOptions.Color;
			Gizmos.matrix = transform.localToWorldMatrix;

			GraphGizmos.DrawGraph(result);

			Gizmos.matrix = Matrix4x4.identity;
		}
#endif
	}
}