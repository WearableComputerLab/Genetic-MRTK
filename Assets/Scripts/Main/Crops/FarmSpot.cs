using UnityEngine;

[ExecuteInEditMode]
public class FarmSpot : MonoBehaviour
{
	[SerializeField] SpriteRenderer spriteRenderer;
	public bool IsStartingPoint;

	[SerializeField] bool _hasSeeds = true;
	public bool HasSeeds {
		get { return _hasSeeds; }
		set {
			_hasSeeds = value;
			if (!IsStartingPoint) spriteRenderer.color = _hasSeeds ? Color.yellow : Color.white;
		}
	}

	void Start()
	{
		HasSeeds = _hasSeeds;
	}

#if UNITY_EDITOR
	void Update()
	{
		if (Application.isPlaying) {
			return;
		}

		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		HasSeeds = _hasSeeds;
	}
#endif
}
