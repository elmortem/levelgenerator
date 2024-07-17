using System;
using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	[Obsolete("Use CombinePointsNode instead")]
	public class OldCombinePointsNode : PreviewCalcNode, INodePointCount
	{
		[Input] public List<Vector3> Points;
		[Output] public List<Vector3> Results;
		
		private List<Vector3> _results;

		public int PointsCount => _results?.Count ?? 0;
		
		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return _results;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			if(LockCalc && _results != null)
				return;
			if (!force && _results != null)
				return;

			if (_results == null)
				_results = new();
			else
				_results.Clear();

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;

			foreach (var points in pointsList)
			{
				_results.AddRange(points);
			}
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<Vector3>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			var pos = transform.position;
			
			Gizmos.color = gizmosOptions.Color;
			var maxCount = Mathf.Min(10000, results.Count);
			for (int i = 0; i < maxCount; i++)
			{
				var point = results[i];
				Gizmos.DrawSphere(pos + point, gizmosOptions.PointSize);
			}
		}
#endif
	}
}