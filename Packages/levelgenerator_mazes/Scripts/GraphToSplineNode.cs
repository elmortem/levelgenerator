using System.Linq;
using LevelGenerator.Mazes.Graphs;
using LevelGenerator.Points;
using LevelGenerator.Splines;
using LevelGenerator.Splines.Utilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Mazes
{
	public class GraphToSplineNode : BasePointsNode
	{
		[Input] public Graph Graph;
		[Output] public SplineContainerData Spline;
		
		private SplineContainerData _splineContainer;
		
		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Spline))
			{
				CalcResults();
				return _splineContainer;
			}

			return null;
		}
		
		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Graph));
			if (port == null || !port.IsConnected)
			{
				_splineContainer = null;
				return;
			}
			
			if(LockCalc && _splineContainer != null)
				return;
			if(!force && _splineContainer != null)
				return;
			
			var graph = GetInputValue(nameof(Graph), Graph);
			if (graph == null)
				return;
			
			_gizmosOptions = null;
			
			// Add splines
			if (_splineContainer == null)
				_splineContainer = new();
			else
				_splineContainer.Splines.Clear();

			_splineContainer.Splines.AddRange(graph.Edges.Select(edge =>
			{
				var spline = new Spline();
				var knot = new BezierKnot(new float3(edge.Node1.Point.x, 0f, edge.Node1.Point.y), new float3(),
					new float3());
				spline.Add(knot, TangentMode.AutoSmooth);
				knot = new BezierKnot(new float3(edge.Node2.Point.x, 0f, edge.Node2.Point.y), new float3(),
					new float3());
				spline.Add(knot, TangentMode.AutoSmooth);
				return spline;
			}));
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var splinePort = GetOutputPort(nameof(Spline));
			var spline = (SplineContainerData)GetValue(splinePort);
			if(spline == null)
				return;

			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			SplinesGizmoUtility.DrawGizmos(spline, transform);
		}
#endif
	}
}