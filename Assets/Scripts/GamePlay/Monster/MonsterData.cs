using UnityEngine;

public enum MonsterType
{
    Grunt = 0,
    Fast = 1,
    Tank = 2,
    Boss = 3
}

[CreateAssetMenu(fileName = "MonsterData", menuName = "GamePlay/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private MonsterType monsterType = MonsterType.Grunt;

    [Header("Base Stats")]
    [Min(1f)] [SerializeField] private float maxHp = 100f;
    [Min(0f)] [SerializeField] private float moveSpeed = 3f;
    [Min(0f)] [SerializeField] private float turnSpeed = 10f;
    [Min(0f)] [SerializeField] private float attackRange = 2f;
    [Min(0)] [SerializeField] private int killScore = 100;

    public MonsterType MonsterType => monsterType;
    public float MaxHp => maxHp;
    public float MoveSpeed => moveSpeed;
    public float TurnSpeed => turnSpeed;
    public float AttackRange => attackRange;
    public int KillScore => killScore;
}
