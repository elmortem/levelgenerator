using UnityEngine;

namespace LevelGenerator.Vectors
{
	public class RandomVectorsNode : BaseVectorsNode
	{
		public Vector3 EulerMin = Vector3.zero;
		public Vector3 EulerMax = Vector3.zero;
		public Vector3 ScaleMin = Vector3.one;
		public Vector3 ScaleMax = Vector3.one;
		public bool LockScale = true;
		public int Seed = -1;

		private int _lastSeed = -1;

		private void Awake()
		{
			if (Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			if ((!force || LockCalc) && _lastSeed == Seed && _results != null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;
			
			if(_results == null)
				_results = new();
			else
				_results.Clear();

			_lastSeed = Seed;
			var state = Random.state;
			Random.InitState(_lastSeed);
			foreach (var points in pointsList)
			{
				if (points == null)
					continue;
				
				for (int i = 0; i < points.Count; i++)
				{
					var scale = new Vector3(Random.Range(ScaleMin.x, ScaleMax.x),
						Random.Range(ScaleMin.y, ScaleMax.y), Random.Range(ScaleMin.z, ScaleMax.z));
					if (LockScale)
					{
						scale.x = scale.y = scale.z = (scale.x + scale.y + scale.z) / 3f;
					}
					_results.Add(new VectorData
					{
						Point = points[i],
						Euler = new Vector3(Random.Range(EulerMin.x, EulerMax.x),
							Random.Range(EulerMin.y, EulerMax.y), Random.Range(EulerMin.z, EulerMax.z)),
						Scale = scale
					});
				}
			}

			Random.state = state;
		}
	}
}