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
    [SerializeField] private Collider attackCollider;

    [Header("Animation Settings")]
    [SerializeField] private string moveStateName = "Move";
    [SerializeField] private string attackStateName = "AttackAnimation";
    [SerializeField] private float attackAnimationLockTime = 0.35f;

    private int moveStateHash;
    private int attackStateHash;
    private bool wasMoving;
    private float attackAnimationLockRemaining;

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

        if (attackCollider == null)
        {
            attackCollider = GetComponent<Collider>();
        }

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
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
            UpdateMovementAnimation(false);
            TryPlayAttackAnimation();
            return;
        }

        UpdateMovementAnimation(true);

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

    public void OnAttackEnd()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void OnDisable()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void CacheAnimatorParameters()
    {
        if (animator == null)
        {
            return;
        }

        moveStateHash = ResolveStateHash(moveStateName);
        attackStateHash = ResolveAttackStateHash();
    }

    private void TryPlayAttackAnimation()
    {
        if (attackAnimationLockRemaining > 0f)
        {
            attackAnimationLockRemaining -= Time.fixedDeltaTime;
            return;
        }

        if (animator == null || attackStateHash == 0 || !animator.HasState(0, attackStateHash))
        {
            return;
        }

        attackAnimationLockRemaining = Mathf.Max(attackAnimationLockTime, 0f);
        animator.CrossFade(attackStateHash, 0.05f, 0);

        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }

    private void UpdateMovementAnimation(bool isMoving)
    {
        if (animator == null)
        {
            return;
        }

        if (isMoving == wasMoving)
        {
            return;
        }

        wasMoving = isMoving;

        if (!isMoving)
        {
            return;
        }

        if (moveStateHash == 0 || !animator.HasState(0, moveStateHash))
        {
            return;
        }

        animator.CrossFade(moveStateHash, 0.1f, 0);
    }

    private int ResolveAttackStateHash()
    {
        if (animator == null)
        {
            return 0;
        }

        int configuredHash = ResolveStateHash(attackStateName);
        if (configuredHash != 0)
        {
            return configuredHash;
        }

        return 0;
    }

    private int ResolveStateHash(string stateName)
    {
        if (animator == null || string.IsNullOrWhiteSpace(stateName))
        {
            return 0;
        }

        int shortNameHash = Animator.StringToHash(stateName);
        if (animator.HasState(0, shortNameHash))
        {
            return shortNameHash;
        }

        int fullPathHash = Animator.StringToHash($"Base Layer.{stateName}");
        if (animator.HasState(0, fullPathHash))
        {
            return fullPathHash;
        }

        return 0;
    }
}
