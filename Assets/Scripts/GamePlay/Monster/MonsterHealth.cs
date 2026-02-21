using System;
using UnityEngine;

public class MonsterHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private float maxHp = 100f;

    public float CurrentHp { get; private set; }
    public bool IsDead => CurrentHp <= 0f;
    public event Action Died;
    public static event Action<int> AnyMonsterDied;

    private void Awake()
    {
        DifficultyManager difficultyManager = DifficultyManager.Instance;
        if (difficultyManager == null)
        {
            difficultyManager = FindObjectOfType<DifficultyManager>();
        }

        float hpMultiplier = difficultyManager != null ? difficultyManager.HpMultiplier : 1f;

        if (monsterData != null)
        {
            maxHp = monsterData.MaxHp * hpMultiplier;
        }
        else
        {
            maxHp *= hpMultiplier;
        }

        CurrentHp = Mathf.Max(0f, maxHp);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0f || IsDead)
        {
            return;
        }

        CurrentHp = Mathf.Max(0f, CurrentHp - damage);

        if (IsDead)
        {
            Died?.Invoke();
            int killScore = monsterData != null ? monsterData.KillScore : 0;
            AnyMonsterDied?.Invoke(killScore);
            Destroy(gameObject);
        }
    }
}
