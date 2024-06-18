using UnityEngine.Serialization;
using XNode;

namespace LevelGenerator.Noises
{
	public class CircleNoiseNode : GenNode
	{
		[FormerlySerializedAs("NoiseData")] 
		public CircleNoiseData CircleNoise;
		[FormerlySerializedAs("Data")] 
		[Output] public NoiseData Noise;
		
		public override object GetValue(NodePort port) 
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Noise))
			{
				return CircleNoise;
			}
			
			return null;
		}
	}
}