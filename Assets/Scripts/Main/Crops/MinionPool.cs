using System.Collections.Generic;
using UnityEngine;

public class MinionPool : MonoBehaviour
{
	[SerializeField] GameObject MinionPrefab;
	[SerializeField] int InitialPoolSize;

	private Queue<GameObject> pool = new Queue<GameObject>();

	void Awake()
	{
		InitPool(InitialPoolSize);
	}

	private void InitPool(int numElements)
	{
		for (int i = 0; i < InitialPoolSize; i++)
		{
			pool.Enqueue(Instantiate(MinionPrefab, transform, false));
		}
	}

	public GameObject Get(Transform parent = null)
	{
		if (pool.Count == 0)
		{
			return Instantiate(MinionPrefab, parent);
		}

		GameObject go = pool.Dequeue();
		go.transform.SetParent(parent, false);
		return go;
	}

	public void Return(GameObject go)
	{
		go.transform.SetParent(transform, false);
		pool.Enqueue(go);
	}
}
