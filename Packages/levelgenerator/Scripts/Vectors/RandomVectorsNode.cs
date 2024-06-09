using UnityEngine;

namespace LevelGenerator.Vectors
{
	public class RandomVectorsNode : BaseVectorsNode
	{
		public Vector3 EulerMin = Vector3.zero;
		public Vector3 EulerMax = Vector3.zero;
		public int Seed = -1;

		private int _lastSeed = -1;

		private void Awake()
		{
			if (Seed == -1)
				Seed = Random.Range(1, int.MaxValue);
		}

		protected override void CalcResults(bool force = false)
		{
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
			{
				_results.Clear();
				return;
			}
			
			if (!force && _lastSeed == Seed)
				return;
			
			_results.Clear();

			_lastSeed = Seed;
			var state = Random.state;
			Random.InitState(_lastSeed);
			foreach (var points in pointsList)
			{
				for (int i = 0; i < points.Count; i++)
				{
					_results.Add(new VectorData
					{
						Point = points[i],
						Euler = new Vector3(Random.Range(EulerMin.x, EulerMax.x),
							Random.Range(EulerMin.y, EulerMax.y), Random.Range(EulerMin.z, EulerMax.z)),
						Scale = Vector3.one
					});
				}
			}

			Random.state = state;
		}
	}
}