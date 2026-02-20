using UnityEngine;

public static class UpgradeCostRepository
{
    private const int AttackBaseCost = 50;
    private const int HpBaseCost = 50;
    private const int AttackCostPerLevel = 10;
    private const int HpCostPerLevel = 10;

    public static int GetTotalCost(UpgradeType type, int amount)
    {
        int upgradeAmount = Mathf.Max(1, amount);
        PlayerStatsSnapshot stats = PlayerStatsRepository.LoadOrCreate();
        int currentLevel = GetCurrentLevel(type, stats);
        return CalculateProgressiveCost(type, currentLevel, upgradeAmount);
    }

    private static int GetCurrentLevel(UpgradeType type, PlayerStatsSnapshot stats)
    {
        return type == UpgradeType.Attack ? stats.AttackLevel : stats.HpLevel;
    }

    private static int CalculateProgressiveCost(UpgradeType type, int currentLevel, int amount)
    {
        int totalCost = 0;

        for (int i = 0; i < amount; i++)
        {
            int level = currentLevel + i;
            totalCost += GetCostAtLevel(type, level);
        }

        return totalCost;
    }

    private static int GetCostAtLevel(UpgradeType type, int level)
    {
        if (type == UpgradeType.Attack)
        {
            return AttackBaseCost + (AttackCostPerLevel * Mathf.Max(0, level));
        }

        return HpBaseCost + (HpCostPerLevel * Mathf.Max(0, level));
    }
}
