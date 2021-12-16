using UnityEngine;

public class Warehouse : MonoBehaviour
{
	[SerializeField] GameController GameController;
	[SerializeField] SpriteRendererFill[] Storages;

	private SpriteRendererFill[] contents;

	void Awake()
	{
		contents = new SpriteRendererFill[Storages.Length];

		for (int i = 0; i < Storages.Length; i++)
		{
			contents[i] = Storages[i].transform.GetChild(0).GetComponent<SpriteRendererFill>();
		}

		GameController.ValueChangedEvent += UpdateResourceValue;
		GameController.StorageChangedEvent += UpdateStorage;
	}

	private void UpdateResourceValue(Resource resource)
	{
		if (resource == Resource.Money) {
			return;
		}

		int index = (int)resource;
		contents[index].FillValue = (float)GameController.Resources[index] / (float)GameController.ResourceStorages[index];
	}

	private void UpdateStorage(Resource resource)
	{
		if (resource == Resource.Money) {
			return;
		}

		int index = (int)resource;
		Storages[index].FillValue = (float)GameController.ResourceStorages[index] / (float)GameController.MaxStorages[index];
	}
}
