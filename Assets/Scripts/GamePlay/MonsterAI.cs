using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform player;
    [SerializeField] private MonsterData monsterData;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
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

            // TODO: Attack logic here when the monster is within attackRange of the player.
            return;
        }

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
}
