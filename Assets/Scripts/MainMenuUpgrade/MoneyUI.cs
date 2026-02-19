using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private string moneyFormat = "{0}";
    [SerializeField] private string systemsObjectName = "MainMenuSystems";

    private void Awake()
    {
        if (moneyText == null)
        {
            moneyText = GetComponent<TMP_Text>();
        }

        if (moneyManager == null && !string.IsNullOrEmpty(systemsObjectName))
        {
            GameObject systems = GameObject.Find(systemsObjectName);
            if (systems != null)
            {
                moneyManager = systems.GetComponent<MoneyManager>();
            }
        }

        RefreshMoney();
    }

    private void OnEnable()
    {
        if (moneyManager == null)
        {
            return;
        }

        moneyManager.MoneyChanged += HandleMoneyChanged;
        RefreshMoney();
    }

    private void OnDisable()
    {
        if (moneyManager == null)
        {
            return;
        }

        moneyManager.MoneyChanged -= HandleMoneyChanged;
    }

    private void HandleMoneyChanged(int currentMoney)
    {
        if (moneyText == null)
        {
            return;
        }

        moneyText.text = string.Format(moneyFormat, currentMoney);
    }

    private void RefreshMoney()
    {
        if (moneyManager == null || moneyText == null)
        {
            return;
        }

        moneyText.text = string.Format(moneyFormat, moneyManager.CurrentMoney);
    }
}
