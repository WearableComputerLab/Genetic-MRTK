using UnityEngine;
using UnityEngine.UI;

public class UpgradeTooltip : MonoBehaviour
{
	[SerializeField] Text nameText;
	[SerializeField] Text costText;
	[SerializeField] Text descriptionText;

	private Upgrade currentUpgrade;
	private bool isUnlocked;

	public void SetUpgrade(Upgrade upgrade, bool isUnlocked = false)
	{
		if (upgrade == currentUpgrade && isUnlocked == this.isUnlocked) {
			return;
		}

		currentUpgrade = upgrade;
		this.isUnlocked = isUnlocked;

		nameText.text = string.Format("{0} (Rank {1})", upgrade.UpgradeName, upgrade.Rank);
		costText.text = isUnlocked ? "Unlocked" : string.Format("{0}$",upgrade.Cost);
		descriptionText.text = upgrade.Description;
	}
}
