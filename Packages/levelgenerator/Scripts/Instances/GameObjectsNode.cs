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
		public bool Enabled = true;
		[Output] public List<GameObjectInstanceData> Results = new();
		
		public GameObject Prefab;

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
			if(LockCalc && _results != null)
				return;
			if(!force && _results != null)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();
			
			if(!Enabled)
				return;
			
			if (Prefab == null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if(pointsList == null || pointsList.Length <= 0)
				return;

			foreach (var points in pointsList)
			{
				foreach (var point in points)
				{
					_results.Add(new GameObjectInstanceData { Prefab = Prefab, Point = point });
				}
			}
		}
	}
}