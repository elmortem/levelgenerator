using System;
using UnityEngine;

namespace LevelGenerator.Vectors
{
	[Obsolete]
	public class SimpleVectorsNode : BaseVectorsNode
	{
		public Vector3 Euler = Vector3.forward;
		public Vector3 Scale = Vector3.one;

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_results = null;
				return;
			}
			
			if(LockCalc && _results != null)
				return;
			if (!force && _results != null)
				return;
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if (pointsList == null || pointsList.Length == 0)
				return;

			if (_results == null)
				_results = new();
			else
				_results.Clear();
			
			foreach (var points in pointsList)
			{
				for (int i = 0; i < points.Count; i++)
				{
					_results.Add(new VectorData { Point = points[i], Euler = Euler, Scale = Scale });
				}
			}
		}
	}
}