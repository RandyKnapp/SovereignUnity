using System;

namespace Sovereign
{
	public static class MathUtil
	{
		public static float Lerp(float a, float b, float t)
		{
			return a + t * (b - a);
		}

		public static float Map(float a, float b, float x)
		{
			return (x - a) / (b - a);
		}
	}
}