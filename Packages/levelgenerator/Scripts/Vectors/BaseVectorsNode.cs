using System;
using System.Collections.Generic;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Vectors
{
	[Obsolete]
	public abstract class BaseVectorsNode : PreviewNode
	{
		[Input(typeConstraint = TypeConstraint.Inherited)] public List<Vector3> Points;
		[Output] public List<VectorData> Results;

		protected List<VectorData> _results;
		private GizmosOptions _gizmosOptions;
		
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if(to.IsInput)
				MainCalcResults(true);
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if(port.IsInput)
				MainCalcResults(true);
		}

		protected override void ApplyChange()
		{
			MainCalcResults(true);
			base.ApplyChange();
		}
		
		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Results))
			{
				MainCalcResults();
				return _results;
			}

			return null;
		}

		private void MainCalcResults(bool force = false)
		{
			if((!force || LockCalc) && _results != null)
				return;

			_gizmosOptions = null;
			CalcResults(force);
		}

		protected abstract void CalcResults(bool force = false);

		private void UpdateGizmosOptions()
		{
			if (_gizmosOptions == null)
			{
				foreach (var provider in this.GetNodeInParent<IGizmosOptionsProvider>())
				{
					_gizmosOptions = provider.GetGizmosOptions();
					break;
				}
			}
		}
		
#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			UpdateGizmosOptions();
			
			var pointsPort = GetOutputPort(nameof(Results));
			var vectors = (List<VectorData>)GetValue(pointsPort);

			var pos = transform.position;
			
			Gizmos.color = _gizmosOptions?.Color ?? Color.white;
			foreach (var vec in vectors)
			{
				var q = Quaternion.Euler(vec.Euler);
				var direction = q * Vector3.up;
				Gizmos.DrawLine(pos + vec.Point, pos + vec.Point + direction * 2f);
			}
		}
#endif
	}
}