using UnityEngine;
using TMPro;
//using NUnit.Framework; // Removed as it's likely not needed for runtime code
using System.Collections.Generic;
//using UnityEngine.EventSystems; // Removed as it's not used
//using System.Drawing; // Removed as it's not standard Unity and likely unused
using System.Collections;

public class ControlTimeWithDistance : MonoBehaviour
{
    // --- Inspector Assigned ---
    public QuestionManager questionManager;
    public CanvasGroup TutorialCanvasGroup, WalkingInstructionsCanvasGroup;
    public TextMeshPro textMeshPro; // Assign your TextMeshPro object here
    public LineRenderer lineRenderer; // Assign your LineRenderer object here
    public Transform trailEndAtSolarSystem; // Assign the target endpoint for the line
    public CanvasGroup PressMeTextCG;
    public GameObject solarSystemObject; // Assign the Solar System GameObject (start point for the line)
    public GameObject solarSystemParticles;
    public GameObject glowBlue, glowGreen;

    [SerializeField] private Transform solarSystemSpawnPointTransform;

    // --- Configuration ---
    [SerializeField] private float distanceForSolarSystemToSpawn = 8f; // Kept, though object seems pre-assigned now
    [SerializeField] private float textRotationSpeed = 2.0f; // Speed at which the text rotates towards the camera
    [SerializeField] private float visibilityThresholdTime = 7.8f; // The timeByDistance value needed to show text/line

    // --- Private Members ---
    private UI_BigBang uiBigBang;
    private AddSliderMarks addSliderMarks;
    private const int particleSystemMaxPlaybackTime = 10;

    private double distanceStart = 0.9; // Start of visualization
    private double distanceEnd = 4.0;   // End of visualization

    // Time mapping (kept for reference, though timeByDistance is directly used)
    // private double timeStart = 0.0;
    // private double timeEnd = 13700000000; // 13.7 billion years

    // Solar System Scaling parameters
    private Vector3 solarSystemMinimumScale = Vector3.zero;
    private Vector3 solarSystemMaximumScale = new Vector3(1f, 1f, 1f);
    private Vector3 solarSystemParticlesMinimumScale = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 solarSystemParticlesMaximumScale = new Vector3(1.04f, 1.04f, 1.04f);

    // State flags
    public bool hasReachedHydrogenFormation = false; // Kept, seems unused currently
    public bool shouldBeVisible = false; // Kept, seems unused currently
    public bool hasShownTutorialText = false;
    public bool targetFound = false;
    private bool canUpdateMarkers = false;

    // Particle System references
    [SerializeField] private List<ParticleSystem> childSystems = new List<ParticleSystem>(); // Initialize list

    // UI and Camera references
    public TextMeshProUGUI distanceText;
    private Camera cam;

    // Cached values for performance/readability
    [SerializeField] float distanceToCamera;
    [SerializeField] float timeByDistance;

    private float waitTimequixkFixForTutorialText = 5;
    private float timeThatTargetWasFound = 0;

    void Start()
    {
        glowBlue.SetActive(false);
        glowGreen.SetActive(false);

        // Initial UI setup
        if (WalkingInstructionsCanvasGroup != null) WalkingInstructionsCanvasGroup.alpha = 0;
        if (TutorialCanvasGroup != null) TutorialCanvasGroup.alpha = 0;

        // Ensure components assigned in Inspector are actually present
        ValidateInspectorAssignments();

        // Initial state for LineRenderer and TextMeshPro
        if (textMeshPro != null) textMeshPro.enabled = false;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            // Ensure LineRenderer has 2 points allocated if it exists
            lineRenderer.positionCount = 2;
        }

        // Get main camera reference
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found! Text rotation requires a camera.", this);
        }

        // For Editor Testing
#if UNITY_EDITOR
        // If you want to test without AR target finding in the editor,
        // uncomment the next line or call CallThisOnTargetFound() from another script.
         CallThisOnTargetFound();
#endif
    }

    // Function called when the AR target is found (or manually in Editor)
    public void CallThisOnTargetFound()
    {
        if (targetFound) return; // Prevent multiple initializations

        // Get references to other components
        uiBigBang = GetComponent<UI_BigBang>();
        if (uiBigBang != null)
        {
            uiBigBang.HideStartText();
            uiBigBang.ShowBigBangInformation();
        }
        else
        {
            Debug.LogWarning("UI_BigBang component not found on this GameObject.", this);
        }

        addSliderMarks = FindAnyObjectByType<AddSliderMarks>(); // Consider assigning via Inspector if possible

        // Collect Particle Systems (if not already assigned in Inspector)
        // It's often better to assign these in the Inspector for clarity.
        if (childSystems.Count == 0)
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            if (ps != null) childSystems.Add(ps);
            // Add children PS only if needed and not manually assigned
            // foreach (Transform child in transform)
            // {
            //     ParticleSystem childPs = child.GetComponent<ParticleSystem>();
            //     if (childPs != null) childSystems.Add(childPs);
            // }
        }


        // Stop particle systems initially
        foreach (ParticleSystem child in childSystems)
        {
            if (child != null) child.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Delay marker updates slightly
        Invoke(nameof(SetCanUpdateMarkers), 0.2f);

        targetFound = true; // Set flag
        timeThatTargetWasFound = Time.time; 


        // Show initial instructions
        ShowWalkInstructions();
    }

    private void SetCanUpdateMarkers()
    {
        canUpdateMarkers = true;
    }

    void Update()
    {
        // Core loop guards
        if (!targetFound || cam == null) return; // Don't update if target not found or no camera
        if (questionManager != null && questionManager.hasStartedAfterQuizSession) return;

        // --- Distance Calculation ---
        Vector3 offset = transform.position - cam.transform.position;
        // Use XZ plane distance for horizontal movement control
        Vector3 projectedOffset = new Vector3(offset.x, 0, offset.z);
        distanceToCamera = projectedOffset.magnitude;

        // --- Time Mapping based on Distance ---
        if (distanceToCamera < distanceStart)
        {
            timeByDistance = 0;
        }
        else if (distanceToCamera > distanceEnd)
        {
            timeByDistance = particleSystemMaxPlaybackTime;
        }
        else
        {
            // Map the distance to particle system time
            timeByDistance = (float)ExtensionMethods.Remap(
                distanceToCamera,
                distanceStart,
                distanceEnd,
                0,
                particleSystemMaxPlaybackTime
            );
        }

        // --- Particle System Simulation ---
        // Stop/Clear is important before Simulate if you want precise time control
        foreach (ParticleSystem child in childSystems)
        {
            if (child == null) continue;
            child.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Clear previous state
            child.Simulate(timeByDistance, withChildren: true, restart: true); // Simulate to the exact time
            child.Play(); // Needed to make Simulate visually update (or keep playing if time increases)
        }


        // --- Tutorial Trigger ---
        if (timeByDistance > 5 && Time.time > timeThatTargetWasFound + waitTimequixkFixForTutorialText && !hasShownTutorialText)
        {
            hasShownTutorialText = true;
            Invoke(nameof(ShowTutorialInformation), 1.8f);
           
        }

        // --- UI Updates ---
        if (distanceText != null) distanceText.text = "dist: " + distanceToCamera.ToString("F2") + "m";
        if (uiBigBang != null)
        {
            uiBigBang.UpdateTimeText(timeByDistance);
            uiBigBang.UpdateTimeSlider(timeByDistance);
            uiBigBang.SetInformationText(timeByDistance); // Cast to float if needed by the method
        }
        if (canUpdateMarkers && addSliderMarks != null)
        {
            addSliderMarks.UpdateSliderMarks(timeByDistance);
        }

        // --- Solar System Scaling ---
        // (Kept your original scaling logic)
        UpdateSolarSystemScale(timeByDistance);


        // --- TextMeshPro and LineRenderer Logic ---
        
        shouldBeVisible = timeByDistance > visibilityThresholdTime;

        // TextMeshPro Handling
        if (PressMeTextCG != null)
        {
            if(shouldBeVisible & hasShownTutorialText)
            PressMeTextCG.gameObject.SetActive(true); // Enable/disable based on time
            else
                PressMeTextCG.gameObject.SetActive(false); // Disable if not visible

            if (PressMeTextCG)
            {
                //// Slowly rotate towards the camera
                //Vector3 directionToCamera = cam.transform.position - textMeshPro.transform.position;
                //directionToCamera.y = 0; // Optional: Remove this line if you want it to tilt up/down too

                //textMeshPro.transform.position = cam.transform.position + cam.transform.forward * 1.5f; // Adjust position to be in front of the camera
                PressMeTextCG.transform.LookAt(cam.transform); // Face the camera

            }
        }

        // TextMeshPro Handling
        if (textMeshPro != null)
        {
            textMeshPro.enabled = shouldBeVisible; // Enable/disable based on time

            if (shouldBeVisible)
            {
                //// Slowly rotate towards the camera
                //Vector3 directionToCamera = cam.transform.position - textMeshPro.transform.position;
                //directionToCamera.y = 0; // Optional: Remove this line if you want it to tilt up/down too

                textMeshPro.transform.position = cam.transform.position + cam.transform.forward * 1.5f; // Adjust position to be in front of the camera
                textMeshPro.transform.LookAt(cam.transform); // Face the camera

            }
        }

        // LineRenderer Handling
        if (lineRenderer != null)
        {
            lineRenderer.enabled = shouldBeVisible; // Enable/disable based on time

            if (shouldBeVisible)
            {
                // Update line positions if the required objects are assigned
                if (solarSystemObject != null && trailEndAtSolarSystem != null)
                {
                    lineRenderer.SetPosition(0, solarSystemObject.transform.position);
                    lineRenderer.SetPosition(1, trailEndAtSolarSystem.position);
                }
                else if (lineRenderer.enabled) // Only log error if it should be visible but can't draw
                {
                    Debug.LogWarning("LineRenderer is visible, but SolarSystemObject or TrailEndAtSolarSystem is not assigned.", this);
                    lineRenderer.enabled = false; // Disable it to prevent errors drawing from (0,0,0)
                }
            }
        }
    }

    // --- Solar System Scaling Logic (extracted for clarity) ---
    private void UpdateSolarSystemScale(float currentTime)
    {
        if (solarSystemObject == null) return; // Need the object to scale it

        // Position Update (seems constant, kept from original)
        if (solarSystemSpawnPointTransform != null)
        {
            solarSystemObject.transform.position = solarSystemSpawnPointTransform.position;
        }

        // Scaling Logic based on time
        Vector3 targetScale = solarSystemMinimumScale;
        Vector3 targetParticleScale = solarSystemParticlesMinimumScale;
        float lerpFactor = 0f;

        if (currentTime <= 7.8f)
        {
            targetScale = solarSystemMinimumScale;
            targetParticleScale = solarSystemParticlesMinimumScale;
        }
        else if (currentTime > 7.8f && currentTime < 8.3f)
        {
            lerpFactor = Mathf.InverseLerp(7.8f, 8.3f, currentTime); // Calculate interpolation factor (0 to 1)
            targetScale = Vector3.Lerp(solarSystemMinimumScale, solarSystemMaximumScale, lerpFactor);
            targetParticleScale = Vector3.Lerp(solarSystemParticlesMinimumScale, solarSystemParticlesMaximumScale, lerpFactor);

        }
        else if (currentTime >= 8.3f && currentTime <= 10.1f) // Adjusted condition slightly based on original logic
        {
            targetScale = solarSystemMaximumScale;
            targetParticleScale = solarSystemParticlesMaximumScale;
        }
        // Original code had a commented-out shrink section, re-added the else for clarity
        else // currentTime > 10.1f
        {
            targetScale = solarSystemMinimumScale; // Or maybe maximum? Adjust as needed for times > 10.1
            targetParticleScale = solarSystemParticlesMinimumScale; // Adjust as needed
        }

        if(currentTime > 7.8f)
        {
            if(questionManager.AllQuestionsAnswered)
            {
                glowBlue.SetActive(false);
                glowGreen.SetActive(true);
            }
            else
            {
                glowBlue.SetActive(true);
                glowGreen.SetActive(false);
            }
        }
            

        // Apply the scale
        solarSystemObject.transform.localScale = targetScale;
        SetOutlineParticleSize(targetParticleScale);
    }


    // --- UI Fade Helpers ---
    public void ShowWalkInstructions()
    {
        if (WalkingInstructionsCanvasGroup != null)
        {
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(WalkingInstructionsCanvasGroup, true, 1.6f));
            Invoke(nameof(HideWalkInstructions), 6.5f);
        }
    }

    private void HideWalkInstructions()
    {
        if (WalkingInstructionsCanvasGroup != null)
        {
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(WalkingInstructionsCanvasGroup, false, 1f));
        }
    }

    private void ShowTutorialInformation()
    {
    
        if (questionManager != null)
        {
            // Assuming PingOpenQuestionPanelButton is a coroutine or method on QuestionManager
            questionManager.StartCoroutine(questionManager.PingOpenQuestionPanelButton());
        }
        if (TutorialCanvasGroup != null )
        {
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(TutorialCanvasGroup, true, 1.4f));
        }
    }

    public void HideTutorialInformation()
    {
        if (TutorialCanvasGroup != null)
        {
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(TutorialCanvasGroup, false, 1.5f));
        }
    }

    // --- Particle Scaling Helpers ---
    private void SetOutlineParticleSize(Vector3 sizeVector)
    {
        if (solarSystemParticles == null) return;
        solarSystemParticles.transform.localScale = sizeVector;
        // Apply to children only if necessary for your setup
        foreach (Transform childTransform in solarSystemParticles.transform)
        {
            childTransform.localScale = sizeVector; // Might need adjustments if children have inherent scales
        }
    }

    // LerpOutlineParticleSizes seems redundant if SetOutlineParticleSize is used within the Lerp calculation in UpdateSolarSystemScale
    // private void LerpOutlineParticleSizes(Vector3 minimum, Vector3 maximum, float t) { ... } // Removed

    // --- Validation ---
    private void ValidateInspectorAssignments()
    {
        if (textMeshPro == null) Debug.LogWarning("TextMeshPro reference not set in the inspector.", this);
        if (lineRenderer == null) Debug.LogWarning("LineRenderer reference not set in the inspector.", this);
        if (trailEndAtSolarSystem == null) Debug.LogWarning("TrailEndAtSolarSystem reference not set in the inspector.", this);
        if (solarSystemObject == null) Debug.LogWarning("SolarSystemObject reference not set in the inspector.", this);
        // Add checks for other crucial references if needed
        if (questionManager == null) Debug.LogWarning("QuestionManager reference not set.", this);
        if (TutorialCanvasGroup == null) Debug.LogWarning("TutorialCanvasGroup reference not set.", this);
        if (WalkingInstructionsCanvasGroup == null) Debug.LogWarning("WalkingInstructionsCanvasGroup reference not set.", this);
        if (solarSystemParticles == null) Debug.LogWarning("SolarSystemParticles reference not set.", this);
        if (solarSystemSpawnPointTransform == null) Debug.LogWarning("SolarSystemSpawnPointTransform reference not set.", this);
        if (distanceText == null) Debug.LogWarning("DistanceText (TextMeshProUGUI) reference not set.", this);
    }
}
