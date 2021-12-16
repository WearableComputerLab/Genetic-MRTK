using System.Collections.Generic;
using UnityEngine;

public class TestResourceGathering : MonoBehaviour
{
	[Header("Genetic Algorithm")]
	[SerializeField] int populationSize = 20;
	[SerializeField] float mutationRate = 0.01f;
	[SerializeField] int elitism = 5;
	[SerializeField] CrossoverMethod crossoverMethod = CrossoverMethod.Uniform;

	[Header("Entities")]
	[SerializeField] int numDirections = 10;
	[SerializeField] float generationLifetime = 1;
	[SerializeField] float speed = 10;

	[Header("References")]
	[SerializeField] EntityResourceGather entityPrefab;
	[SerializeField] Transform startingPoint;
	[SerializeField] Transform targetPoint;
	[SerializeField] Transform entitiesParent;

	private System.Random random;
	private float lastGenerationTime;
	private float directionChangeTime;
	private GeneticAlgorithm<Vector2> ga;
	private List<EntityResourceGather> entityList;

	private Vector2[] possibleDirs = new Vector2[] {
		new Vector2(0,0).normalized,
		new Vector2(0,1).normalized,
		new Vector2(0,-1).normalized,
		new Vector2(1,0).normalized,
		new Vector2(1,1).normalized,
		new Vector2(1,-1).normalized,
		new Vector2(-1,0).normalized,
		new Vector2(-1,1).normalized,
		new Vector2(-1,-1).normalized,
	};

	void Awake()
	{
		entityList = new List<EntityResourceGather>(populationSize);

		for (int i = 0; i < populationSize; i++)
		{
			entityList.Add(Instantiate(entityPrefab, startingPoint.localPosition, Quaternion.identity, entitiesParent));
		}
	}

	void Start()
	{
		directionChangeTime = generationLifetime / numDirections;

		random = new System.Random();
		ga = new GeneticAlgorithm<Vector2>(populationSize, numDirections, random, GetRandomVector2, FitnessFunction, mutationRate, elitism, crossoverMethod);

		lastGenerationTime = Time.time;
		ResetEntities();
	}

	void Update()
	{
		if (Time.time - lastGenerationTime >= generationLifetime)
		{
			lastGenerationTime = Time.time;

			CalcDists();
			ga.NewGeneration();
			ResetEntities();
		}
	}

	private void ResetEntities()
	{
		for (int i = 0; i < ga.Population.Count; i++)
		{
			EntityResourceGather entity = entityList[i];
			entity.Speed = speed;
			entity.DNA = ga.Population[i];
			entity.DirectionChangeTime = directionChangeTime;
			entity.transform.localPosition = startingPoint.localPosition;
			entity.Reset();

			if (!entity.gameObject.activeSelf) entity.gameObject.SetActive(true);
		}
	}

	private void CalcDists()
	{
		foreach (var entity in entityList)
		{
			entity.DistToTarget = Utils.Distance(entity.transform.localPosition, targetPoint.localPosition);
			entity.DistToStart = Utils.Distance(entity.transform.localPosition, startingPoint.localPosition);
		}
	}

	private float FitnessFunction(int index)
	{
		float score = 0;
		EntityResourceGather entity = entityList[index];

		if (entity.ResourceDelivered > 0)
		{
			score += Mathf.Pow(2, entity.ResourceDelivered * 40);
		}
		else if (entity.ResourceGathered > 0)
		{
			score += Mathf.Pow(2, entity.ResourceGathered * 20);

			if (entity.DistToStart <= 0.1f) entity.DistToStart = 0.1f;
			float inverseDist = 1 / entity.DistToStart;
			score += Mathf.Pow(2, inverseDist);
		}
		else
		{
			if (entity.DistToTarget <= 0.1f) entity.DistToTarget = 0.1f;
			float inverseDist = 1 / entity.DistToTarget;
			score += Mathf.Pow(2, inverseDist);
		}

		return score;
	}

	private Vector2 GetRandomVector2(System.Random random)
	{
		//float x = random.Next(-1, 1);
		//float y = random.Next(-1, 1);
		//Vector2 v = new Vector2(x, y);
		//v.Normalize();
		//return v;
		return possibleDirs[random.Next(possibleDirs.Length)];
	}
}
