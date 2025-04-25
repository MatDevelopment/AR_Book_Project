using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private DrillMovement1 _drillMovement1;
    
    // Layer Transition Text UI
    [SerializeField] private GameObject LayerTransitionTextUI;
    
    // Periodic elements UI panel
    [SerializeField] private GameObject PeriodicElementsUI;
    
    // Temperature UI
    [SerializeField] private TMP_Text temperatureUItext;
    
    // Depth UI
    [SerializeField] private TMP_Text depthUItext;
    
    // CRUST UI
    [SerializeField] private List<GameObject> crust_elementIcons = new List<GameObject>();        //USE A LIST FOR EACH LAYER, SO THAT YOU CAN JUST DRAG AND DROP UI IN INSPECTOR
    [SerializeField] private List<GameObject> crust_elementText = new List<GameObject>();        
    /*[SerializeField] private Image crust_oxygenImage;
    [SerializeField] private Image crust_siliconImage;
    [SerializeField] private Image crust_aluminiumImage;*/
    
    // UPPER MANTLE UI
    [SerializeField] private List<GameObject> upperMantle_elementIcons = new List<GameObject>();        //USE A LIST FOR EACH LAYER, SO THAT YOU CAN JUST DRAG AND DROP UI IN INSPECTOR
    [SerializeField] private List<GameObject> upperMantle_elementText = new List<GameObject>();        
    /*[SerializeField] private Image upperMantle_magnesiumImage;
    [SerializeField] private Image upperMantle_ironImage;
    [SerializeField] private Image upperMantle_calciumImage;*/
    
    // MANTLE UI
    [SerializeField] private List<GameObject> mantle_elementIcons = new List<GameObject>();        //USE A LIST FOR EACH LAYER, SO THAT YOU CAN JUST DRAG AND DROP UI IN INSPECTOR
    [SerializeField] private List<GameObject> mantle_elementText = new List<GameObject>();        
    /*[SerializeField] private Image mantle_siliconImage;
    [SerializeField] private Image mantle_MagnesiumImage;
    [SerializeField] private Image mantle_oxygenImage;*/
    
    // OUTER CORE UI
    [SerializeField] private List<GameObject> outerCore_elementIcons = new List<GameObject>();        //USE A LIST FOR EACH LAYER, SO THAT YOU CAN JUST DRAG AND DROP UI IN INSPECTOR
    [SerializeField] private List<GameObject> outerCore_elementText = new List<GameObject>();        
    /*[SerializeField] private Image outerCore_ironImage;
    [SerializeField] private Image outerCore_nickelImage;
    [SerializeField] private Image outerCore_sulfurImage;*/
    
    // INNER CORE UI
    [SerializeField] private List<GameObject> innerCore_elementIcons = new List<GameObject>();        //USE A LIST FOR EACH LAYER, SO THAT YOU CAN JUST DRAG AND DROP UI IN INSPECTOR
    [SerializeField] private List<GameObject> innerCore_elementText = new List<GameObject>();        
    /*[SerializeField] private Image innerCore_ironImage;
    [SerializeField] private Image innerCore_nickelImage;
    [SerializeField] private Image innerCore_goldImage;*/
    
    // ALL ELEMENT LAYER ICON UI (For disabling when shifting layer)
    private List<GameObject> elementIcons = new List<GameObject>();
    private List<GameObject> elementTexts = new List<GameObject>();

    /*[SerializeField] private TMP_Text DepthUI;
    [SerializeField] private TMP_Text TemperatureUI;
    [SerializeField] private TMP_Text SpeedUI;
    [SerializeField] private TMP_Text LayerElement_1_UI;
    [SerializeField] private TMP_Text LayerElement_2_UI;
    [SerializeField] private TMP_Text LayerElement_3_UI;*/
    // private void OnCollisionEnter(Collision collision)
    // {
    //     switch (collision.gameObject.tag)
    //     {
    //         case "Crust":
    //             LayerElement_1_UI.text = "Silikone";
    //             LayerElement_2_UI.text = "";
    //             LayerElement_3_UI.text = "";
    //             break;
    //         case "Upper Mantle":
    //             LayerElement_1_UI.text = "Jern";
    //             LayerElement_2_UI.text = "";
    //             LayerElement_3_UI.text = "";
    //             break;
    //         case "Mantle":
    //             LayerElement_1_UI.text = "Jern";
    //             LayerElement_2_UI.text = "";
    //             LayerElement_3_UI.text = "";
    //             break;
    //         case "Outer Core":
    //             
    //             break;
    //         case "Inner Core":
    //             break;
    //     }
    //     
    // }

    private void Start()
    {
        LayerTransitionTextUI.SetActive(false);
        
        foreach (GameObject elementIcon in crust_elementIcons)
        {
            elementIcons.Add(elementIcon);
        }
        foreach (GameObject elementIcon in upperMantle_elementIcons)
        {
            elementIcons.Add(elementIcon);
        }
        foreach (GameObject elementIcon in mantle_elementIcons)
        {
            elementIcons.Add(elementIcon);
        }
        foreach (GameObject elementIcon in outerCore_elementIcons)
        {
            elementIcons.Add(elementIcon);
        }
        foreach (GameObject elementIcon in innerCore_elementIcons)
        {
            elementIcons.Add(elementIcon);
        }
        Debug.Log(elementIcons);
        
        
        // ALSO REMEMBER TO ADD THE TEXT ELEMENTS
        foreach (GameObject elementText in crust_elementText)
        {
            elementTexts.Add(elementText);
        }
        foreach (GameObject elementText in upperMantle_elementText)
        {
            elementTexts.Add(elementText);
        }
        foreach (GameObject elementText in mantle_elementText)
        {
            elementTexts.Add(elementText);
        }
        foreach (GameObject elementText in outerCore_elementText)
        {
            elementTexts.Add(elementText);
        }
        foreach (GameObject elementText in innerCore_elementText)
        {
            elementTexts.Add(elementText);
        }
        Debug.Log(elementTexts);
        
        EnableIconsAndTexts(false, elementIcons, elementTexts);
    }

    private void FixedUpdate()
    {
        _drillMovement1.layerTemperature += Random.Range(-0.1f, 0.1f);      // Add random fluctuations in temperature
        _drillMovement1.layerTemperature = Mathf.Round(_drillMovement1.layerTemperature * 10f) * 0.1f;  // Round to one decimal
        temperatureUItext.text = _drillMovement1.layerTemperature + " °C";      // Display temperature in UI
        
        // Depth UI
        float depthUIFloat = ((6371f / 100f) * _drillMovement1.progress);
        depthUItext.text = Mathf.Round((depthUIFloat * 10f) * 0.1f) + " meter";
    }

    private void OnTriggerEnter(Collider other)
    {
        
        //Change Elements UI and other related UIs relating to the current layer (All information UI)
        switch (other.tag)
        {
            case "Crust":
                EnableIconsAndTexts(false, elementIcons, elementTexts);
                EnableIconsAndTexts(true, crust_elementIcons, crust_elementText);
                StartCoroutine(ShowUIForDuration(UI_Gameobject: LayerTransitionTextUI, Duration: 3f, UItext: "Skorpen - ca. 0-35 km"));
                _drillMovement1.riseInTemperaturePerClick = 30;
                
                // Oxygen (O) - Light blue stone or quartz background
                // Silicon (Si) - Silver/gray mineral look
                // Aluminium (Al) - Shiny silvery metal
                break;
            case "Upper Mantle":
                EnableIconsAndTexts(false, elementIcons, elementTexts);
                EnableIconsAndTexts(true, upperMantle_elementIcons, upperMantle_elementText);
                StartCoroutine(ShowUIForDuration(UI_Gameobject: LayerTransitionTextUI, Duration: 3f, UItext: "Den Øvre Mantel - ca. 35-600 km"));
                _drillMovement1.layerTemperature = 700;
                _drillMovement1.riseInTemperaturePerClick = 100;
                
                // Magnesium (Mg) - green-tinged rock (like olivine)
                // Iron (Fe) - Rusty metal
                // Calcium - Chalky white stone
                break;
            case "Mantle":
                EnableIconsAndTexts(false, elementIcons, elementTexts);
                EnableIconsAndTexts(true, mantle_elementIcons, mantle_elementText);
                StartCoroutine(ShowUIForDuration(UI_Gameobject: LayerTransitionTextUI, Duration: 3f, UItext: "Mantlen - ca. 660-2900 km"));
                _drillMovement1.layerTemperature = 2800;
                _drillMovement1.riseInTemperaturePerClick = 80;
                
                // Silicon (Si) -  dark silicate rock
                // Magnesium (Mg) - Darker grey /
                //Oxygen (O) - Bright flame with an O, representing that oxygen mostly burns here
                break;
            case "Outer Core": 
                EnableIconsAndTexts(false, elementIcons, elementTexts);
                EnableIconsAndTexts(true, outerCore_elementIcons, outerCore_elementText);
                StartCoroutine(ShowUIForDuration(UI_Gameobject: LayerTransitionTextUI, Duration: 3f, UItext: "Den Ydre Kerne - ca. 2900-5150 km"));
                _drillMovement1.layerTemperature = 3700;
                _drillMovement1.riseInTemperaturePerClick = 80;
                
                // Iron (Fe) - Glowing red molten metal
                // Nickel (Ni) - Shiny metallic silver
                // Sulfur (S) - Yellow sulfur element with an S on it
                break;
            case "Inner Core":
                EnableIconsAndTexts(false, elementIcons, elementTexts);
                EnableIconsAndTexts(true, innerCore_elementIcons, innerCore_elementText);
                StartCoroutine(ShowUIForDuration(UI_Gameobject: LayerTransitionTextUI, Duration: 3f, UItext: "Den Indre Kerne - ca. 5150-6371 km"));
                _drillMovement1.layerTemperature = 6000;
                _drillMovement1.riseInTemperaturePerClick = 30;
                
                // Iron (Fe) - Molten or crystalized iron texture 
                // Nickel (Ni) - Solid, dense metallic cube
                // Gold (Au) - Golden square-like with Au in the middle
                break;
            case "InnerCoreTarget":
                Destroy(gameObject);    // <------------------------- END GAME HEEEEEEEEEEEEERE <----------------------------- //
                break;
        }
    }

    
    private void EnableIconsAndTexts(bool enableDisableBool, List<GameObject> icons, List<GameObject> texts)
    {
        if (enableDisableBool == false)
        {
            PeriodicElementsUI.SetActive(false);
            
            foreach (GameObject elementIcon in icons)
            {
                elementIcon.SetActive(false);
            }
            foreach (GameObject elementText in texts)
            {
                elementText.SetActive(false);
            }
        }
        else
        {
            PeriodicElementsUI.SetActive(true);
            
            foreach (GameObject elementIcon in icons)
            {
                elementIcon.SetActive(true);
            }
            foreach (GameObject elementText in texts)
            {
                elementText.SetActive(true);
            }
        }
    }
    
    IEnumerator ShowUIForDuration(GameObject UI_Gameobject, float Duration, string UItext)
    {
        if (UI_Gameobject.TryGetComponent<TMP_Text>(out TMP_Text textUI))
        {
            textUI.text = UItext;
        }
        
        UI_Gameobject.SetActive(true);
        yield return new WaitForSeconds(Duration);
        UI_Gameobject.SetActive(false);
    }
}
