using LevelGenerator.Points;
using UnityEngine.Serialization;
using XNode;

namespace LevelGenerator.Noises
{
	public class CombineNoiseNode : BasePointsNode
	{
		[Input] public NoiseData Noises;
		public NoiseChangeMode Mode;
		[FormerlySerializedAs("Result")] 
		[Output] public NoiseData ResultNoise;

		private NoiseChangeMode _lastMode;
		private CombineNoiseData _result;

		public override object GetValue(NodePort port)
		{
			if(port == null)
				return null;

			if (port.fieldName == nameof(ResultNoise))
			{
				CalcResults();
				return _result;
			}

			return null;
		}

		protected override void CalcResults(bool force = false)
		{
			var port = GetInputPort(nameof(Noises));
			if (port == null || !port.IsConnected)
			{
				_result = null;
				return;
			}
			
			if(LockCalc && _result != null)
				return;
			if(!force && _lastMode == Mode && _result != null)
				return;
			
			var noises = GetInputValues<NoiseData>(nameof(Noises));
			if (noises == null || noises.Length <= 0)
			{
				_result = null;
				return;
			}
			
			if(_result == null)
				_result = new CombineNoiseData();
			else
				_result.Noises.Clear();
			
			_lastMode = Mode;

			foreach (var noise in noises)
			{
				_result.Noises.Add(noise);
			}
			_result.Mode = Mode;
		}
	}
}