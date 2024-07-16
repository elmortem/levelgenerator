using System;
using LevelGenerator.Bounds.Datas;
using LevelGenerator.NodeGizmos;
using UnityEngine;
using XNode;

namespace LevelGenerator.Bounds
{
	[Obsolete]
	public class BoxBoundsNode : PreviewNode
	{
		public UnityEngine.Bounds Bounds;
		[Output(typeConstraint = TypeConstraint.Inherited)] public BoundData Result;

		private BoxBoundData _result;
		
		public override object GetValue(NodePort port) 
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Result))
			{
				if (_result == null)
					_result = new BoxBoundData();
				_result.Bounds = Bounds;
				return _result;
			}
			
			return null;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();
			Gizmos.color = gizmosOptions.Color;
			Gizmos.DrawWireCube(transform.TransformPoint(Bounds.center), Bounds.size);
		}
#endif
	}
}