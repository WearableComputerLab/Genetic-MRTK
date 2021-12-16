using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
	[SerializeField] UpgradesManager UpgradesManager;
	[SerializeField] UpgradeTooltip UpgradeTooltipCurrentRank;
	[SerializeField] UpgradeTooltip UpgradeTooltipNextRank;
	[SerializeField] Image border;
	[SerializeField] Color borderNormalColor = new Color(150/255f, 150/255f, 150/255f, 1);
	[SerializeField] Color borderMaxRankColor = new Color(175/255f, 150/255f, 90/255f, 1);
	[SerializeField] Button button;
	[SerializeField] Text nameText;
	[SerializeField] Text rankText;
	[SerializeField] UpgradeType upgradeType;

	private Upgrade upgrade;
	private bool isMaxRank;

	void Start()
	{
		SetUpgrade();
	}

	public void OnUpgradeButtonClick()
	{
		if (UpgradesManager.UnlockUpgrade(upgrade.UpgradeType, upgrade.Rank))
		{
			SetUpgrade();
			ShowTooltip();
		}
	}

	public void OnUpgradeButtonMouseEnter()
	{
		ShowTooltip();
	}

	public void OnUpgradeButtonMouseExit()
	{
		HideTooltip();
	}

	private void SetUpgrade()
	{
		upgrade = UpgradesManager.GetNextRankUpgrade(upgradeType, out isMaxRank);
		nameText.text = upgrade.UpgradeName;
		rankText.text = string.Format("{0}/{1}", isMaxRank ? upgrade.Rank : upgrade.Rank-1, UpgradesManager.NumRanks[upgrade.UpgradeType]);

		if (isMaxRank)
		{
			if (button.interactable) button.interactable = false;
			border.color = borderMaxRankColor;
		}
		else
		{
			border.color = borderNormalColor;
		}
	}

	private void ShowTooltip()
	{
		if (!UpgradeTooltipNextRank.gameObject.activeSelf) UpgradeTooltipNextRank.gameObject.SetActive(true);
		UpgradeTooltipNextRank.SetUpgrade(upgrade, isMaxRank);

		if (isMaxRank) {
			return;
		}

		bool currentIsMax;
		Upgrade currentRankUpgrade = UpgradesManager.GetCurrentRankUpgrade(upgradeType, out currentIsMax);

		if (currentRankUpgrade != null && !currentIsMax)
		{
			if (!UpgradeTooltipCurrentRank.gameObject.activeSelf) UpgradeTooltipCurrentRank.gameObject.SetActive(true);
			UpgradeTooltipCurrentRank.SetUpgrade(currentRankUpgrade, isUnlocked: true);
		}
	}

	private void HideTooltip()
	{
		if (UpgradeTooltipNextRank.gameObject.activeSelf) UpgradeTooltipNextRank.gameObject.SetActive(false);
		if (UpgradeTooltipCurrentRank.gameObject.activeSelf) UpgradeTooltipCurrentRank.gameObject.SetActive(false);
	}
}
