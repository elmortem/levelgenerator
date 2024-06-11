using System.Collections.Generic;
using LevelGenerator.Vectors;
using UnityEngine;
using XNode;

namespace LevelGenerator.Instances
{
	public class SimpleInstancesNode : PreviewNode
	{
		[Input] public List<VectorData> Vectors;
		public bool Enabled = true;
		[Output] public List<InstanceData> Results = new();
		
		public GameObject Prefab;

		private List<InstanceData> _results;

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
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Results))
			{
				CalcResults();
				return _results;
			}
			
			return null;
		}

		private void CalcResults(bool force = false)
		{
			if(LockCalc && _results != null)
				return;
			if(!force && _results != null)
				return;
			
			if (_results == null)
				_results = new();
			else
				_results.Clear();
			
			if(!Enabled)
				return;
			
			if (Prefab == null)
				return;
			
			var vectors = GetInputValue(nameof(Vectors), Vectors);
			for (int i = 0; i < vectors.Count; i++)
			{
				_results.Add(new InstanceData { Prefab = Prefab, Vector = vectors[i] });
			}
		}
	}
}