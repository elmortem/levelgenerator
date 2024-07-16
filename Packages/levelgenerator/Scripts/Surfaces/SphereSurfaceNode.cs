using System.Collections.Generic;
using LevelGenerator.Points;
using LevelGenerator.Surfaces.Datas;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Surfaces
{
	public class SphereSurfaceNode : PreviewCalcNode
	{
		public SphereSurfaceData Sphere;
		[NodeEnum]
		public SurfacePointMode PointMode;
		public int Count = 100;
		public int Seed = -1;
		[Output] public List<PointData> Results;

		private SurfacePointMode _lastPointMode;
		private int _lastCount;
		private int _lastSeed;
		private List<PointData> _results;
		
		private void Awake()
		{
			if (Seed == -1)
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
			if(LockCalc && _results != null)
				return;
			if(!force && _lastPointMode == PointMode && _lastCount == Count && _lastSeed == Seed && _results != null)
				return;
			
			_lastPointMode = PointMode;
			_lastCount = Count;
			_lastSeed = Seed;

			if(_results == null)
				_results = new();
			else
				_results.Clear();
			
			Sphere.GetPoints(_results, PointMode, Count, Seed);
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			Gizmos.color = GizmosOptions.Color;
			Gizmos.DrawWireSphere(transform.position + Sphere.Offset, Sphere.Radius);
			
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<PointData>)GetValue(resultsPort);
			if (results == null || results.Count <= 0)
				return;
			
			GizmosUtility.DrawPoints(results, GizmosOptions, transform);
		}
#endif
	}
}