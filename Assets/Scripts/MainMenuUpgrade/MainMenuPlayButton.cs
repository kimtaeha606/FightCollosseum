using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPlayButton : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "GameScene";
    [SerializeField] private UpgradeManager upgradeManager;

    public void OnClickPlay()
    {
        if (upgradeManager != null)
        {
            upgradeManager.PublishCurrentStats();
        }
        else
        {
            UpgradeEventBus.PublishStats(PlayerStatsRepository.LoadOrCreate());
        }

        SceneManager.LoadScene(gameplaySceneName);
    }
}
