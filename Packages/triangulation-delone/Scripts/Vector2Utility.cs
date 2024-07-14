using UnityEngine;

namespace Delone
{
	public static class Vector2Utility
	{
		public static float CrossProduct(Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.y - v2.x * v1.y;
		}

		public static float DotProduct(Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.x + v2.y * v1.y;
		}

		public static double AngleBetweenVectors(Vector2 v1, Vector2 v2)
		{
			return Mathf.Acos(DotProduct(v1, v2) / v1.SqrMagnitude());
		}

		public static int IsLeft(Vector2 p0, Vector2 p1, Vector2 p)
		{
			float result = (p1.x - p0.x) * (p.y - p0.y) - (p.x - p0.x) * (p1.y - p0.y);

			if (result > 0f)
				return 1;
			
			if (result < 0f)
				return -1;
			
			return 0;
		}
		
		public static float SqrMagnitude(this Vector2 v)
		{
			return v.x * v.x + v.y * v.y;
		}

		public static float Magnitude(this Vector2 v)
		{
			return Mathf.Sqrt(v.SqrMagnitude());
		}
	}
}