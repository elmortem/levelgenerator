using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class CombinePointsNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		[Input] public List<Vector3> Points;
		[Output] public List<Vector3> Results;
		
		private List<Vector3> _results;
		private GizmosOptions _gizmosOptions;

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
			if ((!force || LockCalc) && _results != null)
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
		
		private void UpdateGizmosOptions()
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

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<Vector3>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			var pos = transform.position;
			
			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			var maxCount = Mathf.Min(10000, results.Count);
			for (int i = 0; i < maxCount; i++)
			{
				var point = results[i];
				Gizmos.DrawSphere(pos + point, _gizmosOptions?.PointSize ?? 0.2f);
			}
		}
#endif
	}
}