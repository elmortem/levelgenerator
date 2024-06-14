using System;
using System.Collections.Generic;
using LevelGenerator.Instancers;
using LevelGenerator.Instances;
using LevelGenerator.Splines;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.U2D;

namespace Instancers
{
	[ExecuteInEditMode]
	public class AdvancedInstancer : MonoBehaviour, IInstancer
	{
		public Transform Parent;
		public Vector3 SpriteShapeEuler = new Vector3(90f, 0f, 0f);
		
		public List<GameObject> Objects;
		
		public int ObjectsCount => Objects.Count;

		private void Update()
		{
			Debug.Log(Objects);
		}

		private void AddGameObject(GameObject prefab, Vector3 point, Quaternion rotation, Vector3 scale)
		{
			var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, Parent);
			go.transform.localPosition = point;
			go.transform.localRotation = rotation;
			go.transform.localScale = scale;
			Objects.Add(go);
		}

		private void AddSpriteShape(SpriteShapeInstanceData data)
		{
			foreach (var spline in data.SplineContainer.Splines)
			{
				var go = new GameObject(string.IsNullOrEmpty(data.Name) ? "SpriteShape" : data.Name);
				go.transform.parent = Parent;
				go.transform.localPosition = Vector3.zero;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.Euler(SpriteShapeEuler);
				
				var ssCon = go.AddComponent<SpriteShapeController>();
				ssCon.spriteShape = data.SpriteShape;

				Convert3DTo2D(spline, ssCon);
				
				ssCon.RefreshSpriteShape();
				ssCon.UpdateSpriteShapeParameters();

				Objects.Add(go);
			}
		}

		private void Convert3DTo2D(UnityEngine.Splines.Spline spline, SpriteShapeController controller)
		{
			var s = controller.spline;
			s.isOpenEnded = true;
			s.Clear();
			for (int i = 0; i < spline.Count; i++)
			{
				Vector3 pos = spline[i].Position;
				pos = pos.SwapYZ();
				s.InsertPointAt(i, pos);
				
				s.SetTangentMode(i, ShapeTangentMode.Continuous);
				s.SetCorner(i, true);

				if (i > 0 && i < spline.Count - 1)
				{
					Vector3 tangent = UnityEngine.Splines.SplineUtility.GetAutoSmoothTangent(spline[i - 1].Position, spline[i].Position,
						spline[i + 1].Position, UnityEngine.Splines.SplineUtility.CatmullRomTension);
					tangent = tangent.SwapYZ();
					s.SetRightTangent(i, tangent);
					s.SetLeftTangent(i, -tangent);
				}
			}
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
					else if (data is SpriteShapeInstanceData ssData)
					{
						AddSpriteShape(ssData);
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