using System;

public enum UpgradeType
{
    Attack,
    HP
}

[Serializable]
public struct UpgradeRequest
{
    public UpgradeType Type;
    public int Amount;

    public UpgradeRequest(UpgradeType type, int amount)
    {
        Type = type;
        Amount = Math.Max(1, amount);
    }
}

[Serializable]
public struct UpgradeResult
{
    public bool Success;
    public UpgradeType Type;
    public int Amount;
    public int TotalCost;
    public string Reason;

    public UpgradeResult(bool success, UpgradeType type, int amount, int totalCost, string reason)
    {
        Success = success;
        Type = type;
        Amount = amount;
        TotalCost = totalCost;
        Reason = reason;
    }
}

[Serializable]
public struct PlayerStatsSnapshot
{
    public int AttackLevel;
    public int HpLevel;
    public float AttackPower;
    public float MaxHp;

    public PlayerStatsSnapshot(int attackLevel, int hpLevel, float attackPower, float maxHp)
    {
        AttackLevel = attackLevel;
        HpLevel = hpLevel;
        AttackPower = attackPower;
        MaxHp = maxHp;
    }
}
