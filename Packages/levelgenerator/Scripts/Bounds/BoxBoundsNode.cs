using System;
using LevelGenerator.Bounds.Datas;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Bounds
{
	[Obsolete]
	public class BoxBoundsNode : PreviewNode, IGizmosOptionsProvider
	{
		public UnityEngine.Bounds Bounds;
		[Header("Gizmos")] 
		public GizmosOptions GizmosOptions;
		[Output(typeConstraint = TypeConstraint.Inherited)] public BoundData Result;

		private BoxBoundData _result;
		
		public GizmosOptions GetGizmosOptions() => GizmosOptions;
		
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
			Gizmos.color = GizmosOptions.Color;
			Gizmos.DrawWireCube(transform.TransformPoint(Bounds.center), Bounds.size);
		}
#endif
	}
}