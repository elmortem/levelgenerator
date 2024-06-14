using System;
using System.Collections.Generic;
using UnityEngine.Splines;

namespace LevelGenerator.Splines
{
	[Serializable]
	public class SplineContainerData
	{
		public List<Spline> Splines = new();
		public KnotLinkCollection Knots = new();
	}
}