using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void OnClickMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
