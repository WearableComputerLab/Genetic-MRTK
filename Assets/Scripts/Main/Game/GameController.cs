using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameController : MonoBehaviour
{
	[Header("Starting Values")]
	[SerializeField] int[] startingResources;
	[SerializeField] int[] startingStorages;
	[SerializeField] int[] maxStorages;
	[SerializeField] int minionPrice;

	[Header("Game References")]
	[SerializeField] UpgradesManager UpgradesManager;
	[SerializeField] GameAreaUnlocker GameAreaUnlocker;
	//[SerializeField] SimpleCameraFit simpleCameraFit;
	[SerializeField] Transform[] gameAreas;

	[Header("UI References")]
	[SerializeField] Text[] resourceTexts;
	[SerializeField] Canvas[] gameAreaCanvases;
	[SerializeField] GameObject upgradesMenu;
	[SerializeField] Text notEnoughMoneyText;

	public event Action<Resource> ValueChangedEvent;
	public event Action<Resource> StorageChangedEvent;

	public int[] Resources { get { return resources; } }
	public int[] ResourceStorages { get { return resourceStorages; } }
	public int[] MaxStorages { get { return maxStorages; } }

	private int[] resources;
	private int[] resourceStorages;
	private Tweener[] moveToAreaTweens;
	CropsMinionController cornController;
	CropsMinionController wheatController;

	private Tweener noMoneyTextAnimation;
	private Sequence delaySequence;

	void Awake()
	{
		//GameAreaUnlocker.CornSetActive(true);
		GameAreaUnlocker.WheatSetActive(true);
		//GameAreaUnlocker.FishSetActive(false);

		UpgradesManager.BuyUpgrade = Buy;
		UpgradesManager.CanBuyUpgrade = CanBuy;
		UpgradesManager.UpgradeUnlockedEvent += CheckUpgrades;

		//cornController = gameAreas[(int)Resource.Corn].GetComponent<CropsMinionController>();
		//cornController.AddResource = val => SetResource(Resource.Corn, GetResource(Resource.Corn) + val);
		//cornController.CanBuyMinion = CanBuyMinion;
		//cornController.BuyMinion = BuyMinion;
		//cornController.SellMinion = SellMinion;

		wheatController = gameAreas[0].GetComponent<CropsMinionController>();
		wheatController.AddResource = val => SetResource(Resource.Wheat, GetResource(Resource.Wheat) + val);
		wheatController.CanBuyMinion = CanBuyMinion;
		wheatController.BuyMinion = BuyMinion;
		wheatController.SellMinion = SellMinion;
	}

	void Start()
	{
		Array resourceEnumValues = Enum.GetValues(typeof(Resource));

		resources = new int[resourceEnumValues.Length];
		resourceStorages = new int[resourceEnumValues.Length];

		foreach (Resource resource in resourceEnumValues)
		{
			int index = (int)resource;
			SetStorage(resource, startingStorages[index]);
			SetResource(resource, startingResources[index]);
		}

		moveToAreaTweens = new Tweener[gameAreas.Length];

		//for (int i = 0; i < moveToAreaTweens.Length; i++)
		//{
		//	Vector3 pos = gameAreas[i].localPosition;
		//	pos.z = simpleCameraFit.transform.localPosition.z;

		//	moveToAreaTweens[i] = simpleCameraFit.transform.DOLocalMove(pos, 1)
		//		.SetEase(Ease.OutExpo)
		//		.SetAutoKill(false)
		//		.Pause();
		//}

		//OnGameAreaChangeButtonClick(0);
		//moveToAreaTweens[0].Complete();

		//noMoneyTextAnimation = notEnoughMoneyText.DOFade(0, 0.3f)
		//	.From()
		//	.OnComplete(() => delaySequence.Restart())
		//	.SetEase(Ease.Linear)
		//	.SetAutoKill(false)
		//	.Pause();

		//delaySequence = DOTween.Sequence()
		//	.AppendInterval(3)
		//	.AppendCallback(() => noMoneyTextAnimation.PlayBackwards())
		//	.SetAutoKill(false)
		//	.Pause();
	}

	void OnValidate()
	{
		int numResources = Enum.GetValues(typeof(Resource)).Length;

		if (startingResources.Length != numResources) startingResources = new int[numResources];
		if (startingStorages.Length != numResources) startingStorages = new int[numResources];
		if (maxStorages.Length != numResources) maxStorages = new int[numResources];
	}

	private bool CanBuy(int cost)
	{
		if (GetResource(Resource.Money) >= cost)
		{
			return true;
		}
		else
		{
			if (noMoneyTextAnimation.IsComplete() || delaySequence.IsPlaying())
			{
				noMoneyTextAnimation.Complete();
				delaySequence.Restart();
			}
			else
			{
				noMoneyTextAnimation.PlayForward();
			}
			return false;
		}
	}

	private void Buy(int cost)
	{
		SetResource(Resource.Money, GetResource(Resource.Money) - cost);
	}

	private bool CanBuyMinion()
	{
		return CanBuy(minionPrice);
	}

	private void BuyMinion()
	{
		Buy(minionPrice);
	}

	private void SellMinion()
	{
		delaySequence.Rewind();
		noMoneyTextAnimation.PlayBackwards();

		SetResource(Resource.Money, GetResource(Resource.Money) + minionPrice);
	}

	#region Resources

	private int GetResource(Resource resource)
	{
		return resources[(int)resource];
	}

	private int GetResourceStorage(Resource resource)
	{
		return resourceStorages[(int)resource];
	}

	private void SetResource(Resource resource, int value)
	{
		int index = (int)resource;
		int maxValue = resourceStorages[index];

		resources[index] = maxValue <= 0 ? value : Mathf.Min(value, maxValue);
		UpdateResourceVisuals(resource);
	}

	private void SetStorage(Resource resource, int value)
	{
		int index = (int)resource;
		int maxValue = maxStorages[(int)resource];

		resourceStorages[index] = maxValue <= 0 ? value : Mathf.Min(value, maxValue);
		UpdateResourceVisuals(resource);

		if (ValueChangedEvent != null) StorageChangedEvent(resource);
	}

	private void UpdateResourceVisuals(Resource resource)
	{
		//int index = (int)resource;

		//if (resourceStorages[index] > 0) {
		//	resourceTexts[index].text = resources[index] + "/" + resourceStorages[index];
		//} else {
		//	resourceTexts[index].text = resources[index].ToString();
		//}

		//if (ValueChangedEvent != null) ValueChangedEvent(resource);
	}

	#endregion

	#region Game Areas

	private void StopTweens()
	{
		for (int i = 0; i < gameAreas.Length; i++)
		{
			moveToAreaTweens[i].Pause();
		}
	}

	public void OnGameAreaChangeButtonClick(int index)
	{
		//if (moveToAreaTweens[index].IsPlaying()) return;

		//StopTweens();
		//moveToAreaTweens[index].ChangeStartValue(simpleCameraFit.transform.localPosition);
		//moveToAreaTweens[index].Play();

		//for (int i = 0; i < gameAreaCanvases.Length; i++)
		//{
		//	gameAreaCanvases[i].gameObject.SetActive(i == index);
		//}
	}

	public void OnUpgradesButtonClick()
	{
		upgradesMenu.SetActive(!upgradesMenu.activeSelf);
	}

	public void OnSellAllButtonClick()
	{
		int moneyGained = 0;

		foreach (Resource resource in Enum.GetValues(typeof(Resource)))
		{
			if (resource != Resource.Money)
			{
				int i = (int)resource;
				moneyGained += resources[i];
				resources[i] = 0;
				UpdateResourceVisuals(resource);
			}
		}

		resources[(int)Resource.Money] += moneyGained;
		UpdateResourceVisuals(Resource.Money);
	}

	#endregion

	#region Upgrades

	private void CheckUpgrades()
	{
		for (int i = 0; i < UpgradesManager.Upgrades.Length; i++)
		{
			if (UpgradesManager.UpgradesUnlocked[i])
			{
				CheckUnlockUpgrade(UpgradesManager.Upgrades[i].UpgradeType);
			}
		}
	}

	private void CheckUnlockUpgrade(UpgradeType upgradeType)
	{
		int val;
		bool isMaxRank;

		switch (upgradeType)
		{
			case UpgradeType.CornStorage:
				val = (int)UpgradesManager.GetCurrentRankUpgrade(upgradeType, out isMaxRank).FinalValue;
				SetStorage(Resource.Corn, val);
				break;
			case UpgradeType.WheatStorage:
				val = (int)UpgradesManager.GetCurrentRankUpgrade(upgradeType, out isMaxRank).FinalValue;
				SetStorage(Resource.Wheat, val);
				break;
			case UpgradeType.FishStorage:
				val = (int)UpgradesManager.GetCurrentRankUpgrade(upgradeType, out isMaxRank).FinalValue;
				SetStorage(Resource.Fish, val);
				break;
			case UpgradeType.UnlockWheat:
				GameAreaUnlocker.WheatSetActive(true);
				break;
			case UpgradeType.UnlockFish:
				GameAreaUnlocker.FishSetActive(true);
				break;
			case UpgradeType.MutationRate:
				float rate = UpgradesManager.GetCurrentRankUpgrade(upgradeType, out isMaxRank).FinalValue / 100;
				cornController.MutationRate = rate;
				wheatController.MutationRate = rate;
				break;
			case UpgradeType.Elitism:
				int elitism = (int)UpgradesManager.GetCurrentRankUpgrade(upgradeType, out isMaxRank).FinalValue;
				cornController.Elitism = elitism;
				wheatController.Elitism = elitism;
				break;
			case UpgradeType.CrossoverMethod:
				CrossoverMethod method = (CrossoverMethod)UpgradesManager.GetCurrentRankUpgrade(upgradeType, out isMaxRank).FinalValue;
				cornController.CrossoverMethod = method;
				wheatController.CrossoverMethod = method;
				break;
		}
	}

	#endregion

	#region Exit Game
	public void OnExitGameButtonClick()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
#endregion
}
