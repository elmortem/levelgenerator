using LevelGenerator.NodeGizmos;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator
{
	public abstract class PreviewNode : GenNode, IGizmosOptionsProvider
	{
		[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Inherited)] 
		public GizmosOptions GizmosOptions = new();
		
		private GizmosOptions _gizmosOptions;
		
		[HideInInspector]
		public bool ShowPreview = true;
		[HideInInspector]
		public bool LockCalc;
		//TODO support Editor for preview
		
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if (to.IsInput && to.fieldName == nameof(GizmosOptions))
				ResetGizmosOptions();
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if (port.IsInput && port.fieldName == nameof(GizmosOptions))
				ResetGizmosOptions();
		}

		public void ResetGizmosOptions()
		{
			_gizmosOptions = null;
		}
		
		public GizmosOptions GetGizmosOptions()
		{
			if (_gizmosOptions == null && GetPort(nameof(GizmosOptions)) != null)
			{
				_gizmosOptions = GetInputValue<GizmosOptions>(nameof(GizmosOptions), null);
			}
			
			if(GizmosOptions.Override)
				return GizmosOptions;

			if (_gizmosOptions == null)
			{
				foreach (var provider in this.GetNodeInParent<IGizmosOptionsProvider>())
				{
					_gizmosOptions = provider.GetGizmosOptions();
					break;
				}
			}
			
			return _gizmosOptions ?? GizmosOptions;
		}
		
		protected override void ApplyChange()
		{
			if (!LockCalc)
			{
				base.ApplyChange();
			}
		}

#if UNITY_EDITOR
		public virtual void DrawGizmos(Transform transform)
		{
		}
#endif
	}
}