using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TestShakespeare : MonoBehaviour
{
	[SerializeField] string targetString = "To be, or not to be, that is the question.";
	[SerializeField] string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.|!#$%&/()=? ";
	[SerializeField] int populationSize = 200;
	[SerializeField] float mutationRate = 0.01f;
	[SerializeField] int elitism = 5;
	[SerializeField] CrossoverMethod crossoverMethod = CrossoverMethod.Uniform;
	[SerializeField] int numCharsPerText = 15000;

	[SerializeField] Text targetText;
	[SerializeField] Text bestText;
	[SerializeField] Text bestFitnessText;
	[SerializeField] Text numGenerationsText;
	[SerializeField] Transform populationTextParent;
	[SerializeField] Text textPrefab;

	private int numDudesPerTextObj;
	private List<Text> textList = new List<Text>();
	private System.Random random = new System.Random();
	private GeneticAlgorithm<char> ga;

	void Awake()
	{
		numDudesPerTextObj = numCharsPerText / validCharacters.Length;
		if (numDudesPerTextObj > populationSize) numDudesPerTextObj = populationSize;

		int numTextObjects = Mathf.CeilToInt((float)populationSize / numDudesPerTextObj);

		for (int i = 0; i < numTextObjects; i++)
		{
			textList.Add(Instantiate(textPrefab, populationTextParent));
		}
	}

	void Start()
	{
		targetText.text = targetString;

		ga = new GeneticAlgorithm<char>(populationSize, targetString.Length, random, GetRandomChar, FitnessFunction, mutationRate, elitism, crossoverMethod);

		if (string.IsNullOrEmpty(targetString))
		{
			Debug.LogError("Target string is null or empty");
			this.enabled = false;
		}
	}

	void Update()
	{
		ga.NewGeneration();

		UpdateText();

		if (ga.BestFitness == 1)
		{
			this.enabled = false;
		}
	}

	private char GetRandomChar(System.Random random)
	{
		return validCharacters[random.Next(0, validCharacters.Length)];
	}

	private float FitnessFunction(int index)
	{
		char[] c = ga.Population[index].Genes;
		int score = 0;

		for (int i = 0; i < c.Length; i++)
		{
			if (c[i] == targetString[i])
			{
				score++;
			}
		}

		float result = (float)score / c.Length;

		float a = 5;
		result = (Mathf.Pow(a, result) - 1) / (a - 1);

		return result;
	}

	private void UpdateText()
	{
		bestText.text = CharArrayToString(ga.BestGenes);
		bestFitnessText.text = ga.BestFitness.ToString();

		numGenerationsText.text = ga.Generation.ToString();

		for (int i = 0; i < textList.Count; i++)
		{
			var sb = new StringBuilder();
			int endIndex = i == textList.Count - 1 ? ga.Population.Count : (i + 1) * numDudesPerTextObj;
			for (int j = i * numDudesPerTextObj; j < endIndex; j++)
			{
				foreach (var c in ga.Population[j].Genes)
				{
					sb.Append(c);
				}
				if (j < endIndex - 1) sb.AppendLine();
			}

			textList[i].text = sb.ToString();
		}
	}

	private string CharArrayToString(char[] charArray)
	{
		var sb = new StringBuilder();
		foreach (var c in charArray)
		{
			sb.Append(c);
		}

		return sb.ToString();
	}
}
