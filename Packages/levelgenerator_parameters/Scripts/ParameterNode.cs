using System;
using System.Collections.Generic;
using LevelGenerator.Parameters.Types;
using UnityEngine;
using XNode;

namespace LevelGenerator.Parameters
{
	[Serializable]
	public class PortItem
	{
		public Node Node;
		public string PortName;
	}
	
	public class ParameterNode : PreviewNode, INodeInfo
	{
		public string Name;
		
		[SerializeReference, HideInInspector]
		public IParameter _parameter;
		
		private NodePort _paramPort;
		
		public bool HasNodeInfo() => _parameter != null;

		public string GetNodeInfo() => $"Value: {_parameter?.GetValue()}";
		
		public void AddParameters(List<IParameter> parameters)
		{
			IParameter parameter = null;
			if (parameters != null && parameters.Count > 0)
			{
				parameter = parameters.Find(p => p != null && p.GetName() == Name);
				if (parameter == _parameter)
				{
					ApplyChange();
					return;
				}
			}

			//TODO need optimize

			List<PortItem> inputPorts = null;
			if (_paramPort != null)
			{
				inputPorts = new();
				try
				{
					foreach (var port in _paramPort.GetConnections())
					{
						inputPorts.Add(new PortItem { Node = port.node, PortName = port.fieldName });
					}
				}
				catch (Exception)
				{
					// ignored
				}

				RemoveDynamicPort(_paramPort);
				_paramPort = null;
			}
			
			_parameter = null;
			
			if (parameter != null)
			{
				_parameter = parameter;
				_paramPort = AddDynamicOutput(parameter.GetParameterType(), ConnectionType.Multiple, TypeConstraint.Inherited, Name);

				if (inputPorts != null)
				{
					foreach (var portItem in inputPorts)
					{
						portItem.Node.GetInputPort(portItem.PortName)?.Connect(_paramPort);
					}
				}
			}
		}

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;

			if (_parameter != null && port.fieldName == _parameter.GetName())
			{
				return _parameter.GetValue();
			}

			return null;
		}
	}
}