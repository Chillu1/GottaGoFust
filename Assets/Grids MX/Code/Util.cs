using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	public static class Util
	{
		public static float Wrap(float f, float min, float max)
		{
			float diff = Mathf.Abs(max - min);
			if (f > max)
			{
				f -= diff;
			}
			if (f < min)
			{
				f += diff;
			}
			return f;
		}
		
		public static bool IsOutsideRange(float value, float min, float max)
		{
			return (value < min || value > max);
		}
	}
}