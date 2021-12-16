using UnityEngine;

public static class MathExtensions
{
	public static bool IsApproximately(this float a, float b, float threshold = 0.001f)
	{
		return ((a < b) ? (b - a) : (a - b)) <= threshold;
	}

	public static float NormalizeDegrees(this float angle)
	{
		angle = angle % 360;
		if (angle < 0) {
			angle += 360;
		}
		return angle;
	}

	public static bool IsApproximatelyEqualTo(this Vector3 v1, Vector3 v2, float threshold = 0.001f)
	{
		return IsApproximately(v1.x, v2.x, threshold) && IsApproximately(v1.y, v2.y, threshold) && IsApproximately(v1.z, v2.z, threshold);
	}

	public static Vector3 Multiply(this Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
	}

	public static Vector2 Multiply(this Vector2 v1, Vector2 v2)
	{
		return new Vector2(v1.x * v2.x, v1.y * v2.y);
	}

	public static float CalcAngle2D(this Vector3 v1, Vector3 v2)
	{
		return Mathf.Atan2(v2.y - v1.y, v2.x - v1.x) * Mathf.Rad2Deg;
	}
}
