#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Splines;

namespace LevelGenerator.Splines.Utilities
{
	public static class SplinesCache
	{
		private static Dictionary<Spline, Vector3[]> _cachedPositions = new Dictionary<Spline, Vector3[]>();

		[InitializeOnLoadMethod]
		static void Initialize()
		{
			Spline.Changed += ClearCache;
			Undo.undoRedoPerformed += ClearAllCache;
			PrefabStage.prefabStageClosing += _ => ClearAllCache();
			PrefabUtility.prefabInstanceReverting += _ => ClearAllCache();
		}

		public static void GetCachedPositions(Spline spline, int segments, out Vector3[] positions)
		{
			if (spline == null)
			{
				positions = null;
				return;
			}
			
			int count = spline.Closed ? spline.Count : spline.Count - 1;
			if (segments <= 1)
				segments = 32;
			
			if (!_cachedPositions.TryGetValue(spline, out positions))
			{
				positions = new Vector3[count * segments];
				
				float inv = 1f / (segments - 1);
				for(int i = 0; i < count; ++i)
				{
					var curve = spline.GetCurve(i);
					var startIndex = i * segments;
					for(int n = 0; n < segments; n++)
						positions[startIndex + n] = CurveUtility.EvaluatePosition(curve, n * inv);
				}

				_cachedPositions[spline] = positions;
			}
		}
		
		public static void ClearAllCache()
		{
			_cachedPositions.Clear();
		}
		
		private static void ClearCache(Spline spline, int index, SplineModification modification)
		{
			_cachedPositions.Remove(spline);
		}
	}
}
#endif