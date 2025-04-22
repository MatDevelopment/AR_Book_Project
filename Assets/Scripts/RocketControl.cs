using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class RocketControl : MonoBehaviour
{
    [Header("Rocket Spawning")]
    // Spawning the Rocket
    public GameObject mainCamera;
    public float spawnDistance = 1f; // Distance in front of the camera
    public GameObject rocketPrefab;

    [Header("UI Elements")]
    // UI Elements
    public GameObject spawnButton;
    public GameObject verticalSpawn;
    public GameObject despawnButton;
    public GameObject verticalDespawn;
    public Slider throttleSlider;
    public Slider verticalSlider;
    public GameObject dragButton;
    [SerializeField] private GameObject rocketRender;

    // Rocket Movement
    private GameObject rocket;
    [Header("Rocket Movement")]
    public Rigidbody rocketRB;
    public float moveSpeed = 0.2f;
    public float rotateSpeed = 1f;
    [SerializeField] private float maxMoveSpeed = 0.5f;
    [SerializeField] private float maxRotateSpeed = 1f;
    private float rocketSpeed = 0f;

    [Header("Touch Rotation Control")]
    // Values for rotating by touch
    [SerializeField] private InputActionAsset _action;
    [SerializeField] private bool isInverted = false;
    private bool rotateAllowed = false;
    private bool isDragging = false;

    [Header("Tangible Rotation")]
    public bool tangibleRotation = false;
    public bool fullyTangible = false;
    [SerializeField] private GameObject modelTarget;

    [Header("Rocket Particles")]
    [SerializeField] private SUI_particleSystemsHolder particleHolder;
    // Setting up and getting the input actions from the action asset
    public InputActionAsset action
    {
        get => _action; set => _action = value;
    }

    

    protected InputAction LeftClickPressedInputAction { get; set; }
    protected InputAction MouseLookInputAction { get; set; }

    private void Awake()
    {
        InitializeInputSystem();
    }

    private void InitializeInputSystem()
    {
        // Finds and defines left click action from action asset
        LeftClickPressedInputAction = action.FindAction("Left Click");
        if (LeftClickPressedInputAction != null)
        {
            LeftClickPressedInputAction.started += OnLeftClickPressed;
            LeftClickPressedInputAction.performed += OnLeftClickPressed;
            LeftClickPressedInputAction.canceled += OnLeftClickPressed;
        }

        // Finds the mouse look action for dragging
        MouseLookInputAction = action.FindAction("Mouse Look");

        action.Enable();
    }

    // Checks for left clicking or primary touch to define if rotation is allowed
    protected virtual void OnLeftClickPressed(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            rotateAllowed = true;
        }
        else if (context.canceled)
        {
            rotateAllowed = false;
        }
    }

    // Function that reads the delta value from mouse or touch
    protected virtual Vector2 GetMouseLookInput()
    {
        if (MouseLookInputAction != null)
        {
            return MouseLookInputAction.ReadValue<Vector2>();
        }

        return Vector2.zero;
    }

    private void Start()
    {
        // Shows and hides the right buttons
        spawnButton.SetActive(false);
        verticalSpawn.SetActive(false);
        despawnButton.SetActive(false);
        verticalDespawn.SetActive(false);
        throttleSlider.gameObject.SetActive(false);
        verticalSlider.gameObject.SetActive(false);
        dragButton.SetActive(false);

        // If tangible change UI elements to vertical ones
        if (tangibleRotation)
        {
            throttleSlider = verticalSlider;
            spawnButton = verticalSpawn;
            despawnButton = verticalDespawn;
        }

        // Activates the appropriate spawn button
        if (!fullyTangible)
        {
            spawnButton.SetActive(true);
        }

        //Adds a listener to the slider and invokes a method when the value changes.
        throttleSlider.onValueChanged.AddListener(delegate { SliderValueChangeCheck(); });
    }

    private void SliderValueChangeCheck()
    {
        // Updates the rocketspeed based on slider value
        rocketSpeed = throttleSlider.value * moveSpeed;
        SetParticlesToMatchSpeed(throttleSlider.value);
    }

    public void SetParticlesToMatchSpeed(float input)
    {
        particleHolder.SetBigFlameSpeed(input);
        particleHolder.SetSmallFlameSpeed(input);
    }

    public void SpawnRocket()
    {
        // Determine spawn position in front of the camera
        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * spawnDistance;

        // Spawn the rocket
        rocket = Instantiate(rocketPrefab, spawnPosition, Quaternion.identity);
        rocketRB = rocket.GetComponent<Rigidbody>();
        particleHolder = rocket.GetComponentInChildren<SUI_particleSystemsHolder>();

        // Hide spawn rocket button
        spawnButton.SetActive(false);
        despawnButton.SetActive(true);
        throttleSlider.gameObject.SetActive(true);
        throttleSlider.value = 0f;
        particleHolder.SetFlamesToZero();

        if (!tangibleRotation)
        {
            dragButton.SetActive(true);
        }
    }

    public void DespawnRocket()
    {
        // Remove the rocket
        Destroy(rocket);

        // Show spawn rocket again
        spawnButton.SetActive(true);
        despawnButton.SetActive(false);
        throttleSlider.gameObject.SetActive(false);
        dragButton.SetActive(false);
    }

    void FixedUpdate()
    {
        if (rocketRB != null)
        {
            // Moving the rocket forward with the throttle
            if (rocketSpeed != 0f)
            {
                rocketRB.AddForce(rocket.transform.forward * rocketSpeed, ForceMode.Force);
            }

            if (tangibleRotation)
            {
                rocket.transform.rotation = modelTarget.transform.rotation * Quaternion.Euler(-90, 0, 0);
            }

            // Rotating the rocket using touch
            if (!tangibleRotation && rotateAllowed && isDragging)
            {
                Vector2 MouseDeltaVector = GetMouseLookInput();

                float rotationX = MouseDeltaVector.x * rotateSpeed * Time.fixedDeltaTime;
                float rotationY = MouseDeltaVector.y * rotateSpeed * Time.fixedDeltaTime;

                // Physics Based Rotation (Spins quickly out of control
                //rocketRB.AddTorque(mainCamera.transform.up * -rotationX);
                //rocketRB.AddTorque(mainCamera.transform.right * rotationY);

                // Transform Based Rotation (Much more stable)
                rocket.transform.Rotate(Vector3.up * (isInverted ? 1 : -1), MouseDeltaVector.y, Space.World);
                rocket.transform.Rotate(Vector3.right * (isInverted ? 1 : -1), MouseDeltaVector.x, Space.World);

                rocketRender.transform.rotation = rocket.transform.rotation;
            }

            // **Limit Maximum Speed**
            rocketRB.linearVelocity = Vector3.ClampMagnitude(rocketRB.linearVelocity, maxMoveSpeed);

            // **Limit Maximum Rotation Speed**
            rocketRB.angularVelocity = Vector3.ClampMagnitude(rocketRB.angularVelocity, maxRotateSpeed);
        }
    }

    public void StopRocket()
    {
        rocketRB.linearVelocity = Vector3.zero;
        throttleSlider.value = 0;
        rocketSpeed = 0;
    }

    public void StartDragging() => isDragging = true;
    public void StopDragging() => isDragging = false;
}
