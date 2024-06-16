using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Points;
using LevelGenerator.Surfaces.Datas;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Surfaces
{
	public class TerrainSurfaceNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		public TerrainSurfaceData Terrain;
		public SurfacePointMode PointMode;
		public int Count = 100;
		public int Seed = -1;
		[Header("Gizmos")]
		public GizmosOptions GizmosOptions;
		
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
			
			Terrain.GetPoints(_results, PointMode, Count, Seed);
		}

		public GizmosOptions GetGizmosOptions() => GizmosOptions;

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var resultsPort = GetOutputPort(nameof(Results));
			var results = (List<PointData>)GetValue(resultsPort);
			if (results == null || results.Count <= 0)
				return;
			
			GizmosUtility.DrawPoints(results, GizmosOptions.PointSize, transform, GizmosOptions);
		}
#endif
	}
}