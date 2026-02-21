using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MonsterWeaponDamageDealer : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private bool oneHitPerEnable = true;

    private readonly HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

    private void OnEnable()
    {
        hitTargets.Clear();
    }

    private void OnDisable()
    {
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDealDamage(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        TryDealDamage(collision.collider);
    }

    private void TryDealDamage(Collider other)
    {
        if (other == null || damage <= 0)
        {
            return;
        }

        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = other.GetComponentInParent<CharacterController>();
        }

        if (characterController == null)
        {
            return;
        }

        IDamageable target = characterController.GetComponentInParent<IDamageable>();
        if (target == null)
        {
            return;
        }

        if (oneHitPerEnable && hitTargets.Contains(target))
        {
            return;
        }

        hitTargets.Add(target);
        target.TakeDamage(damage);
    }
}
