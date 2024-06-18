using UnityEngine.Serialization;
using XNode;

namespace LevelGenerator.Noises
{
	public class PerlinNoiseNode : GenNode
	{
		[FormerlySerializedAs("NoiseData")] public PerlinNoiseData PerlinNoise;
		[FormerlySerializedAs("Data")] [Output] public NoiseData Noise;
		
		public override object GetValue(NodePort port) 
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Noise))
			{
				return PerlinNoise;
			}
			
			return null;
		}
	}
}