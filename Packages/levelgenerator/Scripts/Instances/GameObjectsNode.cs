using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Vectors;
using UnityEngine;
using XNode;

namespace LevelGenerator.Instances
{
	public class GameObjectsNode : PreviewCalcNode
	{
		[Input] public List<PointData> Points;
		[Input] public GameObject Prefab;
		public bool Enabled = true;
		[Output] public List<GameObjectInstanceData> Results = new();

		private GameObject _lastPrefab;
		private List<GameObjectInstanceData> _results;
		
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
			if (!Enabled)
			{
				_results = null;
				return;
			}
			
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			var prefab = GetInputValue(nameof(Prefab), Prefab);
			if (Prefab == null)
			{
				_results = null;
				return;
			}
			
			if(LockCalc && _results != null)
				return;
			if(!force && _results != null && _lastPrefab == prefab)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if(pointsList == null || pointsList.Length <= 0)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();

			_lastPrefab = prefab;

			foreach (var points in pointsList)
			{
				if(points == null)
					continue;
				
				foreach (var point in points)
				{
					_results.Add(new GameObjectInstanceData { Prefab = prefab, Point = point });
				}
			}
		}
	}
}