using UnityEngine;

public class TapFunctionality_BigBang : MonoBehaviour
{
  public void LoadPlanetScene()
    {

        FindFirstObjectByType<MainMenuManager>().LoadScene("PlanetsAndStars");
    }
}
