using UnityEngine;

public static class RectTransformExtensions
{
	public static void SetSizeDeltaX(this RectTransform t, float value)
	{
		Vector2 size = t.sizeDelta;
		size.x = value;
		t.sizeDelta = size;
	}

	public static void SetSizeDeltaY(this RectTransform t, float value)
	{
		Vector2 size = t.sizeDelta;
		size.y = value;
		t.sizeDelta = size;
	}

	public static void SetAnchoredPositionX(this RectTransform t, float value)
	{
		Vector3 pos = t.anchoredPosition;
		pos.x = value;
		t.anchoredPosition = pos;
	}

	public static void SetAnchoredPositionY(this RectTransform t, float value)
	{
		Vector3 pos = t.anchoredPosition;
		pos.y = value;
		t.anchoredPosition = pos;
	}

	public static void SetAnchoredPositionZ(this RectTransform t, float value)
	{
		Vector3 pos = t.anchoredPosition;
		pos.z = value;
		t.anchoredPosition = pos;
	}
}
