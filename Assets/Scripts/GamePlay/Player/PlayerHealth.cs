using System;
using UnityEngine;

[RequireComponent(typeof(CharacterApplier))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private CharacterApplier characterApplier;
    [SerializeField] private float fallbackHp = 100f;

    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
    public bool IsDead => CurrentHp <= 0f;
    public event Action Died;

    private void Reset()
    {
        characterApplier = GetComponent<CharacterApplier>();
    }

    private void Awake()
    {
        if (characterApplier == null)
        {
            characterApplier = GetComponent<CharacterApplier>();
            MaxHp = Mathf.Max(0f, characterApplier.CurrentStats.MaxHp);
        }

        ApplyStatsOrFallback();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(CurrentHp);
        if (damage <= 0f || IsDead)
        {
            return;
        }

        CurrentHp = Mathf.Max(0f, CurrentHp - damage);
        Debug.Log(CurrentHp);

        if (IsDead)
        {
            Died?.Invoke();
        }
    }

    private void ApplyStatsOrFallback()
    {
        if (characterApplier != null && characterApplier.HasStats)
        {
            MaxHp = Mathf.Max(0f, characterApplier.CurrentStats.MaxHp);
            CurrentHp = MaxHp;
            return;
        }

        MaxHp = Mathf.Max(0f, fallbackHp);
        CurrentHp = MaxHp;
    }
}
