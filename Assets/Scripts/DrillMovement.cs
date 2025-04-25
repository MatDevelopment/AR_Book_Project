using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrillMovement : MonoBehaviour
{
    [SerializeField] private GameObject DrillGameobjectParent;
    [SerializeField] private GameObject DrillToRotate;
    [SerializeField] private GameObject EarthInnerCore;

    [SerializeField] private GameObject LayerGameobjectCollider;

    private bool DrillSupposedToSpin = true;
    private bool DrillIsActive;
    
    private float drillMovementSpeed = 1;
    private float drillInnerCore_StartDistance;
    private float amountOfTimeToDrill;

    private float temperatureDrill_current;
    private float depthDrill_current;

    private Vector3 lastPosition;
    

    private void Start()
    {
        
        drillInnerCore_StartDistance =
            Vector3.Distance(EarthInnerCore.transform.position, DrillGameobjectParent.transform.position);
    }

    private void Update()
    {

        if (amountOfTimeToDrill > 0)    // If there is more time added to drill ...
        {
            DrillIsActive = true;       // T
        }
        else if (amountOfTimeToDrill <= 0)
        {
            DrillIsActive = false;
        }
        
        // If the drill is (supposed) to spinning right now, then...
        if (DrillSupposedToSpin)
        {
            //DrillToRotate.transform.Rotate(Vector3.down * 50 * Time.deltaTime, Space.World);
        }

        if (DrillIsActive)
        {
            DrillToRotate.transform.LookAt(EarthInnerCore.transform);   // the drill will look at the earth's inner core gameobject
            //var step = drillMovementSpeed * Time.deltaTime;
            //DrillGameobjectParent.transform.position = Vector3.MoveTowards(DrillGameobjectParent.transform.position, EarthInnerCore.transform.position, step);      //Moves the drill towards the core of the earth
            
            //Trying to find new way of directing towards core, because MoveTowards cannot be stopped mid action AND THAT IS FUCKING STUPID
            Vector3 DirectionVector = Vector3.Normalize(EarthInnerCore.transform.position);
            //Maybe use a direction vector to push with force. Then object can be stopped using isKinematic bool switching.

        }
        else if (DrillIsActive == false)
        {
            
        }
        
        //Measuring distance between the drill and the inner core, thereafter using it to change the depth UI text display
        float drillInnerCore_CurrentDistance =
            Vector3.Distance(EarthInnerCore.transform.position, DrillGameobjectParent.transform.position);
        
        float scaledValue = (drillInnerCore_CurrentDistance - 0) / (drillInnerCore_StartDistance - 0);
        Debug.Log(scaledValue);
        
        
        //----------TEMPERATURE CHECKING----------//
        if (Mathf.Clamp(temperatureDrill_current, -20, 10000) > 1000)       //If the current temperature is above 1000 degrees, then the drill will be stressed, making sounds and needing to be cooled down by shaking the phone
        {
            //Shaking phone mechanic run method here
        }
        
        //---------END (TEMPERATURE CHECKING--------//
    }

    private void FixedUpdate()
    {
        //-------Temperature accumulation when drill is active--------//
        if (DrillIsActive)
        {
            temperatureDrill_current += 0.1f;
        }
        else if (DrillIsActive == false)
        {
            temperatureDrill_current -= 0.25f;
        }
        
        //-------END (Temperature accumulation when drill is active)--------//
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Crust":
                //Change Elements UI and other related UIs relating to the current layer (All information UI)
                // Oxygen (O) - Light blue stone or quartz background
                // Silicon (Si) - Silver/gray mineral look
                // Aluminium (Al) - Shiny silvery metal
                break;
            case "Upper Mantle":
                // Magnesium (Mg) - green-tinged rock (like olivine)
                // Iron (Fe) - Rusty metal
                // Calcium - Chalky white stone
                break;
            case "Mantle":
                // Silicon (Si) -  dark silicate rock
                // Magnesium (Mg) - Darker grey /
                //Oxygen (O) - Bright flame with an O, representing that oxygen mostly burns here
                break;
            case "Outer Core": 
                // Iron (Fe) - Glowing red molten metal
                // Nickel (Ni) - Shiny metallic silver
                // Sulfur (S) - Yellow sulfur element with an S on it
                break;
            case "Inner Core":
                // Iron (Fe) - Molten or crystalized iron texture 
                // Nickel (Ni) - Solid, dense metallic cube
                // Gold (Au) - Golden square-like with Au in the middle
                break;
        }
    }

    public void AddTimeToDrill()       //Run this method each time the user does the elligible action that leads to the drill going deeper.
    {
        if (DrillIsActive & amountOfTimeToDrill <= 4)      //If the drill is running, each elligible action will lead to the drill running an additional 0.25 seconds.
        {
            amountOfTimeToDrill += 0.02f;
        }
        else if (DrillIsActive == false)    //If the drill is starting up from standing still, it will run for half a second at least.
        {
            amountOfTimeToDrill += 0.2f;
        }
        
    }
}
