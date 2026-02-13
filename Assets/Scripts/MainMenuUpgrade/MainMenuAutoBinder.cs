using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAutoBinder : MonoBehaviour
{
    [SerializeField] private string systemsObjectName = "MainMenuSystems";
    [SerializeField] private string attackButtonName = "AttackUpgradeButton";
    [SerializeField] private string hpButtonName = "HpUpgradeButton";
    [SerializeField] private string playButtonName = "PlayButton";

    private void Awake()
    {
        BindManagerReferences();
        BindButtonListeners();
    }

    private void BindManagerReferences()
    {
        GameObject systems = GameObject.Find(systemsObjectName);
        if (systems == null)
        {
            return;
        }

        MoneyManager moneyManager = systems.GetComponent<MoneyManager>();
        UpgradeManager upgradeManager = systems.GetComponent<UpgradeManager>();
        MainMenuPlayButton playButton = FindComponentOnObject<MainMenuPlayButton>(playButtonName);

        if (upgradeManager != null && moneyManager != null)
        {
            SetPrivateField(upgradeManager, "moneyManager", moneyManager);
        }

        if (playButton != null && upgradeManager != null)
        {
            SetPrivateField(playButton, "upgradeManager", upgradeManager);
        }
    }

    private void BindButtonListeners()
    {
        Button attackButton = FindComponentOnObject<Button>(attackButtonName);
        Button hpButton = FindComponentOnObject<Button>(hpButtonName);
        Button playButton = FindComponentOnObject<Button>(playButtonName);

        AttackUpgradeRequestor attackRequestor = FindComponentOnObject<AttackUpgradeRequestor>(attackButtonName);
        HPUpgradeRequestor hpRequestor = FindComponentOnObject<HPUpgradeRequestor>(hpButtonName);
        MainMenuPlayButton playHandler = FindComponentOnObject<MainMenuPlayButton>(playButtonName);

        if (attackButton != null && attackRequestor != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(attackRequestor.RequestUpgrade);
        }

        if (hpButton != null && hpRequestor != null)
        {
            hpButton.onClick.RemoveAllListeners();
            hpButton.onClick.AddListener(hpRequestor.RequestUpgrade);
        }

        if (playButton != null && playHandler != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(playHandler.OnClickPlay);
        }
    }

    private static T FindComponentOnObject<T>(string objectName) where T : Component
    {
        GameObject go = GameObject.Find(objectName);
        if (go == null)
        {
            return null;
        }

        return go.GetComponent<T>();
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        if (target == null)
        {
            return;
        }

        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            return;
        }

        field.SetValue(target, value);
    }
}
