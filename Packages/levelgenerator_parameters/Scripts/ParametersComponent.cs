using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Parameters.Types;
using SerializeReferenceEditor;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace LevelGenerator.Parameters
{
	public class ParametersComponent : MonoBehaviour
	{
		public LevelGeneratorSceneGraph SceneGraph;
		[SerializeReference, SR]
		public List<IParameter> Parameters = new();

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (SceneGraph == null)
				SceneGraph = GetComponent<LevelGeneratorSceneGraph>();

			if (SceneGraph.graph != null)
			{
				var parameterNodes = SceneGraph.graph.nodes.OfType<ParameterNode>();
				foreach (var parameterNode in parameterNodes)
				{
					if(parameterNode != null)
						parameterNode.AddParameters(Parameters);
				}
			}
		}
#endif
	}
}