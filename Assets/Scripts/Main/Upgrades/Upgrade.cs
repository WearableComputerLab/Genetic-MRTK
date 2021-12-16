#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu]
public class Upgrade : ScriptableObject
{
#if UNITY_EDITOR
	[Header("Naming Options")]
	[SerializeField] bool autoName = true;
	[SerializeField] bool autoNameAssetFile = true;
#endif

	[Header("Properties")]
	[SerializeField] UpgradeType upgradeType;
	[SerializeField] string upgradeName;
#if UNITY_EDITOR
	[SerializeField] string description = "Increases {1} to {0}";
#endif
	[SerializeField] int rank = 1;
	[SerializeField] int cost;
	[SerializeField] float finalValue;
	[SerializeField] Upgrade[] prerequisites;

	[Header("Auto Properties")]
	[SerializeField] string finalDescription;

	public UpgradeType UpgradeType { get { return upgradeType; } }
	public string UpgradeName { get { return upgradeName; } }
	public string Description { get { return finalDescription; } }
	public int Rank { get { return rank; } }
	public int Cost { get { return cost; } }
	public float FinalValue { get { return finalValue; } }
	public Upgrade[] Prerequisites { get { return prerequisites; } }

#if UNITY_EDITOR
	void OnValidate()
	{
		try {
			finalDescription = string.Format(description, FinalValue, upgradeType.ToString().Readable());
		} catch {
			finalDescription = description;
		}

		if (autoName)
		{
			upgradeName = UpgradeType.ToString().Readable();
		}

		if (autoNameAssetFile && this.name != upgradeName)
		{
			string fullName = string.Format("{0} (Rank {1})", upgradeName, rank);
			AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), fullName);
		}
	}
#endif
}
