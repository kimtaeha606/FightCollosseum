using UnityEngine;

public class AttackUpgradeRequestor : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    public void RequestUpgrade()
    {
        UpgradeEventBus.RequestUpgrade(new UpgradeRequest(UpgradeType.Attack, amount));
    }
}
