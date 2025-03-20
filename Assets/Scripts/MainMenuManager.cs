using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    public GameObject returnToMenuButton;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive between scenes
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        returnToMenuButton.SetActive(true);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene"); // Ensure the scene is named "MenuScene"
        returnToMenuButton.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
