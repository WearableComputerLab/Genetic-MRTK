using UnityEngine;
using UnityEngine.UI;

public class ResourceBank : MonoBehaviour
{
	public int Resource;
	[SerializeField] Text text;

	private int prevFrameResource;

	void Update()
	{
		if (prevFrameResource != Resource)
		{
			prevFrameResource = Resource;
			text.text = Resource.ToString();
		}
	}
}
