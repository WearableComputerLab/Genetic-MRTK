using UnityEngine;

[ExecuteInEditMode]
public class FarmArea : MonoBehaviour
{
	public FarmSpot[] FarmSpots;

#if UNITY_EDITOR
	void Update()
	{
		if (!Application.isPlaying) {
			FarmSpots = GetComponentsInChildren<FarmSpot>();
		}
	}
#endif
}
