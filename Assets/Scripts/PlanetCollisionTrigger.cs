using System.Collections;
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

    [Header("Other Scripts Needed")]
    [SerializeField]
    private SolarSystemManager solarSystemManager;

    [SerializeField]
    private RocketControl rocketControl;
    private float gracePeriod = 2f;
    private bool isInGracePeriod = false;

    private void Start()
    {
        marsEnterPanel.SetActive(false);
        earthEnterPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
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
        if (isInGracePeriod)
        {
            return;
        }

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

        rocketControl.StopRocket();
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

        if (!earthEnterPanel.activeInHierarchy && !marsEnterPanel.activeInHierarchy)
        {
            solarSystemManager.StartAllPlanetsRotating();
        }

        StartCoroutine(StartGracePeriod(gracePeriod));
    }

    IEnumerator StartGracePeriod(float graceTime)
    {
        isInGracePeriod = true;
        yield return new WaitForSeconds(graceTime);
        isInGracePeriod = false;
    }
}
