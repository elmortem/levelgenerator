using XNode;

namespace LevelGenerator.Noises
{
	public class PerlinNoiseNode : GenNode
	{
		public NoiseData NoiseData;
		[Output] public NoiseData Data;
		
		public override object GetValue(NodePort port) 
		{
			if (port.fieldName == nameof(Data))
			{
				return NoiseData;
			}
			
			return null;
		}
	}
}