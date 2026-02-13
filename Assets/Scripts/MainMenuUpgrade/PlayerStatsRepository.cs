using UnityEngine;

public static class PlayerStatsRepository
{
    private const string AttackLevelKey = "PlayerStats.AttackLevel";
    private const string HpLevelKey = "PlayerStats.HpLevel";
    private const string AttackPowerKey = "PlayerStats.AttackPower";
    private const string MaxHpKey = "PlayerStats.MaxHp";

    private const float DefaultAttack = 10f;
    private const float DefaultHp = 100f;

    public static PlayerStatsSnapshot LoadOrCreate(float defaultAttack = DefaultAttack, float defaultHp = DefaultHp)
    {
        if (!PlayerPrefs.HasKey(AttackPowerKey) || !PlayerPrefs.HasKey(MaxHpKey))
        {
            var initial = new PlayerStatsSnapshot(0, 0, defaultAttack, defaultHp);
            Save(initial);
            return initial;
        }

        return new PlayerStatsSnapshot(
            PlayerPrefs.GetInt(AttackLevelKey, 0),
            PlayerPrefs.GetInt(HpLevelKey, 0),
            PlayerPrefs.GetFloat(AttackPowerKey, defaultAttack),
            PlayerPrefs.GetFloat(MaxHpKey, defaultHp));
    }

    public static void Save(PlayerStatsSnapshot snapshot)
    {
        PlayerPrefs.SetInt(AttackLevelKey, snapshot.AttackLevel);
        PlayerPrefs.SetInt(HpLevelKey, snapshot.HpLevel);
        PlayerPrefs.SetFloat(AttackPowerKey, snapshot.AttackPower);
        PlayerPrefs.SetFloat(MaxHpKey, snapshot.MaxHp);
        PlayerPrefs.Save();
    }
}
