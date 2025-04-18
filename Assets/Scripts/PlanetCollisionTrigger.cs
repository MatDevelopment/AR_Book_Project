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

    [SerializeField]
    private SolarSystemManager solarSystemManager;

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
                solarSystemManager.StopAllPlanetsRotating();
                break;
            case "Mars":
                marsEnterPanel.SetActive(true);
                solarSystemManager.StopAllPlanetsRotating();
                break;
        }
    }

    public void ClosePanel()
    {
        switch (currentPlanet)
        {
            case "Earth":
                earthEnterPanel.SetActive(false);
                solarSystemManager.StartAllPlanetsRotating();
                break;
            case "Mars":
                marsEnterPanel.SetActive(false);
                solarSystemManager.StartAllPlanetsRotating();
                break;
        }
    }
}
