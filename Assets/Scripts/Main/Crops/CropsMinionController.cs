using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[ExecuteInEditMode]
public class CropsMinionController : MonoBehaviour
{
	[Header("Genetic Algorithm")]
	public float MutationRate;
	public int Elitism;
	public CrossoverMethod CrossoverMethod;

	[Header("Minions")]
	[SerializeField] int startingNumMinions;
	[SerializeField] float generationLifeTime;
	[SerializeField] float minionMovementSpeed = 1;
	[SerializeField] float minionActionSpeed = 1;
	[SerializeField] float minionRotationSpeed = 1;
	[SerializeField] MinionPool minionPool;
	[SerializeField] float evolutionTimePerMinion;
	[SerializeField] float evolutionStartAndEndDelay;
	[SerializeField] float maxEvolutionTime;

	[Header("Farm")]
	[SerializeField] Transform farm;
	[SerializeField] FarmArea[] farmAreas;

	[Header("UI")]
	[SerializeField] Text topCenterText;
	[SerializeField] Text numMinionsText;
	[SerializeField] Text maxMinionsText;
	[SerializeField] Text minionLimitText;

	public Action<int> AddResource;
	public Func<bool> CanBuyMinion;
	public Action BuyMinion;
	public Action SellMinion;

	public int MaxNumMinions { get { return farmAreas.Length; } }

	private GeneticAlgorithm<float> ga;
	private float lastGenerationTime;

	private List<Minion> minions;
	private float evolutionStartTime;
	private int minionEvolveIndex;
	private StringBuilder sb = new StringBuilder();

	private int _numMinionsToAdd;
	private int NumMinionsToAdd {
		get { return _numMinionsToAdd; }
		set {
			_numMinionsToAdd = Mathf.Clamp(value, 0, MaxNumMinions - minions.Count);
			SetMinionsText();
		}
	}

	private int numMinions = -1;

	private Tweener limitTextAnimation;
	private Sequence delaySequence;

	void Start()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) {
			return;
		}
#endif
		minions = new List<Minion>(MaxNumMinions);
		//maxMinionsText.text = MaxNumMinions + " max";
		//topCenterText.text = "";

		int numMinions = Mathf.Min(startingNumMinions, MaxNumMinions);
		AddMinions(numMinions);
		ga = new GeneticAlgorithm<float>(numMinions, 2, Rand.Random, GetRandomFloat, FitnessFunction,
			mutationRate: MutationRate, elitism: Elitism, crossoverMethod: CrossoverMethod);

		ResetFarm();
		evolutionStartTime = numMinions > 0 ? Time.time : -1;
		minionEvolveIndex = 0;

		NumMinionsToAdd = 0;

		limitTextAnimation = minionLimitText.DOFade(0, 0.3f)
			.From()
			.OnComplete(() => delaySequence.Restart())
			.SetEase(Ease.Linear)
			.SetAutoKill(false)
			.Pause();

		delaySequence = DOTween.Sequence()
			.AppendInterval(3)
			.AppendCallback(() => limitTextAnimation.PlayBackwards())
			.SetAutoKill(false)
			.Pause();
	}

	void Update()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) {
			farmAreas = farm.GetComponentsInChildren<FarmArea>();
			return;
		}
#endif

		if (numMinions != minions.Count) {
			numMinions = minions.Count;
			SetMinionsText();
		}

		if (minions.Count == 0 && NumMinionsToAdd == 0)
		{
			//Do Nothing
		}
		else if (evolutionStartTime >= 0)
		{
			//TODO: THIS CODE SUCKS!! REWRITE THIS CRAP!
			float totalEvolutionTime = maxEvolutionTime;
			float time = Time.time - evolutionStartTime;
			// topCenterText.text = "Evolving: " + (totalEvolutionTime - time).ToString("F1");

			float actualEvolTime = totalEvolutionTime - evolutionStartAndEndDelay * 2;
			float waitTime = actualEvolTime / minions.Count;
			float aux = Mathf.Clamp(time - evolutionStartAndEndDelay, -evolutionStartAndEndDelay, actualEvolTime) / actualEvolTime * (minions.Count - 1);
			int index = Mathf.FloorToInt(aux);

			while (minionEvolveIndex <= index) {
				minions[minionEvolveIndex].Reset(ga.Population[minionEvolveIndex].Genes, minionMovementSpeed, minionActionSpeed, minionRotationSpeed, waitTime);
				minionEvolveIndex++;
			}

			if (time >= totalEvolutionTime)
			{
				evolutionStartTime = -1;
				lastGenerationTime = Time.time;
				topCenterText.text = "";
			}
		}
		else
		{
			float time = Time.time - lastGenerationTime;
			if (generationLifeTime > 0) {
				topCenterText.text = "Ends in: " + (generationLifeTime - time).ToString("F1");
			} else if (!string.IsNullOrEmpty(topCenterText.text)) {
				topCenterText.text = "";
			}

			if (generationLifeTime > 0 && time >= generationLifeTime || DidAllMinionsFinish())
			{
				ga.MutationRate = MutationRate;
				ga.Elitism = Elitism;
				ga.CrossoverMethod = CrossoverMethod;

				ga.NewGeneration(NumMinionsToAdd);
				AddMinions(NumMinionsToAdd);
				NumMinionsToAdd = 0;

				ResetFarm();
				evolutionStartTime = Time.time;
				minionEvolveIndex = 0;

				return;
			}

			for (int i = 0; i < minions.Count; i++)
			{
				minions[i].MyUpdate();
			}
		}
	}

	private float GetRandomFloat(System.Random r)
	{
		return (float)r.NextDouble();
	}

	private float FitnessFunction(int i)
	{
		return Mathf.Pow(5, minions[i].ResourceGathered / minions[i].TimeToCompletion);
	}

	private void AddMinions(int amount)
	{
		int num = minions.Count;

		for (int i = num; i < num + amount; i++)
		{
			minions.Add(minionPool.Get(this.transform).GetComponent<Minion>());
			minions[i].Init(farmAreas[i]);
			minions[i].ResourceDeliveredEvent += AddResource;
		}
	}

	private void ResetFarm()
	{
		for (int i = 0; i < farmAreas.Length; i++)
		{
			for (int j = 0; j < farmAreas[i].FarmSpots.Length; j++)
			{
				farmAreas[i].FarmSpots[j].HasSeeds = true;
			}
		}
	}

	private bool DidAllMinionsFinish()
	{
		for (int i = 0; i < minions.Count; i++)
		{
			if (minions[i].State != State.Finished) {
				return false;
			}
		}
		return true;
	}

	private void SetMinionsText()
	{
		sb.Length = 0;
		sb.Append(numMinions);
		sb.Append(" (+");
		sb.Append(NumMinionsToAdd);
		sb.Append(")");
		numMinionsText.text = sb.ToString();
	}

	#region Button Functions
	public void OnAddMinionButtonClick()
	{
		if (!CanBuyMinion()) return;

		if (NumMinionsToAdd + 1 > MaxNumMinions - minions.Count) {
			if (limitTextAnimation.IsComplete() || delaySequence.IsPlaying()) {
				limitTextAnimation.Complete();
				delaySequence.Restart();
			} else {
				limitTextAnimation.PlayForward();
			}
			return;
		}

		BuyMinion();
		NumMinionsToAdd++;
	}

	public void OnRemoveMinionButtonClick()
	{
		delaySequence.Rewind();
		limitTextAnimation.PlayBackwards();

		if (NumMinionsToAdd > 0) SellMinion();
		NumMinionsToAdd--;
	}
	#endregion
}
