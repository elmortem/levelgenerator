using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Splines.Utilities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines.Externals
{
	[MovedFrom("LevelGenerator.Splines")]
	public class GetSplineNode : PreviewCalcNode
	{
		public string Name = "Spline";
		public string Tag = "";
		[Output] public SplineContainerData Result;

		private string _lastKey;
		private List<SplineContainer> _sources;
		private SplineContainerData _result;

		protected override void Init()
		{
			base.Init();

			Spline.Changed += OnSplineChanged;
		}

		private void OnDisable()
		{
			Spline.Changed -= OnSplineChanged;
		}

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Result))
			{
				CalcResults();
				return _result;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			if (string.IsNullOrEmpty(Name))
				return;

			if (LockCalc && _result != null)
				return;
			if (!force && _sources != null && _sources.Count > 0 && _result != null && (!string.IsNullOrEmpty(Name) && _lastKey == Name || !string.IsNullOrEmpty(Tag) && _lastKey == Tag))
				return;

			if (!string.IsNullOrEmpty(Name))
			{
				var sources = GameObject.FindObjectsOfType<SplineContainer>().Where(p => p.name == Name).ToList();
				if (sources.Count <= 0)
					return;

				_sources = sources;
				_lastKey = Name;
			}
			else if (!string.IsNullOrEmpty(Tag))
			{
				var sources = GameObject.FindGameObjectsWithTag(Tag).Select(p => p.GetComponent<SplineContainer>()).Where(p => p != null).ToList();
				if (sources.Count <= 0)
					return;

				_sources = sources;
				_lastKey = Tag;
			}
			
			if(_sources == null)
				return;

			ResetGizmosOptions();

			if (_result == null)
				_result = new SplineContainerData();
			else
				_result.Splines.Clear();

			var transformedSplines = new List<Spline>();
			foreach (var source in _sources)
			{
				foreach (var spline in source.Splines)
				{
					var transformedSpline = new Spline();
					transformedSpline.Closed = spline.Closed;
					for (var i = 0; i < spline.Count; ++i)
					{
						var knot = spline[i];
						var transformedKnot = new BezierKnot(
							source.transform.TransformPoint(knot.Position),
							source.transform.TransformDirection(knot.TangentIn),
							source.transform.TransformDirection(knot.TangentOut),
							source.transform.rotation * knot.Rotation
						);
						transformedSpline.Add(transformedKnot, spline.GetTangentMode(i));
					}

					transformedSplines.Add(transformedSpline);
				}
			}

			_result.Splines = transformedSplines;
		}

		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification mod)
		{
			if (_sources == null || _sources.Count <= 0)
				return;

			if (!_sources.Any(p => p.Splines.Contains(spline)))
				return;

			ApplyChange();
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();

			var resultsPort = GetOutputPort(nameof(Result));
			var result = (SplineContainerData)GetValue(resultsPort);
			if (result == null)
				return;

			Gizmos.color = gizmosOptions.Color;
			SplinesGizmoUtility.DrawGizmos(result, transform);
		}
#endif
	}
}