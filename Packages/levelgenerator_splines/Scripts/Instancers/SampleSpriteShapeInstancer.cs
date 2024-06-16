using System;
using System.Collections.Generic;
using System.Linq;
using LevelGenerator.Instancers;
using LevelGenerator.Instances;
using LevelGenerator.Utility;
using UnityEngine;
using UnityEngine.U2D;

namespace LevelGenerator.Splines.Instancers
{
	[ExecuteInEditMode]
	public class SampleSpriteShapeInstancer : MonoBehaviour, IInstancer
	{
		public Transform Parent;
		public Vector3 SpriteShapeEuler = new Vector3(90f, 0f, 0f);
		
		public List<GameObject> Objects;
		
		public int ObjectsCount => Objects.Count;

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

				Convert3DTo2D(spline, data.Height, ssCon);
				
				ssCon.RefreshSpriteShape();
				ssCon.UpdateSpriteShapeParameters();

				Objects.Add(go);
			}
		}

		private void Convert3DTo2D(UnityEngine.Splines.Spline spline, float height, SpriteShapeController controller)
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
				s.SetHeight(i, height);

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
		
		public bool TryAddInstances(IEnumerable<InstanceData> instances)
		{
			if (instances == null)
				return true;
			
			var instanceDatas = instances.ToList();
			
			if (!instanceDatas.Any())
				return true;

			if (instanceDatas.First() is not SpriteShapeInstanceData)
				return false;
			
			var spriteShapes = instanceDatas.Cast<SpriteShapeInstanceData>();
			foreach (var data in spriteShapes)
			{
				AddSpriteShape(data);
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