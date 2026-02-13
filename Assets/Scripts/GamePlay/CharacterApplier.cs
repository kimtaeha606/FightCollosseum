using UnityEngine;

public class CharacterApplier : MonoBehaviour
{
    public PlayerStatsSnapshot CurrentStats { get; private set; }
    public bool HasStats { get; private set; }

    private void OnEnable()
    {
        UpgradeEventBus.StatsPublished += HandleStatsPublished;

        if (UpgradeEventBus.HasPublishedStats)
        {
            ApplyStats(UpgradeEventBus.LastPublishedStats);
        }
    }

    private void OnDisable()
    {
        UpgradeEventBus.StatsPublished -= HandleStatsPublished;
    }

    private void HandleStatsPublished(PlayerStatsSnapshot snapshot)
    {
        ApplyStats(snapshot);
    }

    public void ApplyStats(PlayerStatsSnapshot snapshot)
    {
        CurrentStats = snapshot;
        HasStats = true;
    }
}
