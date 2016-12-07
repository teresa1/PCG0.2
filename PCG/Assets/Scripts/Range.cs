using UnityEngine;
using System.Collections.Generic;
public class Range
	{
		float min, max;
		public Range (float min, float max)
		{
			this.max = max;
			this.min = min;
		}

		public float Min { get { return min; } } 
		public float Max { get { return max; } }

		public bool Intersects(Range other,out Range intersection)
		{
			intersection = new Range(0, 0);
			if (min > other.max || other.min > max) return false;

			intersection.min = Mathf.Max(min, other.min);
			intersection.max = Mathf.Min(max, other.max);
			return true;
		}

		public static List<Range> IntersectLists(List<Range> l1, List<Range> l2)
		{
			List<Range> intersections = new List<Range>();

			foreach (var item1 in l1)
			{
				foreach (var item2 in l2)
				{
					Range intersection;
					if (item1.Intersects(item2, out intersection))
						intersections.Add(intersection);
				}
			}
			return intersections;
		}

		public float Length { get { return max - min; }}

	}