using UnityEngine;

public class MonsterAttackAnimationEventRelay : MonoBehaviour
{
    [SerializeField] private MonsterAI monsterAI;

    private void Awake()
    {
        if (monsterAI == null)
        {
            monsterAI = GetComponentInParent<MonsterAI>();
        }
    }

    // Bind this function in the monster attack animation event.
    public void OnAttackEnd()
    {
        if (monsterAI == null)
        {
            monsterAI = GetComponentInParent<MonsterAI>();
        }

        monsterAI?.OnAttackEnd();
    }
}
