using System.Collections.Generic;
using LevelGenerator.Instancers;
using LevelGenerator.Instances;
using XNode;

namespace LevelGenerator
{
	public class ResultNode : GenNode, IInstancerOwner
	{
		[Input] public List<InstanceData> Instances = new();

		public bool AutoGenerate = true;
		
		private bool _changed = false;
		private IInstancer _instancer;

		public int ObjectsCount => _instancer?.ObjectsCount ?? 0;
		
		public bool Changed
		{
			get => _changed;
			set => _changed = value;
		}

		public void SetInstancer(IInstancer instancer)
		{
			_instancer = instancer;
		}

		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if (to.IsInput)
				_changed = true;
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if (port.IsInput)
				_changed = true;
		}

		protected override void ApplyChange()
		{
			_changed = true;
		}

		public void Generate()
		{
			if (_instancer != null)
			{
				Clear();
				var instancesList = GetInputValues<List<InstanceData>>(nameof(Instances), null);
				foreach (var instances in instancesList)
				{
					_instancer.AddInstances(instances);
				}
				_instancer.RaiseChange();

				if (AutoGenerate && _instancer.ObjectsCount > 10000)
					AutoGenerate = false;

				_changed = false;
			}
		}

		public void Clear()
		{
			if (_instancer != null)
			{
				_instancer.RemoveAll();
				_instancer.RaiseChange();
			}
		}
	}
}