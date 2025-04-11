using UnityEngine;
using UnityEngine.SceneManagement;

public class TapFunctionality_BigBang : MonoBehaviour
{
  public void LoadPlanetScene()
    {
        SceneManager.LoadScene("PlanetsAndStars");
        //FindFirstObjectByType<MainMenuManager>().LoadScene("PlanetsAndStars");
    }
}
