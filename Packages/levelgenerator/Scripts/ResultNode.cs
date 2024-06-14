using System.Collections.Generic;
using System.Linq;
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
		private IEnumerable<IInstancer> _instancers;

		public int ObjectsCount => _instancers?.Sum(p => p.ObjectsCount) ?? 0;
		
		public bool Changed
		{
			get => _changed;
			set => _changed = value;
		}

		public void SetInstancers(IEnumerable<IInstancer> instancers)
		{
			_instancers = instancers;
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
			if (_instancers != null)
			{
				Clear();
				var instancesList = GetInputValues<object>(nameof(Instances), null);
				foreach (var instances in instancesList)
				{
					if (instances is IEnumerable<object> enumerable)
					{
						foreach (var instancer in _instancers)
						{
							if(instancer.TryAddInstances(enumerable.Cast<InstanceData>()))
								break;
						}
					}
				}

				foreach (var instancer in _instancers)
				{
					instancer.RaiseChange();
				}

				if (AutoGenerate && ObjectsCount > 10000)
					AutoGenerate = false;

				_changed = false;
			}
		}

		public void Clear()
		{
			if (_instancers != null)
			{
				foreach (var instancer in _instancers)
				{
					instancer.RemoveAll();
					instancer.RaiseChange();
				}
			}
		}
	}
}