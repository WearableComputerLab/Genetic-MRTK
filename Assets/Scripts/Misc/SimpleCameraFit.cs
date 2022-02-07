using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SimpleCameraFit : MonoBehaviour
{
	public enum Orientation { Landscape, Portrait }

	public float WorldSize = 1;
	public Orientation DefaultOrientation;

	private Camera _camera;
	private float aspect = -1;
	private float prevWorldSize = -1;

	void Awake()
	{
		_camera = GetComponent<Camera>();

		DidAspectRatioChange();
		SetCameraSize();
	}

	void Update()
	{
#if !UNITY_EDITOR
		if (DidAspectRatioChange())
#endif
		{
			SetCameraSize();
		}
	}

	private bool DidAspectRatioChange()
	{
		if (Screen.height == 0) return true;

		float tmpAspect = Screen.width / Screen.height;

		if (aspect != tmpAspect || prevWorldSize != WorldSize)
		{
			aspect = tmpAspect;
			prevWorldSize = WorldSize;
			return true;
		}

		return false;
	}

	private void SetCameraSize()
	{
		if (DefaultOrientation == Orientation.Landscape)
		{
			_camera.orthographicSize = 1f / _camera.aspect * WorldSize / 2f;
		}
		else {
			_camera.orthographicSize = WorldSize / 2f;
		}
	}
}
