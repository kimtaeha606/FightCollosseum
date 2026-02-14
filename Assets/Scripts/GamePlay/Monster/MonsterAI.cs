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
    [SerializeField] private string moveSpeedParameter = "MoveSpeed";
    [SerializeField] private string attackTriggerParameter = "Attack";
    [SerializeField] private string idleStateName = "Idle";
    [SerializeField] private string moveStateName = "Move";
    [SerializeField] private string attackStateName = "Attack";
    [SerializeField] private float attackInterval = 1.25f;

    private int moveBoolHash = -1;
    private int moveSpeedHash = -1;
    private int attackTriggerHash = -1;
    private int idleStateHash = -1;
    private int moveStateHash = -1;
    private int attackStateHash = -1;
    private bool hasMoveBoolParameter;
    private bool hasMoveSpeedParameter;
    private bool hasAttackTriggerParameter;
    private float nextAttackTime;

    private enum AnimationState
    {
        None,
        Idle,
        Move,
        Attack
    }

    private AnimationState currentAnimationState = AnimationState.None;

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
            UpdateMovementAnimation(false, 0f);
            return;
        }

        Vector3 toPlayer = player.position - rb.position;
        toPlayer.y = 0f;

        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            UpdateMovementAnimation(false, 0f);
            if (!TryPlayAttackAnimation())
            {
                TryPlayIdleAnimation();
            }

            return;
        }

        if (toPlayer.sqrMagnitude <= Mathf.Epsilon)
        {
            UpdateMovementAnimation(false, 0f);
            TryPlayIdleAnimation();
            return;
        }

        Vector3 moveDirection = toPlayer.normalized;
        Vector3 nextPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        Quaternion nextRotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(nextRotation);

        UpdateMovementAnimation(true, moveSpeed);
        TryPlayMoveAnimation();
        EnsureMoveAnimationLoop();
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
        moveSpeedHash = Animator.StringToHash(moveSpeedParameter);
        attackTriggerHash = Animator.StringToHash(attackTriggerParameter);
        idleStateHash = Animator.StringToHash(idleStateName);
        moveStateHash = Animator.StringToHash(moveStateName);
        attackStateHash = Animator.StringToHash(attackStateName);

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.nameHash == moveBoolHash)
            {
                hasMoveBoolParameter = true;
            }

            if (parameter.type == AnimatorControllerParameterType.Float && parameter.nameHash == moveSpeedHash)
            {
                hasMoveSpeedParameter = true;
            }

            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.nameHash == attackTriggerHash)
            {
                hasAttackTriggerParameter = true;
            }
        }
    }

    private void UpdateMovementAnimation(bool isMoving, float speed)
    {
        if (animator == null)
        {
            return;
        }

        if (hasMoveBoolParameter)
        {
            animator.SetBool(moveBoolHash, isMoving);
        }

        if (hasMoveSpeedParameter)
        {
            animator.SetFloat(moveSpeedHash, speed);
        }
    }

    private void TryPlayIdleAnimation()
    {
        if (animator == null || currentAnimationState == AnimationState.Idle)
        {
            return;
        }

        if (idleStateHash != 0 && animator.HasState(0, idleStateHash))
        {
            animator.CrossFade(idleStateHash, 0.1f);
        }

        currentAnimationState = AnimationState.Idle;
    }

    private void TryPlayMoveAnimation()
    {
        if (animator == null)
        {
            return;
        }

        if (moveStateHash == 0 || !animator.HasState(0, moveStateHash))
        {
            currentAnimationState = AnimationState.Move;
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool alreadyOnMove = stateInfo.shortNameHash == moveStateHash && !animator.IsInTransition(0);
        if (alreadyOnMove && currentAnimationState == AnimationState.Move)
        {
            return;
        }

        animator.CrossFade(moveStateHash, 0.1f);

        currentAnimationState = AnimationState.Move;
    }

    private void EnsureMoveAnimationLoop()
    {
        if (animator == null || moveStateHash == 0 || !animator.HasState(0, moveStateHash) || animator.IsInTransition(0))
        {
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash != moveStateHash)
        {
            animator.CrossFade(moveStateHash, 0.05f);
            return;
        }

        // Some imported clips are not set to loop. Restart move clip while moving.
        if (stateInfo.normalizedTime >= 0.98f)
        {
            animator.Play(moveStateHash, 0, 0f);
        }
    }

    private bool TryPlayAttackAnimation()
    {
        if (animator == null || Time.time < nextAttackTime)
        {
            return false;
        }

        nextAttackTime = Time.time + Mathf.Max(0.1f, attackInterval);

        if (hasAttackTriggerParameter)
        {
            animator.SetTrigger(attackTriggerHash);
            currentAnimationState = AnimationState.Attack;
            return true;
        }

        if (attackStateHash != 0 && animator.HasState(0, attackStateHash))
        {
            animator.CrossFade(attackStateHash, 0.05f);
            currentAnimationState = AnimationState.Attack;
            return true;
        }

        return false;
    }
}
