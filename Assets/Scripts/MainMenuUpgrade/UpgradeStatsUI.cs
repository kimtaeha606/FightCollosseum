using TMPro;
using UnityEngine;

public class UpgradeStatsUI : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private TMP_Text attackPowerText;
    [SerializeField] private TMP_Text maxHpText;
    [SerializeField] private string systemsObjectName = "MainMenuSystems";
    [SerializeField] private string attackTextObjectName = "AttackPowerText";
    [SerializeField] private string hpTextObjectName = "MaxHpText";
    [SerializeField] private string numberFormat = "{0}";

    private void Awake()
    {
        AutoBindReferences();
        RefreshStats();
    }

    private void OnEnable()
    {
        UpgradeEventBus.UpgradeProcessed += HandleUpgradeProcessed;
        RefreshStats();
    }

    private void OnDisable()
    {
        UpgradeEventBus.UpgradeProcessed -= HandleUpgradeProcessed;
    }

    private void HandleUpgradeProcessed(UpgradeResult result)
    {
        if (!result.Success)
        {
            return;
        }

        RefreshStats();
    }

    private void AutoBindReferences()
    {
        if (upgradeManager == null)
        {
            GameObject systems = GameObject.Find(systemsObjectName);
            if (systems != null)
            {
                upgradeManager = systems.GetComponent<UpgradeManager>();
            }
        }

        if (attackPowerText == null)
        {
            attackPowerText = FindTextByName(attackTextObjectName);
        }

        if (maxHpText == null)
        {
            maxHpText = FindTextByName(hpTextObjectName);
        }
    }

    private TMP_Text FindTextByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return null;
        }

        TMP_Text[] sceneTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
        for (int i = 0; i < sceneTexts.Length; i++)
        {
            TMP_Text text = sceneTexts[i];
            if (text == null || !text.gameObject.scene.IsValid())
            {
                continue;
            }

            if (text.gameObject.name == objectName)
            {
                return text;
            }
        }

        return null;
    }

    private void RefreshStats()
    {
        PlayerStatsSnapshot stats = GetLatestStats();

        if (attackPowerText != null)
        {
            attackPowerText.text = string.Format(numberFormat, Mathf.RoundToInt(stats.AttackPower));
        }

        if (maxHpText != null)
        {
            maxHpText.text = string.Format(numberFormat, Mathf.RoundToInt(stats.MaxHp));
        }
    }

    private PlayerStatsSnapshot GetLatestStats()
    {
        if (upgradeManager == null)
        {
            return PlayerStatsRepository.LoadOrCreate();
        }

        PlayerStatsSnapshot managerStats = upgradeManager.CurrentStats;
        if (managerStats.AttackPower <= 0f && managerStats.MaxHp <= 0f)
        {
            return PlayerStatsRepository.LoadOrCreate();
        }

        return managerStats;
    }
}
