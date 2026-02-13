using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName;
    
    [Header("전투 수치")]
    public int baseDamage;      // 무기 자체의 기본 데미지
    public float attackRange;   // 필요 시 사거리
    public float attackSpeed = 1f; // 애니메이션 속도 조절용

    [Header("시각적 요소")]
    public GameObject weaponPrefab; // 무기 모델링
    public AudioClip attackSound;   // 공격 소리
}