using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MoneyManager moneyManager;

    [Header("Initial Stats")]
    [SerializeField] private float defaultAttackPower = 10f;
    [SerializeField] private float defaultMaxHp = 100f;

    [Header("Upgrade Costs")]
    [SerializeField] private int attackUpgradeCost = 50;
    [SerializeField] private int hpUpgradeCost = 50;

    [Header("Upgrade Values")]
    [SerializeField] private float attackPerLevel = 5f;
    [SerializeField] private float hpPerLevel = 20f;

    private PlayerStatsSnapshot currentStats;

    private void Awake()
    {
        currentStats = PlayerStatsRepository.LoadOrCreate(defaultAttackPower, defaultMaxHp);
    }

    private void OnEnable()
    {
        UpgradeEventBus.UpgradeRequested += HandleUpgradeRequested;
    }

    private void OnDisable()
    {
        UpgradeEventBus.UpgradeRequested -= HandleUpgradeRequested;
    }

    public void PublishCurrentStats()
    {
        UpgradeEventBus.PublishStats(currentStats);
    }

    private void HandleUpgradeRequested(UpgradeRequest request)
    {
        if (moneyManager == null)
        {
            UpgradeEventBus.PublishUpgradeResult(new UpgradeResult(false, request.Type, request.Amount, 0, "MoneyManager is missing."));
            return;
        }

        int amount = Mathf.Max(1, request.Amount);
        int unitCost = request.Type == UpgradeType.Attack ? attackUpgradeCost : hpUpgradeCost;
        int totalCost = unitCost * amount;

        if (!moneyManager.TrySpend(totalCost))
        {
            UpgradeEventBus.PublishUpgradeResult(new UpgradeResult(false, request.Type, amount, totalCost, "Not enough money."));
            return;
        }

        switch (request.Type)
        {
            case UpgradeType.Attack:
                currentStats.AttackLevel += amount;
                currentStats.AttackPower += attackPerLevel * amount;
                break;
            case UpgradeType.HP:
                currentStats.HpLevel += amount;
                currentStats.MaxHp += hpPerLevel * amount;
                break;
        }

        PlayerStatsRepository.Save(currentStats);
        UpgradeEventBus.PublishUpgradeResult(new UpgradeResult(true, request.Type, amount, totalCost, "Upgrade succeeded."));
    }
}
