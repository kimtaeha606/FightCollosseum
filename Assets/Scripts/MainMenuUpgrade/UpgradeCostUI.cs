using TMPro;
using UnityEngine;

public class UpgradeCostUI : MonoBehaviour
{
    [SerializeField] private TMP_Text attackCostText;
    [SerializeField] private TMP_Text hpCostText;
    [SerializeField] private int attackAmount = 1;
    [SerializeField] private int hpAmount = 1;
    [SerializeField] private string numberFormat = "{0}";
    [SerializeField] private string attackTextObjectName = "AttackCostText";
    [SerializeField] private string hpTextObjectName = "HpCostText";

    private void Awake()
    {
        AutoBindTextReferences();
        RefreshCosts();
    }

    private void OnEnable()
    {
        UpgradeEventBus.UpgradeProcessed += HandleUpgradeProcessed;
        RefreshCosts();
    }

    private void OnDisable()
    {
        UpgradeEventBus.UpgradeProcessed -= HandleUpgradeProcessed;
    }

    private void HandleUpgradeProcessed(UpgradeResult _)
    {
        RefreshCosts();
    }

    private void AutoBindTextReferences()
    {
        if (attackCostText == null)
        {
            attackCostText = FindTextByName(attackTextObjectName);
        }

        if (hpCostText == null)
        {
            hpCostText = FindTextByName(hpTextObjectName);
        }
    }

    private TMP_Text FindTextByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return null;
        }

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];
            if (text != null && text.gameObject.name == objectName)
            {
                return text;
            }
        }

        TMP_Text[] sceneTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
        for (int i = 0; i < sceneTexts.Length; i++)
        {
            TMP_Text text = sceneTexts[i];
            if (text == null)
            {
                continue;
            }

            if (!text.gameObject.scene.IsValid())
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

    private void RefreshCosts()
    {
        if (attackCostText != null)
        {
            int attackCost = UpgradeCostRepository.GetTotalCost(UpgradeType.Attack, Mathf.Max(1, attackAmount));
            attackCostText.text = string.Format(numberFormat, attackCost);
        }

        if (hpCostText != null)
        {
            int hpCost = UpgradeCostRepository.GetTotalCost(UpgradeType.HP, Mathf.Max(1, hpAmount));
            hpCostText.text = string.Format(numberFormat, hpCost);
        }
    }
}
