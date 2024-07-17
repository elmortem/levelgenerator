using System;
using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Splines.Utilities;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines
{
	public class GetSplineNode : PreviewCalcNode
	{
		public string Name = "Spline";
		[Output] public SplineContainerData Result;

		private string _lastName;
		private SplineContainer _source;
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
			if (!force && _source != null && _result != null && _lastName == Name)
				return;

			var go = GameObject.Find(Name);
			if (go == null)
				return;

			_source = go.GetComponent<SplineContainer>();

			if (_source == null)
				return;

			ResetGizmosOptions();
			
			_lastName = Name;

			if (_result == null)
				_result = new SplineContainerData();
			else
				_result.Splines.Clear();

			var transformedSplines = new List<Spline>();
			foreach (var spline in _source.Splines)
			{
				var transformedSpline = new Spline();
				transformedSpline.Closed = spline.Closed;
				for (var i = 0; i < spline.Count; ++i)
				{
					var knot = spline[i];
					var transformedKnot = new BezierKnot(
						_source.transform.TransformPoint(knot.Position),
						_source.transform.TransformDirection(knot.TangentIn),
						_source.transform.TransformDirection(knot.TangentOut),
						_source.transform.rotation * knot.Rotation
					);
					transformedSpline.Add(transformedKnot, spline.GetTangentMode(i));
				}

				transformedSplines.Add(transformedSpline);
			}

			_result.Splines = transformedSplines;
		}

		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification mod)
		{
			if (_source == null)
				return;

			if (!_source.Splines.Contains(spline))
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