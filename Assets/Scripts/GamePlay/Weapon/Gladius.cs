using UnityEngine;
using System.Collections.Generic;

public class Gladius : MonoBehaviour, IWeapon
{
    [SerializeField] private Collider targetCollider;
    [SerializeField] private WeaponData data;
    [SerializeField] private CharacterApplier characterApplier;
    private int finalDamage; // 최종 계산된 데미지

    private readonly HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

    private void Awake()
    {
        if (characterApplier == null)
        {
            characterApplier = GetComponentInParent<CharacterApplier>();
        }

        if (targetCollider == null)
        {
            targetCollider = GetComponent<Collider>();
        }

        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }

        RecalculateDamage();
    }

    public void OnInputStart()
    {
        RecalculateDamage();
        hitTargets.Clear();

        if (targetCollider != null)
        {
            targetCollider.enabled = true;
        }
    }

    public void OnInputHold(float deltaTime)
    {
    }

    public void OnInputEnd()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }

        if (other.transform.root == transform.root)
        {
            return;
        }

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target == null)
        {
            return;
        }

        if (hitTargets.Contains(target))
        {
            return;
        }

        hitTargets.Add(target);
        target.TakeDamage(finalDamage);
    }

    private void RecalculateDamage()
    {
        int baseDamage = data != null ? data.baseDamage : 0;
        float attackPower = 0f;

        if (characterApplier != null && characterApplier.HasStats)
        {
            attackPower = characterApplier.CurrentStats.AttackPower;
        }

        finalDamage = Mathf.Max(0, baseDamage + Mathf.RoundToInt(attackPower));
    }
}
