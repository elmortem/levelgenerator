using System.Collections.Generic;
using LevelGenerator.Points;
using UnityEngine;
using XNode;

namespace LevelGenerator.Instances
{
	public class GameObjectWeightsNode : PreviewCalcNode
	{
		[Input] public List<PointData> Points;
		[Input] public List<GameObjectWeight> Weights = new();
		public int Seed = -1;
		public bool Enabled = true;
		[Output] public List<GameObjectInstanceData> Results = new();

		private List<GameObjectWeight> _lastWeights;
		private int _lastSeed;
		private List<GameObjectInstanceData> _results;

		private void Awake()
		{
			if(Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}
		
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
			
			var weights = GetInputValue(nameof(Weights), Weights);
			if (weights == null || weights.Count <= 0)
			{
				_results = null;
				return;
			}

			if(LockCalc && _results != null)
				return;
			if(!force && _results != null && _lastWeights == weights && _lastSeed == Seed)
				return;
			
			if(!Enabled)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if(pointsList == null || pointsList.Length <= 0)
				return;
			
			_lastWeights = weights;
			_lastSeed = Seed;

			var state = Random.state;
			Random.InitState(Seed);

			if (_results == null)
				_results = new();
			else
				_results.Clear();
			
			var weightsSum = 0f;
			foreach (var weight in weights)
			{
				weightsSum += weight.Weight;
			}
			
			foreach (var points in pointsList)
			{
				if(points == null)
					continue;
				
				foreach (var point in points)
				{
					var rand = Random.Range(0f, weightsSum);
					foreach (var weight in weights)
					{
						rand -= weight.Weight;
						if (rand < 0)
						{
							_results.Add(new GameObjectInstanceData { Prefab = weight.Prefab, Point = point });
							break;
						}
					}
				}
			}
			
			Random.state = state;
		}
	}
}