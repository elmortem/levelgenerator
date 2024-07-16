using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Mazes.Graphs;
using LevelGenerator.Mazes.Utilities;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;

namespace LevelGenerator.Mazes
{
	public class MazeMSTNode : PreviewCalcNode
	{
		[Input] public Graph Graph;
		public int Seed = -1;
		[Output] public Graph MstGraph = new();
		[Output] public List<PointData> EndPoints = new();
		//[Output] public MeshSurfaceData Surface;

		private int _lastSeed;
		private Graph _mstGraph;
		private List<PointData> _endPoints;
		//private MeshSurfaceData _surface;

		private void Awake()
		{
			if (Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}
		
		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(MstGraph))
			{
				CalcResults();
				return _mstGraph;
			}
			if (port.fieldName == nameof(EndPoints))
			{
				CalcResults();
				return _endPoints;
			}

			return null;
		}
		
		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Graph));
			if (port == null || !port.IsConnected)
			{
				_mstGraph = null;
				_endPoints = null;
				return;
			}
			
			if(LockCalc && _mstGraph != null && _mstGraph.Edges.Count > 0 && _endPoints != null)
				return;
			if(!force && _lastSeed == Seed && _mstGraph != null && _mstGraph.Edges.Count > 0 && _endPoints != null)
				return;
			
			var graph = GetInputValue(nameof(Graph), Graph);
			if (graph == null)
				return;

			ResetGizmosOptions();
			
			_lastSeed = Seed;
			
			var state = Random.state;
			Random.InitState(Seed);
			
			// Randomize weights
			foreach (var edge in graph.Edges)
			{
				edge.Weight = Random.value;
			}
			
			_mstGraph = MazeGenerator.GenerateMaze(graph);
			if (_mstGraph != null)
			{
				// Add end points
				if (_endPoints == null)
					_endPoints = new();
				else
					_endPoints.Clear();

				_endPoints.AddRange(_mstGraph.Nodes.Where(node => node.Edges.Count == 1).Select(p =>
					new PointData
					{
						Position = new Vector3(p.Point.x, 0f, p.Point.y), Normal = Vector3.up, Scale = Vector3.one
					}));
			}

			Random.state = state;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var graphPort = GetOutputPort(nameof(MstGraph));
			var graph = (Graph)GetValue(graphPort);
			if(graph == null)
				return;

			Gizmos.color = gizmosOptions.Color;
			Gizmos.matrix = transform.localToWorldMatrix;

			GraphGizmos.DrawGraph(graph);

			Gizmos.matrix = Matrix4x4.identity;

			GizmosUtility.DrawPoints(_endPoints, gizmosOptions, transform);
		}
#endif
	}
}