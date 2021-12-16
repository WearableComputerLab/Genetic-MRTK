using System;

public enum CrossoverMethod
{
	Midpoint,
	Uniform,
}

public class DNA<T>
{
	public T[] Genes { get; private set; }
	public float Fitness { get; private set; }

	private Random random;
	private Func<Random, T> getRandomGene;
	private Func<int, float> fitnessFunction;

	public DNA(int size, Random random, Func<Random, T> getRandomGene, Func<int, float> fitnessFunction, bool initGenes = true, T[] preload = null)
	{
		this.random = random;
		this.getRandomGene = getRandomGene;
		this.fitnessFunction = fitnessFunction;

		Genes = new T[size];

		if (initGenes)
		{
			if (preload != null && preload.Length == size)
			{
				preload.CopyTo(Genes, 0);
			}
			else
			{
				for (int i = 0; i < size; i++)
				{
					Genes[i] = this.getRandomGene(random);
				}
			}
		}
	}

	public float CalculateFitness(int index)
	{
		Fitness = fitnessFunction(index);
		return Fitness;
	}

	public DNA<T> Crossover(DNA<T> otherParent, CrossoverMethod crossoverMethod)
	{
		var child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, initGenes: false);

		if (crossoverMethod == CrossoverMethod.Midpoint)
		{
			int midPoint = random.Next(Genes.Length);
			for (int i = 0; i < Genes.Length; i++)
			{
				if (i > midPoint) child.Genes[i] = this.Genes[i];
				else child.Genes[i] = otherParent.Genes[i];
			}
		}
		else if (crossoverMethod == CrossoverMethod.Uniform)
		{
			for (int i = 0; i < Genes.Length; i++)
			{
				child.Genes[i] = random.NextDouble() < 0.5 ? this.Genes[i] : otherParent.Genes[i];
			}
		}

		return child;
	}

	public void Mutate(float mutationRate)
	{
		for (int i = 0; i < Genes.Length; i++)
		{
			if (random.NextDouble() < mutationRate)
			{
				Genes[i] = getRandomGene(random);
			}
		}
	}
}