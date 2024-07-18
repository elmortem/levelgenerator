using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Points;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Externals
{
	public class GetGameObjectsNode : PreviewCalcNode, INodePointCount
	{
		[Input] public string Name = "Point";
		[Input] public string Tag = "";
		[Output] public List<PointData> Points;

		private List<GameObject> _sources;
		private List<PointData> _points;
		
		public int PointsCount => _points?.Count ?? 0;
		
		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Points))
			{
				CalcResults();
				return _points;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			if (LockCalc && _points != null)
				return;
			if (!force && _sources != null && _sources.Count > 0 && _points != null)
				return;
			
			var gameObjectName = GetInputValue(nameof(Name), Name);
			var gameObjectTag = GetInputValue(nameof(Tag), Tag);

			if (string.IsNullOrEmpty(gameObjectName) && string.IsNullOrEmpty(gameObjectTag))
			{
				_points = null;
				return;
			}

			if (!string.IsNullOrEmpty(gameObjectName))
			{
				var sources = GameObject.FindObjectsOfType<GameObject>().Where(p => p.name == gameObjectName).ToList();
				if (sources.Count <= 0)
				{
					_points = null;
					return;
				}

				_sources = sources;
			}
			else if (!string.IsNullOrEmpty(gameObjectTag))
			{
				var sources = GameObject.FindGameObjectsWithTag(gameObjectTag).ToList();
				if (sources.Count <= 0)
				{
					_points = null;
					return;
				}

				_sources = sources;
			}

			if (_sources == null)
				return;

			ResetGizmosOptions();

			if (_points == null)
				_points = new();
			else
				_points.Clear();

			foreach (var source in _sources)
			{
				_points.Add(new PointData
				{
					Position = source.transform.position,
					Normal = source.transform.up,
					Scale = source.transform.localScale,
					Angle = source.transform.eulerAngles.z
				});
			}
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			
			var resultsPort = GetOutputPort(nameof(Points));
			var results = (List<PointData>)GetValue(resultsPort);
			if(results == null || results.Count <= 0)
				return;

			GizmosUtility.DrawPoints(results, gizmosOptions, transform);
		}
#endif
	}
}