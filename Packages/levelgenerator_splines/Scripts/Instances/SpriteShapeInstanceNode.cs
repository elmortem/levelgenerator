using UnityEngine.U2D;
using XNode;

namespace LevelGenerator.Splines
{
	public class SpriteShapeInstanceNode : SplineInstanceNode<SpriteShapeInstanceData>
	{
		public string Name;
		public SpriteShape SpriteShape;
		public float Height = 1f;

		public override object GetValue(NodePort port) => base.GetValue(port);

		protected override void CalcResults(bool force = false)
		{
			if(LockCalc && Results != null)
				return;
			if(!force && Results != null)
				return;
			
			if(Results == null)
				Results = new();
			else
				Results.Clear();

			Results.Add(new SpriteShapeInstanceData
			{
				Name = Name,
				SplineContainer = GetInputValue(nameof(SplineContainer), SplineContainer),
				SpriteShape = SpriteShape,
				Height = Height
			});
		}
	}
}