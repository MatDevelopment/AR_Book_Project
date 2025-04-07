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

    [Header("UI Elements")]
    public GameObject spawnButton;
    public GameObject despawnButton;

    [Header("Movement")]
    private GameObject rover;
    public Rigidbody roverRB;
    public float moveSpeed = 2.8f;
    public float rotateSpeed = 1.4f;

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

    private void Start()
    {
        spawnButton.SetActive(true);
        despawnButton.SetActive(false);

        // Set gravity to mars
        Physics.gravity = new Vector3(0, -3.73f, 0);

        if (isDebugging)
        {
            moveSpeedText = moveSpeedSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            rotateSpeedText = rotateSpeedSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            staticFrictionText = staticFrictionSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            dynamicFrictionText = dynamicFrictionSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            // Sets sliders active
            moveSpeedSlider.gameObject.SetActive(true);
            rotateSpeedSlider.gameObject.SetActive(true);
            dynamicFrictionSlider.gameObject.SetActive(true);
            staticFrictionSlider.gameObject.SetActive(true);

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
        moveSpeedText.text = moveSpeed.ToString();
        rotateSpeedText.text = rotateSpeed.ToString();
        dynamicFrictionText.text = roverMaterial.dynamicFriction.ToString();
        staticFrictionText.text = roverMaterial.staticFriction.ToString();
    }

    public void SpawnRover()
    {
        // Determine spawn position in front of the camera
        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * spawnDistance;

        // Spawn the rover
        rover = Instantiate(roverPrefab, spawnPosition, Quaternion.identity);
        roverRB = rover.GetComponent<Rigidbody>();

        // Hide spawn rover button
        spawnButton.SetActive(false);
        despawnButton.SetActive(true);
    }

    public void DespawnRover()
    {
        // Remove the rover
        Destroy(rover);

        // Show spawn rover again
        spawnButton.SetActive(true);
        despawnButton.SetActive(false);
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
            float maxSpeed = 3f;  // Adjust as needed
            roverRB.linearVelocity = Vector3.ClampMagnitude(roverRB.linearVelocity, maxSpeed);

            // **Limit Maximum Rotation Speed**
            float maxAngularSpeed = 2f;  // Adjust as needed
            roverRB.angularVelocity = Vector3.ClampMagnitude(roverRB.angularVelocity, maxAngularSpeed);
        }
    }

    // Methods for Event Trigger assignment

    public void StartMovingForward() => movingForward = true;
    public void StopMovingForward() => movingForward = false;
    public void StartMovingBackward() => movingBackward = true;
    public void StopMovingBackward() => movingBackward = false;

    public void StartRotatingLeft() => rotatingLeft = true;
    public void StopRotatingLeft() => rotatingLeft = false;

    public void StartRotatingRight() => rotatingRight = true;
    public void StopRotatingRight() => rotatingRight = false;
}
