using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MoveController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private LookController lookController;
    [SerializeField] private Animator animator;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float groundedStickForce = -2f;

    [Header("Animation Settings")]
    [SerializeField] private string moveStateName = "Move";
    [SerializeField] private string attackStateName = "AttackAnimation";
    [SerializeField] private string idleStateName = "Idle";
    [SerializeField] private float attackAnimationLockTime = 0.35f;

    private Vector2 moveInput;
    private float verticalVelocity;
    private bool jumpRequested;
    private bool wasMoving;
    private float attackAnimationLockRemaining;
    private int moveStateHash;
    private int attackStateHash;
    private int idleStateHash;

    private void Reset()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        moveStateHash = string.IsNullOrWhiteSpace(moveStateName) ? 0 : Animator.StringToHash(moveStateName);
        attackStateHash = string.IsNullOrWhiteSpace(attackStateName) ? 0 : Animator.StringToHash(attackStateName);
        idleStateHash = string.IsNullOrWhiteSpace(idleStateName) ? 0 : Animator.StringToHash(idleStateName);
    }

    private void Update()
    {
        if (characterController == null)
        {
            return;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        if (lookController != null)
        {
            forward = lookController.PlanarForward;
            right = lookController.PlanarRight;
        }
        else
        {
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        bool isMoving = moveDirection.sqrMagnitude > 0.0001f;
        UpdateMovementAnimation(isMoving);

        bool isGrounded = characterController.isGrounded;

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedStickForce;
        }

        if (isGrounded && jumpRequested)
        {
            float gravityAbs = Mathf.Abs(gravity);
            verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravityAbs);
        }

        jumpRequested = false;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    public void PlayAttackAnimation()
    {
        if (animator == null || attackStateHash == 0 || !animator.HasState(0, attackStateHash))
        {
            return;
        }

        attackAnimationLockRemaining = Mathf.Max(attackAnimationLockTime, 0f);
        animator.CrossFade(attackStateHash, 0.05f, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveInput = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpRequested = true;
        }
    }

    private void UpdateMovementAnimation(bool isMoving)
    {
        if (animator == null)
        {
            return;
        }

        if (attackAnimationLockRemaining > 0f)
        {
            attackAnimationLockRemaining -= Time.deltaTime;
            return;
        }

        if (isMoving == wasMoving)
        {
            return;
        }

        wasMoving = isMoving;

        if (isMoving && moveStateHash != 0 && animator.HasState(0, moveStateHash))
        {
            animator.CrossFade(moveStateHash, 0.1f, 0);
            return;
        }

        if (!isMoving && idleStateHash != 0 && animator.HasState(0, idleStateHash))
        {
            animator.CrossFade(idleStateHash, 0.1f, 0);
        }
    }
}
