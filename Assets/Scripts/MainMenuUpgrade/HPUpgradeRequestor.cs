using UnityEngine;

public class HPUpgradeRequestor : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    public void RequestUpgrade()
    {
        UpgradeEventBus.RequestUpgrade(new UpgradeRequest(UpgradeType.HP, amount));
    }
}
