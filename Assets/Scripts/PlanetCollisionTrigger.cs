using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class PlanetCollisionTrigger : MonoBehaviour
{
    // Script should sit on the rocket that collides with the planets

    [SerializeField]
    string currentPlanet = "planetName";

    [Header("UI Elements")]
    [SerializeField]
    GameObject marsEnterPanel;
    [SerializeField]
    GameObject marsEnterPanelVertical;

    [SerializeField]
    GameObject earthEnterPanel;
    [SerializeField]
    GameObject earthEnterPanelVertical;

    [SerializeField]
    GameObject tutorialText;

    [Header("Selection Hologram")]
    [SerializeField] GameObject hologramObject;
    [SerializeField] Material disabledMaterial;
    private bool marsDisabled = false;
    private bool earthDisabled = false;
    private Renderer hologramRenderer;

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
        marsEnterPanelVertical.SetActive(false);
        earthEnterPanel.SetActive(false);
        earthEnterPanelVertical.SetActive(false);
        tutorialText.SetActive(false);

        if (rocketControl.tangibleRotation)
        {
            marsEnterPanel = marsEnterPanelVertical;
            earthEnterPanel = earthEnterPanelVertical;
        }

        // Changing the color of the hologram and disabling if exercise is already completed
        hologramRenderer = hologramObject.GetComponent<Renderer>();

        if (DataLogger.Instance.userCompletedRoverScene && currentPlanet == "Mars")
        {
            marsDisabled = true;
            hologramRenderer.material = disabledMaterial;
        }

        if (DataLogger.Instance.userCompletedDiggingScene && currentPlanet == "Earth")
        {
            earthDisabled = true;
            hologramRenderer.material = disabledMaterial;
        }
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

        if (tutorialText.activeInHierarchy)
        {
            tutorialText.SetActive(false);
        }

        switch (currentPlanet)
        {
            case "Earth":
                if (earthDisabled) { break; }
                earthEnterPanel.SetActive(true);
                solarSystemManager.StopAllPlanetsRotating();
                break;
            case "Mars":
                if (marsDisabled) { break; }
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
