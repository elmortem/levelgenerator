using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Instances;
using UnityEngine;

namespace LevelGenerator.Instancers
{
	public class SampleGameObjectInstancer : MonoBehaviour, IInstancer
	{
		public Transform Parent;

		public List<GameObject> Objects = new();

		public int ObjectsCount => Objects.Count;

		private void AddGameObject(GameObject prefab, Vector3 point, Quaternion rotation, Vector3 scale)
		{
			var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, Parent);
			go.transform.localPosition = point;
			go.transform.localRotation = rotation;
			go.transform.localScale = scale;
			Objects.Add(go);
		}

		public bool TryAddInstances(IEnumerable<InstanceData> instances)
		{
			if (instances == null)
				return true;
			
			var instanceDatas = instances.ToList();
			if (!instanceDatas.Any())
				return true;
			
			if (instanceDatas.First() is not GameObjectInstanceData)
				return false;
			
			foreach (var data in instanceDatas)
			{
				if (data is GameObjectInstanceData goData)
				{
					AddGameObject(goData.Prefab, goData.Vector.Point, Quaternion.Euler(goData.Vector.Euler),
						goData.Vector.Scale);
				}
			}

			return true;
		}

		public void RemoveAll()
		{
			foreach (var go in Objects)
			{
				GameObject.DestroyImmediate(go);
			}
			Objects.Clear();
		}

		public void RaiseChange()
		{
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
		}
		
#if UNITY_EDITOR
		private void OnValidate()
		{
			if (Parent == null)
				Parent = transform;
		}
#endif
	}
}