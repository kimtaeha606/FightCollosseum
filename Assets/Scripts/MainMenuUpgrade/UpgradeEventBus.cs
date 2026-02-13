using System;

public static class UpgradeEventBus
{
    public static event Action<UpgradeRequest> UpgradeRequested;
    public static event Action<UpgradeResult> UpgradeProcessed;
    public static event Action<PlayerStatsSnapshot> StatsPublished;

    public static PlayerStatsSnapshot LastPublishedStats { get; private set; }
    public static bool HasPublishedStats { get; private set; }

    public static void RequestUpgrade(UpgradeRequest request)
    {
        UpgradeRequested?.Invoke(request);
    }

    public static void PublishUpgradeResult(UpgradeResult result)
    {
        UpgradeProcessed?.Invoke(result);
    }

    public static void PublishStats(PlayerStatsSnapshot snapshot)
    {
        LastPublishedStats = snapshot;
        HasPublishedStats = true;
        StatsPublished?.Invoke(snapshot);
    }
}
