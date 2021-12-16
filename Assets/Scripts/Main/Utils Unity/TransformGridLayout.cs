using UnityEngine;

[ExecuteInEditMode]
public class TransformGridLayout : MonoBehaviour
{
	public float StartX;
	public float StartY;
	public float ChildWidth;
	public float ChildHeight;
	public float Spacing;
	public int NumElementsPerRow;

#if UNITY_EDITOR
	void Update()
	{
		if (Application.isPlaying) {
			return;
		}

		for (int i = 0, j = 0; i < transform.childCount; i++)
		{
			int col = i % NumElementsPerRow;
			float x = StartX + col * ChildWidth + col * Spacing;
			float y = StartY - j * ChildHeight - j * Spacing;

			transform.GetChild(i).SetLocalPos(x, y);

			if (col >= NumElementsPerRow - 1) {
				j++;
			}
		}
	}
#endif
}
