using UnityEngine;

public class XrayTogglePlanetStar : MonoBehaviour
{
    [SerializeField] private GameObject[] Normal_PlanetsAndStars;
    [SerializeField] private GameObject[] Xray_PlanetsAndStars;

    private bool XrayVision_On = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleXrayVision()
    {
        if (XrayVision_On)
        {
            XrayVision_On = false;
        }
        else
        {
            XrayVision_On = true;
        }
        
        for (int i = 0; i < Normal_PlanetsAndStars.Length; i++)
        {
            if (Normal_PlanetsAndStars[i].activeSelf && XrayVision_On)
            {
                Normal_PlanetsAndStars[i].SetActive(false);
                for (int j = 0; j < Xray_PlanetsAndStars.Length; j++)
                {
                    Xray_PlanetsAndStars[j].SetActive(true);
                }
            }
            else if (Normal_PlanetsAndStars[i].activeSelf == false && XrayVision_On == false)
            {
                Normal_PlanetsAndStars[i].SetActive(true);
                for (int k = 0; k < Xray_PlanetsAndStars.Length; k++)
                {
                    Xray_PlanetsAndStars[k].SetActive(false);
                }
            }
        }
    }

    public void CheckXrayStateOnTargetFound()
    {
        if (XrayVision_On)
        {
            for (int i = 0; i < Normal_PlanetsAndStars.Length; i++)
            {
                Normal_PlanetsAndStars[i].SetActive(false);
                Xray_PlanetsAndStars[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < Normal_PlanetsAndStars.Length; i++)
            {
                Normal_PlanetsAndStars[i].SetActive(true);
                Xray_PlanetsAndStars[i].SetActive(false);
            }
        }
    }
    
}
