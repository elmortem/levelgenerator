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

			if (instanceDatas.First() is OldGameObjectInstanceData)
			{
				foreach (var data in instanceDatas)
				{
					if (data is OldGameObjectInstanceData goData)
					{
						AddGameObject(goData.Prefab, goData.Vector.Point, Quaternion.Euler(goData.Vector.Euler),
							goData.Vector.Scale);
					}
				}

				return true;
			}
			
			if (instanceDatas.First() is GameObjectInstanceData)
			{
				foreach (var data in instanceDatas)
				{
					if (data is GameObjectInstanceData goData)
					{
						var normal = goData.Point.Normal;
						var angleY = goData.Point.Angle;

						Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
						Quaternion yRotation = Quaternion.Euler(0, angleY, 0);
						rot *= yRotation;
						
						AddGameObject(goData.Prefab, goData.Point.Position, rot, goData.Point.Scale);
					}
				}

				return true;
			}

			return false;
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