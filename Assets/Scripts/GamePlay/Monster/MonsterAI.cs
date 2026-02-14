using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform player;
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Animator animator;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;

    [Header("Animation Settings")]
    [SerializeField] private string moveBoolParameter = "IsMove";
    [SerializeField] private string attackBoolParameter = "Attack";

    private int moveBoolHash = -1;
    private int attackBoolHash = -1;
    private bool hasMoveBoolParameter;
    private bool hasAttackBoolParameter;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        DifficultyManager difficultyManager = DifficultyManager.Instance;
        if (difficultyManager == null)
        {
            difficultyManager = FindObjectOfType<DifficultyManager>();
        }

        float moveSpeedMultiplier = difficultyManager != null ? difficultyManager.MoveSpeedMultiplier : 1f;

        if (monsterData != null)
        {
            moveSpeed = monsterData.MoveSpeed * moveSpeedMultiplier;
            turnSpeed = monsterData.TurnSpeed;
            attackRange = monsterData.AttackRange;
        }
        else
        {
            moveSpeed *= moveSpeedMultiplier;
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        CacheAnimatorParameters();
    }

    private void FixedUpdate()
    {
        if (rb == null || player == null)
        {
            return;
        }

        Vector3 toPlayer = player.position - rb.position;
        toPlayer.y = 0f;

        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            SetMoveAnimationState(false);
            SetAttackAnimationState(true);
            return;
        }

        SetAttackAnimationState(false);
        SetMoveAnimationState(true);

        if (toPlayer.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        Vector3 moveDirection = toPlayer.normalized;
        Vector3 nextPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        Quaternion nextRotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(nextRotation);
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    private void CacheAnimatorParameters()
    {
        if (animator == null)
        {
            return;
        }

        moveBoolHash = Animator.StringToHash(moveBoolParameter);
        attackBoolHash = Animator.StringToHash(attackBoolParameter);

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.nameHash == moveBoolHash)
            {
                hasMoveBoolParameter = true;
            }

            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.nameHash == attackBoolHash)
            {
                hasAttackBoolParameter = true;
            }
        }
    }

    private void SetAttackAnimationState(bool isAttacking)
    {
        if (animator == null || !hasAttackBoolParameter)
        {
            return;
        }

        animator.SetBool(attackBoolHash, isAttacking);
    }

    private void SetMoveAnimationState(bool isMoving)
    {
        if (animator == null || !hasMoveBoolParameter)
        {
            return;
        }

        animator.SetBool(moveBoolHash, isMoving);
    }
}
