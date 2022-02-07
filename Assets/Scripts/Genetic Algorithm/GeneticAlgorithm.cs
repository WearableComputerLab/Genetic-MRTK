using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GeneticAlgorithm<T>
{
	Transform farm;
	public List<DNA<T>> Population { get; private set; }
	public T[] BestGenes { get; private set; }
	public float BestFitness { get; private set; }
	public int Generation { get; private set; }

	public int Elitism;
	public float MutationRate;
	public CrossoverMethod CrossoverMethod;

	private int dnaSize;
	private Func<Random, T> getRandomGene;
	private Func<int, float> fitnessFunction;

	private Random random;
	private float fitnessSum;
	private List<DNA<T>> auxPopulation;

	public GeneticAlgorithm(Transform farm, int populationSize, int dnaSize, Random random, Func<Random, T> getRandomGene, Func<int, float> fitnessFunction, 
		float mutationRate = 0.01f, int elitism = 5, CrossoverMethod crossoverMethod = CrossoverMethod.Uniform, T[] preLoad = null)
	{
		this.random = random;

		Generation = 1;
		Elitism = elitism;
		MutationRate = mutationRate;
		CrossoverMethod = crossoverMethod;
		Population = new List<DNA<T>>();
		auxPopulation = new List<DNA<T>>();

		this.farm = farm;
		this.dnaSize = dnaSize;
		this.getRandomGene = getRandomGene;
		this.fitnessFunction = fitnessFunction;

		BestGenes = new T[dnaSize];

		for (int i = 0; i < populationSize; i++)
		{
			Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, initGenes: true, preload: preLoad));
		}
	}

	public void NewGeneration(int numNewDNA = 0, bool crossoverNewMembers = false)
	{
		if (Population.Count + numNewDNA <= 0) return;

		if (Population.Count > 0) CalculateFitness();

		auxPopulation.Clear();
		auxPopulation.Capacity = Population.Count + numNewDNA;

		Population.Sort(CompareFitness);

		for (int i = 0; i < auxPopulation.Capacity; i++)
		{
			if (i < Elitism && i < Population.Count - 1)
			{
				//Population[i].Mutate(MutationRate);
				auxPopulation.Add(Population[i]);
			}
			else if (i < Population.Count || crossoverNewMembers)
			{
				DNA<T> parent1 = GetWeightedRandomDNA();
				DNA<T> parent2 = GetWeightedRandomDNA();

				DNA<T> child = parent1.Crossover(parent2, CrossoverMethod);

				child.Mutate(MutationRate);

				auxPopulation.Add(child);
			}
			else
			{
				auxPopulation.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, initGenes: true));
			}
		}

		List<DNA<T>> tmp = Population;
		Population = auxPopulation;
		auxPopulation = tmp;

		Generation++;
	}

	private int CompareFitness(DNA<T> a, DNA<T> b)
	{
		if (a.Fitness > b.Fitness) return -1;
		else if (a.Fitness < b.Fitness) return 1;
		else return 0;
	}

	private void CalculateFitness()
	{
		int indexOfBest = 0;
		fitnessSum = 0;

		

		for (int i = 0; i < Population.Count; i++)
		{
			FarmArea farmArea = farm.GetComponentsInChildren<FarmArea>()[i];
			
			Population[i].CalculateFitness(i);
			if (farmArea.GetComponent<FarmAreaInteractable>().IsSelected())
            {
				Population[i].OverrideFitness();
				Debug.Log("Add 20 to Fitness");
            }
			if (Population[i].Fitness > Population[indexOfBest].Fitness) {
				indexOfBest = i;
			}

			fitnessSum += Population[i].Fitness;
		}

		BestFitness = Population[indexOfBest].Fitness;
		Population[indexOfBest].Genes.CopyTo(BestGenes, 0);
	}

	private DNA<T> GetWeightedRandomDNA()
	{
		if (fitnessSum == 0) return Population[random.Next(Population.Count)];

		while (true)
		{
			int index = random.Next(Population.Count);

			if (Population[index].Fitness < 0) {
				throw new Exception("Fitness should not be negative!");
			}

			if (random.NextDouble() < Population[index].Fitness / fitnessSum)
			{
				return Population[index];
			}
		}
	}
}
