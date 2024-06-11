using System;
using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Instancers;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace LevelGenerator
{
	[ExecuteInEditMode]
	public class LevelGeneratorSceneGraph : SceneGraph<LevelGeneratorGraph>
	{
#if UNITY_EDITOR
		public LevelInstancer LevelInstancer;
		public bool ShowGizmosOnObjects;
		
		[HideInInspector]
		public ResultNode ResultNode;
		
		private bool _needGenerate;
		private float _timer;
		private int _drawGizmosCount;
		private List<PreviewNode> _previewNodeDrawed = new();

		private void Awake()
		{
			if(Application.isPlaying)
				return;
			
			UpdateResultNode();
			UpdateInstancer();
		}

		private void Update()
		{
			if (ResultNode != null && ResultNode.AutoGenerate)
			{
				if (ResultNode.Changed)
				{
					_needGenerate = true;
					ResultNode.Changed = false;
					_timer = 0.2f;
				}
				
				if (_needGenerate)
				{
					_timer -= Time.deltaTime;

					if (_timer <= 0f)
					{
						_needGenerate = false;
						ResultNode.Generate();
					}
				}
			}
		}

		private void UpdateResultNode()
		{
			if(graph == null)
				return;

			ResultNode = (ResultNode)graph.nodes.Find(p => p is ResultNode);
			ResultNode.SetInstancer(LevelInstancer);
		}

		private void UpdateInstancer()
		{
			/*if(graph == null)
				return;
			
			foreach (var instantiateNode in graph.nodes.Where(p => p is IInstancerOwner).Cast<IInstancerOwner>())
			{
				instantiateNode.SetInstancer(LevelInstancer);
			}*/
		}

		private void OnDrawGizmos()
		{
			if(graph == null)
				return;
			
			if(!ShowGizmosOnObjects && ResultNode != null && ResultNode.ObjectsCount > 0)
				return;

			/*_drawGizmosCount = 0;
			_previewNodeDrawed.Clear();
			var endNodes = graph.nodes.Where(p => !p.Outputs.Any(p1 => p1.IsConnected));
			foreach (var node in endNodes)
			{
				DrawNodeGizmos(node);
			}*/
			
			foreach (var node in graph.nodes)
			{
				if(node is PreviewNode customNode)
					if(customNode.ShowPreview)
						customNode.DrawGizmos(transform);
			}
		}

		private void DrawNodeGizmos(Node node)
		{
			if(_drawGizmosCount > 10)
				return;
			if(_previewNodeDrawed.Contains(node))
				return;
			
			_drawGizmosCount++;

			if (node is PreviewNode previewNode && previewNode.ShowPreview)
			{
				_previewNodeDrawed.Add(previewNode);
				previewNode.DrawGizmos(transform);
			}
			else
			{
				foreach (var inputPort in node.Inputs)
				{
					if (inputPort != null && inputPort.IsConnected)
					{
						foreach (var connection in inputPort.GetConnections())
						{
							DrawNodeGizmos(connection.node);
						}
					}
				}
			}
		}

		private void OnValidate()
		{
			if(Application.isPlaying)
				return;

			if (LevelInstancer == null)
				LevelInstancer = GetComponent<LevelInstancer>();
			
			UpdateResultNode();
			UpdateInstancer();
		}
#endif
	}
}