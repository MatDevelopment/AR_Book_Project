using UnityEngine;

public class InteractableStarPlanet : MonoBehaviour
{
    private bool informationBoxEnabled = false;

    [SerializeField] private GameObject InformationBoxToEnableDisable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleInformationBox()
    {
        if (informationBoxEnabled == false)
        {
            Debug.Log("INFORMATIONBOX ENABLED");
            informationBoxEnabled = true;
            InformationBoxToEnableDisable.SetActive(true);
        }
        else if (informationBoxEnabled)
        {
            Debug.Log("INFORMATIONBOX DISABLED");
            informationBoxEnabled = false;
            InformationBoxToEnableDisable.SetActive(false);
        }
    }
}
