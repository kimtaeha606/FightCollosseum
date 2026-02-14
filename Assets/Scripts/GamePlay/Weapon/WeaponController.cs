using UnityEngine;
using UnityEngine.InputSystem; // New Input System 사용 시 필요

public class WeaponController : MonoBehaviour
{
    // 현재 장착된 무기 (IWeapon 인터페이스를 가진 컴포넌트)
    private IWeapon currentWeapon;
    private bool isPressing = false;

    private void Start()
    {
        // 예시: 자식 오브젝트에 붙은 무기를 자동으로 찾아 연결
        // 나중에 무기 교체 시스템을 만든다면 이 부분을 수정하면 됩니다.
        currentWeapon = GetComponentInChildren<IWeapon>();
    }

    // Input Action에서 연결할 함수 (SendMessage 또는 Invoke Unity Events)
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (currentWeapon == null) return;

        if (context.started) // 버튼을 누른 순간
        {
            isPressing = true;
            currentWeapon.OnInputStart();
        }
        else if (context.canceled) // 버튼에서 손을 뗀 순간
        {
            isPressing = false;
            currentWeapon.OnInputEnd();
        }
    }

    private void Update()
    {
        // 버튼을 누르고 있는 동안 매 프레임 OnInputHold 호출
        if (isPressing && currentWeapon != null)
        {
            currentWeapon.OnInputHold(Time.deltaTime);
        }
    }
}