using System;
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
		public ResultNode ResultNode;
		
		private bool _needGenerate;
		private float _timer;

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
		}

		private void UpdateInstancer()
		{
			if(graph == null)
				return;
			
			foreach (var instantiateNode in graph.nodes.Where(p => p is IInstancerOwner).Cast<IInstancerOwner>())
			{
				instantiateNode.SetInstancer(LevelInstancer);
			}
		}

		private void OnDrawGizmos()
		{
			if(graph == null)
				return;
			
			foreach (var node in graph.nodes)
			{
				if(node is PreviewNode customNode)
					if(customNode.ShowPreview)
						customNode.DrawGizmos(transform);
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