using UnityEngine;

public class GameAreaUnlocker : MonoBehaviour
{
	[SerializeField] GameObject[] cornGameObjects;
	[SerializeField] GameObject[] wheatGameObjects;
	[SerializeField] GameObject[] fishGameObjects;

	public void CornSetActive(bool active)
	{
		SetActiveAll(cornGameObjects, active);
	}

	public void WheatSetActive(bool active)
	{
		SetActiveAll(wheatGameObjects, active);
	}

	public void FishSetActive(bool active)
	{
		SetActiveAll(fishGameObjects, active);
	}

	private void SetActiveAll(GameObject[] gameObjArray, bool active)
	{
		foreach (var go in gameObjArray)
		{
			if (go.activeSelf != active)
			{
				go.SetActive(active);
			}
		}
	}
}
