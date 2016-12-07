using UnityEngine;
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
	}