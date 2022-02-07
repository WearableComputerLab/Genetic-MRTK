using UnityEngine;

public static class TransformExtensions
{
	public static void SetLocalPos(this Transform t, float? x = null, float? y = null, float? z = null)
	{
		Vector3 pos = t.localPosition;
		if (x.HasValue) pos.x = x.Value;
		if (y.HasValue) pos.y = y.Value;
		if (z.HasValue) pos.z = z.Value;
		t.localPosition = pos;
	}

	public static void SetLocalScale(this Transform t, float? x = null, float? y = null, float? z = null)
	{
		Vector3 scale = t.localScale;
		if (x.HasValue) scale.x = x.Value;
		if (y.HasValue) scale.y = y.Value;
		if (z.HasValue) scale.z = z.Value;
		t.localScale = scale;
	}
}
