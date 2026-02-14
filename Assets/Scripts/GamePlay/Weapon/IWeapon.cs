public interface IWeapon
{
    // 1. 누르는 순간 (망치 휘두르기 시작, 활 시위 당기기 시작)
    void OnInputStart();

    // 2. 누르고 있는 중 (활 차징 게이지 상승, 기관총 연사)
    void OnInputHold(float deltaTime);

    // 3. 떼는 순간 (활 발사, 망치 후딜레이 처리)
    void OnInputEnd();
}