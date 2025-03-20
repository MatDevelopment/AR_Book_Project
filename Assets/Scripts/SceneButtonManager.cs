using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonManager : MonoBehaviour
{
    public void LoadNewScene(string sceneName)
    {
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.LoadScene(sceneName);
        }
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MenuScene"); // Ensure you have a scene named "MenuScene"
    }
}
