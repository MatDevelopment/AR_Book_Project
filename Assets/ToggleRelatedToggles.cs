using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UI.Toggle;

public class ToggleRelatedToggles : MonoBehaviour
{
    public List<Toggle> otherToggles = new List<Toggle>();

    private void Start()
    {

        Toggle.ToggleEvent toggleEvent = GetComponent<Toggle>().onValueChanged;
        toggleEvent.AddListener(delegate { GetComponent<Toggle>().GetComponent<ToggleRelatedToggles>().ToggleOthersOff(); });
    }
    public void ToggleOthersOff()
    {
        foreach (Toggle toggle in otherToggles)
        {
        
            if (toggle != GetComponent<Toggle>())
            {
                toggle.isOn = false;
            }
        }
    
    }
}
