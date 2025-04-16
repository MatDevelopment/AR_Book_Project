using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class PlanetCollisionTrigger : MonoBehaviour
{
    // Script should sit on the rocket that collides with the planets

    [SerializeField]
    string currentPlanet = "planetName";
    
    [SerializeField]
    GameObject marsEnterPanel;

    [SerializeField]
    GameObject earthEnterPanel;

    private void Start()
    {
        marsEnterPanel.SetActive(false);
        earthEnterPanel.SetActive(false);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Rocket")
        {
            ShowExercisePanel();
        }
    }

    public void LoadExercise(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ShowExercisePanel()
    {
        switch (currentPlanet)
        {
            case "Earth":
                earthEnterPanel.SetActive(true);
                break;
            case "Mars":
                marsEnterPanel.SetActive(true);
                break;
        }
    }

    public void ClosePanel()
    {
        switch (currentPlanet)
        {
            case "Earth":
                earthEnterPanel.SetActive(false);
                break;
            case "Mars":
                marsEnterPanel.SetActive(false);
                break;
        }
    }
}
