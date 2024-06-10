using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public class PercentPointsNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		[Input] public List<Vector3> Points = new();
		public float Percent = 0.5f;
		[Output] public List<Vector3> Results = new();

		private float _lastPercent = -1;
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
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			if ((!force || LockCalc) && Mathf.Approximately(_lastPercent, Percent) && _results != null)
				return;

			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();

			_lastPercent = Percent;

			foreach (var points in pointsList)
			{
				var count = Mathf.RoundToInt(points.Count * Percent);
				for (int i = 0; i < count; i++)
				{
					_results.Add(points[i]);
				}
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