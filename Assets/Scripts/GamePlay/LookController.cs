using UnityEngine;
using UnityEngine.InputSystem;

public class LookController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 120f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool invertY;
    [SerializeField] private bool lockCursorOnStart = true;

    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    public Vector3 PlanarForward
    {
        get
        {
            Transform basis = cameraPivot != null ? cameraPivot : transform;
            Vector3 forward = Vector3.ProjectOnPlane(basis.forward, Vector3.up);
            return forward.sqrMagnitude > 0.0001f ? forward.normalized : transform.forward;
        }
    }

    public Vector3 PlanarRight
    {
        get
        {
            Transform basis = cameraPivot != null ? cameraPivot : transform;
            Vector3 right = Vector3.ProjectOnPlane(basis.right, Vector3.up);
            return right.sqrMagnitude > 0.0001f ? right.normalized : transform.right;
        }
    }

    private void Awake()
    {
        if (lockCursorOnStart)
        {
            SetCursorLock(true);
        }

        yaw = transform.eulerAngles.y;

        if (cameraPivot != null)
        {
            float localPitch = cameraPivot.localEulerAngles.x;
            if (localPitch > 180f)
            {
                localPitch -= 360f;
            }

            pitch = Mathf.Clamp(localPitch, minPitch, maxPitch);
        }
    }

    private void LateUpdate()
    {
        UpdateCursorLockInput();

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        Vector2 delta = lookInput * lookSensitivity * deltaTime;

        yaw += delta.x;
        pitch += invertY ? delta.y : -delta.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lookInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            lookInput = Vector2.zero;
        }
    }

    private void UpdateCursorLockInput()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetCursorLock(false);
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            SetCursorLock(true);
        }
    }

    private static void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
