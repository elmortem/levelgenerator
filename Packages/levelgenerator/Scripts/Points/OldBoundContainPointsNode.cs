using System;
using System.Collections.Generic;
using LevelGenerator.Bounds.Datas;
using UnityEngine;
using XNode;

namespace LevelGenerator.Points
{
	[Obsolete]
	public class OldBoundContainPointsNode : BasePointsNode
	{
		[Input] public List<Vector3> Points;
		[Input] public BoundData Bound;
		[Output] public List<Vector3> InsidePoints;
		[Output] public List<Vector3> OutsidePoints;
		
		private List<Vector3> _insidePoints;
		private List<Vector3> _outsidePoints;
		
		public override object GetValue(NodePort port)
		{
			if (port == null)
				return null;
			
			if (port.fieldName == nameof(InsidePoints))
			{
				CalcResults();
				return _insidePoints;
			}

			if (port.fieldName == nameof(OutsidePoints))
			{
				CalcResults();
				return _outsidePoints;
			}

			return null;
		}
		
		protected override void CalcResults(bool force = false)
		{
			if(LockCalc && _insidePoints != null)
				return;
			if(!force && _insidePoints != null)
				return;
			
			if(_insidePoints == null)
				_insidePoints = new();
			else
				_insidePoints.Clear();
			
			if(_outsidePoints == null)
				_outsidePoints = new();
			else
				_outsidePoints.Clear();
			
			var pointsList = GetInputValues(nameof(Points), Points);
			if(pointsList == null || pointsList.Length <= 0)
				return;
			
			var boundList = GetInputValues(nameof(Bound), Bound);
			if(boundList == null || boundList.Length <= 0)
				return;
			
			foreach (var point in Points)
			{
				bool inside = false;
				foreach (var boundData in boundList)
				{
					if (boundData.InBounds(point))
					{
						_insidePoints.Add(point);
						inside = true;
					}
				}
				
				if(!inside)
					_outsidePoints.Add(point);
			}
		}
	}
}