using System;
using LevelGenerator.Bounds.Datas;
using LevelGenerator.NodeGizmos;
using LevelGenerator.Noises;
using UnityEngine;
using XNode;

namespace LevelGenerator.Bounds
{
	[Obsolete]
	public class NoiseBoundsNode : PreviewNode
	{
		[Input(connectionType = ConnectionType.Override)] public BoundData BoundData;
		[Input(connectionType = ConnectionType.Override), SerializeReference] public NoiseData NoiseData;
		
		public float MinValue = 0.5f;
		public float MaxValue = 1f;
		public bool Is2D = true;
		
		[Output] public BoundData Result;

		private NoiseBoundData _result;
		
		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if(to.IsInput)
				CalcResult(true);
		}

		public override void OnRemoveConnection(NodePort port)
		{
			if(port.IsInput)
				CalcResult(true);
		}

		protected override void ApplyChange()
		{
			CalcResult(true);
			base.ApplyChange();
		}

		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Result))
			{
				CalcResult();
				
				return _result;
			}

			return null;
		}

		private void CalcResult(bool force = false)
		{
			if(LockCalc && _result != null)
				return;
			if (!force && _result != null)
				return;

			if (_result == null)
				_result = new NoiseBoundData();
			_result.Data = GetInputValue(nameof(NoiseData), NoiseData);
			_result.BoundData = GetInputValue(nameof(BoundData), BoundData);
			_result.MinValue = MinValue;
			_result.MaxValue = MaxValue;
			_result.Is2D = Is2D;
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			if(_result == null)
				return;
			
			if(_result.Is2D && Mathf.Approximately(_result.BoundData.Max.y - _result.BoundData.Min.y, 0f))
				return;
			if(!_result.Is2D && Mathf.Approximately(_result.BoundData.Max.z - _result.BoundData.Min.z, 0f))
				return;
			
			var gizmosOptions = GetGizmosOptions();

			for (int i = 0; i < gizmosOptions.BoundPoints; i++)
			{
				for (int j = 0; j < gizmosOptions.BoundPoints; j++)
				{
					var pointY = _result.Is2D 
						? _result.BoundData.Min.y 
						  + j / 100f * (_result.BoundData.Max.y - _result.BoundData.Min.y) 
						: _result.BoundData.Min.z 
						  + j / 100f * (_result.BoundData.Max.z - _result.BoundData.Min.z);
					var point = new Vector2(
						_result.BoundData.Min.x + i / 100f * (_result.BoundData.Max.x - _result.BoundData.Min.x),
						pointY);
					var value = _result.Data.GetValue(point.x, point.y);
					if(value >= _result.MinValue && value <= _result.MaxValue)
						Gizmos.color = gizmosOptions.Color;
					else
					{
						if(!gizmosOptions.DrawIncorrects)
							continue;
						Gizmos.color = Color.red;
					}

					var height = _result.Is2D
						? new Vector3(point.x, point.y, value * gizmosOptions.NoiseHeight)
						: new Vector3(point.x, value * gizmosOptions.NoiseHeight, point.y);
					Gizmos.DrawCube(transform.position + height, new Vector3(0.2f, 0.2f, 0.2f));
				}
			}
		}
#endif
	}
}