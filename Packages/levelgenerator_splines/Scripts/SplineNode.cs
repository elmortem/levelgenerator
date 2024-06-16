using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Splines.Utilities;
using UnityEngine;
using UnityEngine.Splines;
using XNode;

namespace LevelGenerator.Splines
{
	public class SplineNode : PreviewCalcNode, IGizmosOptionsProvider
	{
		public GizmosOptions GizmosOptions;
		[Output] public SplineContainerData Result;
		
		private Transform _editContainer;
		
		public GizmosOptions GetGizmosOptions() => GizmosOptions;

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (port.fieldName == nameof(Result))
			{
				return Result;
			}

			return null;
		}

		public void SetData(IReadOnlyList<Spline> splines, KnotLinkCollection knots)
		{
			if(Result == null)
				Result = new();
			
			Result.Splines = new List<Spline>(splines);
			Result.Knots = knots;
		}
		
		public void SetEditContainer(Transform container)
		{
			_editContainer = container;
		}

		protected override void CalcResults(bool force = false)
		{
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			if (_editContainer != null)
			{
				_editContainer.transform.position = transform.position;
				_editContainer.transform.rotation = transform.rotation;
				_editContainer.transform.localScale = transform.localScale;
			}
			else
			{
				var resultsPort = GetOutputPort(nameof(Result));
				var result = (SplineContainerData)GetValue(resultsPort);
				if(result == null)
					return;
				
				Gizmos.color = GizmosOptions.Color;
				SplinesGizmoUtility.DrawGizmos(result, transform);
			}
		}
#endif
	}
}