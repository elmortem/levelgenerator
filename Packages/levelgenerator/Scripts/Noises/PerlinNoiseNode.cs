using XNode;

namespace LevelGenerator.Noises
{
	public class PerlinNoiseNode : GenNode
	{
		public PerlinNoiseData NoiseData;
		[Output] public PerlinNoiseData Data;
		
		public override object GetValue(NodePort port) 
		{
			if(port == null)
				return null;
			
			if (port.fieldName == nameof(Data))
			{
				return NoiseData;
			}
			
			return null;
		}
	}
}