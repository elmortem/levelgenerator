using System.Collections.Generic;
using LevelGenerator.Noises;
using LevelGenerator.Utility;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	public enum NoiseAxesMode
	{
		XZ,
		XY
	}

	public class SplitPointsByNoiseNode : PreviewCalcNode
	{
		[Input] public List<PointData> Points = new();
		[Input(connectionType = ConnectionType.Override)] public NoiseData Noise = new();
		public NoiseAxesMode NoiseAxes = NoiseAxesMode.XZ;
		public float MinValue = 0.5f;
		public float MaxValue = 1f;
		[Header("Gizmos")]
		public bool ShowInsidePoints = true;
		public bool ShowNoise;
		[Output] public List<PointData> InsidePoints = new();
		[Output] public List<PointData> OutsidePoints = new();
		
		private NoiseData _lastNoise;
		private NoiseAxesMode _lastNoiseAxes;
		private float _lastMinValue;
		private float _lastMaxValue;
		private List<PointData> _insidePoints;
		private List<PointData> _outsidePoints;
		private Rect _noiseSize;
		
		public int PointsCount => ShowInsidePoints ? _insidePoints?.Count ?? 0 : _outsidePoints?.Count ?? 0;
		

		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;
			
			if (port.fieldName == nameof(InsidePoints))
			{
				CalcResults();
				return _insidePoints ?? InsidePoints;
			}
			
			if (port.fieldName == nameof(OutsidePoints))
			{
				CalcResults();
				return _outsidePoints ?? OutsidePoints;
			}
			
			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Points));
			if (port == null || !port.IsConnected)
			{
				_insidePoints = null;
				_outsidePoints = null;
				return;
			}
			
			port = GetInputPort(nameof(Noise));
			if (port == null || !port.IsConnected)
			{
				_insidePoints = null;
				_outsidePoints = null;
				return;
			}
			
			if(LockCalc && _insidePoints != null)
				return;
			if(!force && _lastNoise == Noise && _lastNoiseAxes == NoiseAxes && Mathf.Approximately(_lastMinValue, MinValue) && Mathf.Approximately(_lastMaxValue, MaxValue) && _insidePoints != null)
				return;

			var pointsList = GetInputValues(nameof(Points), Points);
			if(pointsList == null || pointsList.Length <= 0)
				return;
			
			var noise = GetInputValue(nameof(Noise), Noise);
            if(noise == null)
				return;
			
			if (_insidePoints == null)
				_insidePoints = new();
			else
				_insidePoints.Clear();
			
			if (_outsidePoints == null)
				_outsidePoints = new();
			else
				_outsidePoints.Clear();

			_noiseSize = CalculateNoiseSize(pointsList);

			ResetGizmosOptions();
			
			_lastNoise = Noise;
			_lastNoiseAxes = NoiseAxes;
			_lastMinValue = MinValue;
			_lastMaxValue = MaxValue;

			foreach (var points in pointsList)
			{
				if(points == null)
					continue;
				
				foreach (var point in points)
				{
					var value = NoiseAxes == NoiseAxesMode.XY
						? noise.GetValue(point.Position.x, point.Position.y)
						: noise.GetValue(point.Position.x, point.Position.z);
					var inside = value >= MinValue && value <= MaxValue; 
					
					if (inside)
						_insidePoints.Add(point);
					else
						_outsidePoints.Add(point);
				}
			}
		}
		
		private Rect CalculateNoiseSize(List<PointData>[] pointsList)
		{
			float minX = float.MaxValue, maxX = float.MinValue;
			float minY = float.MaxValue, maxY = float.MinValue;

			foreach (var points in pointsList)
			{
				if(points == null)
					continue;
				
				foreach (var point in points)
				{
					var pos = point.Position;
					if (pos.x < minX) minX = pos.x;
					if (pos.x > maxX) maxX = pos.x;
					if (NoiseAxes == NoiseAxesMode.XY)
					{
						if (pos.y < minY) minY = pos.y;
						if (pos.y > maxY) maxY = pos.y;
					}
					else
					{
						if (pos.z < minY) minY = pos.z;
						if (pos.z > maxY) maxY = pos.z;
					}
				}
			}

			return new Rect(minX, minY, maxX - minX, maxY - minY);
		}

#if UNITY_EDITOR
		public override void DrawGizmos(Transform transform)
		{
			var gizmosOptions = GetGizmosOptions();

			if (ShowInsidePoints)
			{
				var resultsPort = GetOutputPort(nameof(InsidePoints));
				var results = (List<PointData>)GetValue(resultsPort);
				if (results == null || results.Count <= 0)
					return;

				GizmosUtility.DrawPoints(results, gizmosOptions, transform);
			}
			else
			{
				var resultsPort = GetOutputPort(nameof(OutsidePoints));
				var results = (List<PointData>)GetValue(resultsPort);
				if (results == null || results.Count <= 0)
					return;

				GizmosUtility.DrawPoints(results, gizmosOptions, transform);
			}
			
			if (ShowNoise && _noiseSize != Rect.zero)
			{
				var noise = GetInputValue(nameof(Noise), Noise);
				if (noise != null)
				{
					const int maxCount = 100;
					var segment = gizmosOptions.NoiseSegment;
					var width = Mathf.CeilToInt(_noiseSize.width / segment);
					if (width > maxCount)
					{
						width = maxCount;
						segment = _noiseSize.width / width;
					}
					var height = Mathf.CeilToInt(_noiseSize.height / segment);
					if (height > maxCount)
					{
						height = maxCount;
						segment = _noiseSize.width / height;
						width = Mathf.CeilToInt(_noiseSize.width / segment);
					}

					Gizmos.matrix = transform.localToWorldMatrix;

					Gizmos.color = gizmosOptions.Color;

					var sz = gizmosOptions.NoiseSize;
					var size = new Vector3(sz, sz, sz);
					for (int i = 0; i <= width; i++)
					{
						for (int j = 0; j <= height; j++)
						{
							float x = _noiseSize.xMin + i * segment;
							float y = _noiseSize.yMin + j * segment;

							float value = noise.GetValue(x, y);
							var inside = value >= MinValue && value <= MaxValue;

							var color = inside
								? gizmosOptions.Color
								: Color.white - (gizmosOptions.Color);
							color.a = 1f;
							
							Gizmos.color = color;

							var pos = NoiseAxes == NoiseAxesMode.XY
								? new Vector3(_noiseSize.xMin + i * segment, _noiseSize.yMin + j * segment, 0f)
								: new Vector3(_noiseSize.xMin + i * segment, 0f, _noiseSize.yMin + j * segment);
							Gizmos.DrawCube(pos, size);
						}
					}

					Gizmos.matrix = Matrix4x4.identity;
				}
			}
		}
#endif
	}
}