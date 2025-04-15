using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoverControl : MonoBehaviour
{
    [Header("Spawning Rover")]
    public GameObject mainCamera;
    public float spawnDistance = 1f; // Distance in front of the camera
    public GameObject roverPrefab;
    // private bool readyToSpawn = false;

    [Header("UI Elements")]
    public GameObject spawnButton;
    public GameObject despawnButton;
    public GameObject interactButton;
    public GameObject movementButtons;
    public GameObject eventPanel;
    public GameObject scanEnvPanel;
    private RoverRockCrushing roverRockCrushing;

    [Header("Movement")]
    public Rigidbody roverRB;
    public float moveSpeed = 2.8f;
    public float maxMoveSpeed = 3f;
    public float rotateSpeed = 1.4f;
    public float maxRotateSpeed = 2f;
    private GameObject rover;

    private bool movingForward = false;
    private bool movingBackward = false;
    private bool rotatingLeft = false;
    private bool rotatingRight = false;

    // Debugging
    [Header("Debugging")]
    [SerializeField] bool isDebugging = false;
    [SerializeField] Slider moveSpeedSlider;
    [SerializeField] Slider rotateSpeedSlider;
    [SerializeField] PhysicsMaterial roverMaterial;
    [SerializeField] Slider staticFrictionSlider;
    [SerializeField] Slider dynamicFrictionSlider;
    private TextMeshProUGUI moveSpeedText;
    private TextMeshProUGUI rotateSpeedText;
    private TextMeshProUGUI staticFrictionText;
    private TextMeshProUGUI dynamicFrictionText;

    // Based on: https://www.youtube.com/watch?v=wYSyfeoiK8k
    // Detecting Nearest Event
    private GameObject[] allObjectsWithTag;
    private GameObject nearestObject;

    // Audio
    private SoundEffectManager roverSoundEffectManager;

    private void Start()
    {
        spawnButton.SetActive(false);
        despawnButton.SetActive(false);
        interactButton.SetActive(false);
        eventPanel.SetActive(false);
        movementButtons.SetActive(false);
        scanEnvPanel.SetActive(true);

        // Accessing rockcrushing script
        roverRockCrushing = eventPanel.GetComponent<RoverRockCrushing>();

        // Set gravity to mars
        Physics.gravity = new Vector3(0, -3.73f, 0);

        if (isDebugging)
        {
            // Gets references to the text objects for the sliders
            moveSpeedText = moveSpeedSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            rotateSpeedText = rotateSpeedSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            staticFrictionText = staticFrictionSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            dynamicFrictionText = dynamicFrictionSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            // Sets sliders active
            moveSpeedSlider.gameObject.SetActive(true);
            rotateSpeedSlider.gameObject.SetActive(true);
            dynamicFrictionSlider.gameObject.SetActive(true);
            staticFrictionSlider.gameObject.SetActive(true);

            // Sets the slider values to the predefined values
            moveSpeedSlider.value = moveSpeed;
            rotateSpeedSlider.value = rotateSpeed;
            dynamicFrictionSlider.value = roverMaterial.dynamicFriction;
            staticFrictionSlider.value = roverMaterial.staticFriction;

            //Adds a listener to the slider and invokes a method when the value changes.
            moveSpeedSlider.onValueChanged.AddListener(delegate { SliderValueChangeCheck(); });
            rotateSpeedSlider.onValueChanged.AddListener(delegate { SliderValueChangeCheck(); });
            dynamicFrictionSlider.onValueChanged.AddListener(delegate { SliderValueChangeCheck(); });
            staticFrictionSlider.onValueChanged.AddListener(delegate { SliderValueChangeCheck(); });

        } else if (moveSpeedSlider != null) {
            
            // Sets sliders inactive
            moveSpeedSlider.gameObject.SetActive(false);
            rotateSpeedSlider.gameObject.SetActive(false);
            dynamicFrictionSlider.gameObject.SetActive(false);
            staticFrictionSlider.gameObject.SetActive(false);
        }
    }

    private void SliderValueChangeCheck()
    {
        // Updates the internal values based on slider value
        moveSpeed = moveSpeedSlider.value;
        rotateSpeed = rotateSpeedSlider.value;
        roverMaterial.dynamicFriction = dynamicFrictionSlider.value;
        roverMaterial.staticFriction = staticFrictionSlider.value;

        // Update Text
        moveSpeedText.text = $"Move Speed: {moveSpeed.ToString()}";
        rotateSpeedText.text = $"Rotate Speed: {rotateSpeed.ToString()}";
        dynamicFrictionText.text = $"Dynamic Friction: {roverMaterial.dynamicFriction.ToString()}";
        staticFrictionText.text = $"Static Friction: {roverMaterial.staticFriction.ToString()}";
    }

    public void ReadyToSpawnRover()
    {
        spawnButton.SetActive(true);
        scanEnvPanel.SetActive(false);
    }

    public void SpawnRover()
    {   
        // Determine spawn position in front of the camera
        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * spawnDistance;

        // Spawn the rover
        rover = Instantiate(roverPrefab, spawnPosition, Quaternion.identity);
        roverRB = rover.GetComponent<Rigidbody>();
        roverSoundEffectManager = rover.GetComponent<SoundEffectManager>();

        roverSoundEffectManager.PlaySingleSound("button_click");

        // Hide spawn rover button
        spawnButton.SetActive(false);
        despawnButton.SetActive(true);

        // Show Movement Buttons
        movementButtons.SetActive(true);
    }

    public void DespawnRover()
    {
        // Remove the rover
        Destroy(rover);

        // Show spawn rover again
        spawnButton.SetActive(true);
        despawnButton.SetActive(false);

        // Hide Movement Buttons
        movementButtons.SetActive(false);
    }

    void FixedUpdate()
    {
        if (roverRB != null)
        {
            if (movingForward)
            {
                roverRB.AddForce(rover.transform.forward * moveSpeed, ForceMode.Force);
            }
            if (movingBackward)
            {
                roverRB.AddForce(-rover.transform.forward * moveSpeed, ForceMode.Force);
            }
            if (rotatingLeft)
            {
                roverRB.AddTorque(Vector3.up * -rotateSpeed);
            }
            if (rotatingRight)
            {
                roverRB.AddTorque(Vector3.up * rotateSpeed);
            }

            // **Limit Maximum Speed**
            roverRB.linearVelocity = Vector3.ClampMagnitude(roverRB.linearVelocity, maxMoveSpeed);

            // **Limit Maximum Rotation Speed**
            roverRB.angularVelocity = Vector3.ClampMagnitude(roverRB.angularVelocity, maxRotateSpeed);
        }
    }

    // Methods for Event Trigger assignment

    public void StartMovingForward()
    { 
        movingForward = true;
        roverSoundEffectManager.PlaySound("gravel_road");
    }
    public void StopMovingForward()
    { 
        movingForward = false;
        roverSoundEffectManager.PauseSound();
    }
    public void StartMovingBackward()
    { 
        movingBackward = true;
        roverSoundEffectManager.PlaySound("gravel_road");
    }
    public void StopMovingBackward()
    {
        movingBackward = false;
        roverSoundEffectManager.PauseSound();
    }

    public void StartRotatingLeft()
    {
        rotatingLeft = true;
        roverSoundEffectManager.PlaySound("servo");
    }
    public void StopRotatingLeft()
    {
        rotatingLeft = false;
        roverSoundEffectManager.PauseSound();
    }

    public void StartRotatingRight()
    {
        rotatingRight = true;
        roverSoundEffectManager.PlaySound("servo");
    }
    public void StopRotatingRight()
    {
        rotatingRight = false;
        roverSoundEffectManager.PauseSound();
    }

    // Function called to update the list of events whenever new instances is instantiated
    public void UpdateListOfRoverEvents()
    {
        allObjectsWithTag = GameObject.FindGameObjectsWithTag("MarsEvent");
    }

    private void Update()
    {
        if (rover != null && allObjectsWithTag.Length > 0)
        {
            // Looping through each rover event tagged object and finding the distance to the nearest one
            nearestObject = allObjectsWithTag[0];
            float distanceToNearest = Vector3.Distance(rover.transform.position, nearestObject.transform.position);

            for (int i = 0; i < allObjectsWithTag.Length; i++)
            {
                float distanceToCurrent = Vector3.Distance(rover.transform.position, allObjectsWithTag[i].transform.position);

                // Checks if current distance is less than current nearest
                if (distanceToCurrent < distanceToNearest)
                {
                    nearestObject = allObjectsWithTag[i];
                    distanceToNearest = distanceToCurrent;
                }
            }

            foreach (GameObject g in allObjectsWithTag)
            {
                g.transform.localScale = 10f * Vector3.one;
            }

            if (distanceToNearest < 0.5f)
            {
                if (!interactButton.activeInHierarchy)
                {
                    interactButton.SetActive(true);
                }
                nearestObject.transform.localScale = 15f * Vector3.one;
            } 
            else if (interactButton.activeInHierarchy)
            {
                interactButton.SetActive(false);
            }
        } 
        else if (interactButton.activeInHierarchy)
        {
            interactButton.SetActive(false);
        }
    }

    // Function for the button call when interacting with an event
    public void InteractWithEvent()
    {
        // Sound Effect
        if (rover != null)
        {
            roverSoundEffectManager.PlaySingleSound("button_click");
        }
        
        // Remove and Update list of objects
        nearestObject.SetActive(false);
        UpdateListOfRoverEvents();

        // Show Event UI
        eventPanel.SetActive(true);
        roverRockCrushing.ResetRockCrushing();

        // Hide UI Buttons
        movementButtons.SetActive(false);
        despawnButton.SetActive(false);
    }

    public void FinishMarsEvent()
    {
        // Sound Effect
        if (rover != null)
        {
            roverSoundEffectManager.PlaySingleSound("button_click");
        }

        // Hide Event UI
        eventPanel.SetActive(false);

        // Show UI Buttons
        movementButtons.SetActive(true);
        despawnButton.SetActive(true);
    }
}
