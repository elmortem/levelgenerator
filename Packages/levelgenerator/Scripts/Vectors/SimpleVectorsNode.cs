using UnityEngine;

namespace LevelGenerator.Vectors
{
	public class SimpleVectorsNode : BaseVectorsNode
	{
		public Vector3 Euler = Vector3.forward;


		
		protected override void CalcResults(bool force = false)
		{
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
			{
				_results.Clear();
				return;
			}

			if (!force)
				return;
			
			_results.Clear();
			foreach (var points in pointsList)
			{
				for (int i = 0; i < points.Count; i++)
				{
					_results.Add(new VectorData { Point = points[i], Euler = Euler, Scale = Vector3.one });
				}
			}
		}
	}
}