using System.Collections.Generic;
using LevelGenerator.Vectors;
using UnityEngine;
using XNode;

namespace LevelGenerator.Instances
{
	public class SimpleInstancesNode : GenNode
	{
		[Input] public List<VectorData> Vectors;
		[Output] public List<InstanceData> Results = new();
		
		public GameObject Prefab;

		private List<InstanceData> _results = new();

		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if(to.IsInput)
				CalcResults(true);
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if(port.IsInput)
				CalcResults(true);
		}

		protected override void ApplyChange()
		{
			CalcResults(true);
			base.ApplyChange();
		}

		public override object GetValue(NodePort port)
		{
			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return _results;
			}
			
			return null;
		}

		private void CalcResults(bool force = false)
		{
			if(!force && _results.Count == Vectors.Count)
				return;
			
			var vectors = GetInputValue(nameof(Vectors), Vectors);
			if (Prefab == null)
				return;
			
			_results.Clear();
			for (int i = 0; i < vectors.Count; i++)
			{
				_results.Add(new InstanceData { Prefab = Prefab, Vector = vectors[i] });
			}
		}
	}
}