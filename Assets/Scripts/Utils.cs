using UnityEngine;

public static class Utils
{
	public static float Distance(Vector2 v1, Vector2 v2)
	{
		return Mathf.Sqrt(Mathf.Pow(v2.x - v1.x, 2) + Mathf.Pow(v2.y - v1.y, 2));
	}

	public static float Distance(Vector3 v1, Vector3 v2)
	{
		return Mathf.Sqrt(Mathf.Pow(v2.x - v1.x, 2) + Mathf.Pow(v2.y - v1.y, 2) + Mathf.Pow(v2.z - v1.z, 2));
	}
}
