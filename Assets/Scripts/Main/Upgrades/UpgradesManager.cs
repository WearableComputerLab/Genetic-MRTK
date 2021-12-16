using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
	public Upgrade[] Upgrades { get; private set; }
	public bool[] UpgradesUnlocked { get; private set; }
	public Dictionary<UpgradeType, int> NumRanks { get; private set; }

	public Action<int> BuyUpgrade;
	public Func<int, bool> CanBuyUpgrade;
	public event Action UpgradeUnlockedEvent;


	void Awake()
	{
		Upgrades = Resources.LoadAll<Upgrade>("");
		UpgradesUnlocked = new bool[Upgrades.Length];

		NumRanks = new Dictionary<UpgradeType, int>();
		for (int i = 0; i < Upgrades.Length; i++)
		{
			if (NumRanks.ContainsKey(Upgrades[i].UpgradeType)) {
				NumRanks[Upgrades[i].UpgradeType]++;
			} else {
				NumRanks[Upgrades[i].UpgradeType] = 1;
			}
		}
	}

	public Upgrade GetCurrentRankUpgrade(UpgradeType upgradeType, out bool isMaxRank)
	{
		isMaxRank = true;
		Upgrade upgrade = null;

		for (int i = 0; i < Upgrades.Length; i++)
		{
			if (Upgrades[i].UpgradeType == upgradeType)
			{
				if (!UpgradesUnlocked[i]) {
					isMaxRank = false;
					break;
				}
				upgrade = Upgrades[i];
			}
		}

		return upgrade;
	}

	public Upgrade GetNextRankUpgrade(UpgradeType upgradeType, out bool isMaxRank)
	{
		isMaxRank = true;
		Upgrade upgrade = null;

		for (int i = 0; i < Upgrades.Length; i++)
		{
			if (Upgrades[i].UpgradeType == upgradeType)
			{
				upgrade = Upgrades[i];

				if (!UpgradesUnlocked[i]) {
					isMaxRank = false;
					break;
				}
			}
		}

		return upgrade;
	}

	public bool UnlockUpgrade(UpgradeType upgradeType, int rank)
	{
		for (int i = 0; i < Upgrades.Length; i++)
		{
			Upgrade upgrade = Upgrades[i];

			if (upgrade.UpgradeType == upgradeType)
			{
				if (upgrade.Rank == rank)
				{
					if (UpgradesUnlocked[i])
					{
						Debug.LogError("Upgrade " + upgradeType + " already unlocked");
						return false;
					}

					if (!CanBuyUpgrade(upgrade.Cost))
					{
						return false;
					}

					BuyUpgrade(upgrade.Cost);
					UpgradesUnlocked[i] = true;
					UpgradeUnlockedEvent();
					return true;
				}
				else if (upgrade.Rank < rank && !UpgradesUnlocked[i])
				{
					Debug.LogError("Lower Upgrade rank: " + upgradeType + " rank " + upgrade.Rank + " not yet unlocked");
					return false;
				}
			}
		}

		Debug.LogError("Upgrade " + upgradeType + " not found");
		return false;
	}
}