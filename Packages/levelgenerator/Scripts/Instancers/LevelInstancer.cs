using System.Collections.Generic;
using LevelGenerator.Instances;
using UnityEngine;

namespace LevelGenerator.Instancers
{
	public class LevelInstancer : MonoBehaviour, IInstancer
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

		public void AddInstances(List<InstanceData> instances)
		{
			if (instances != null)
			{
				foreach (var data in instances)
				{
					if (data is GameObjectInstanceData goData)
					{
						AddGameObject(goData.Prefab, goData.Vector.Point, Quaternion.Euler(goData.Vector.Euler),
							goData.Vector.Scale);
					}
				}
			}
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
	}
}