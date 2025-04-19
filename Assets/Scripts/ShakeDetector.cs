using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent
using System.Collections.Generic; // Required for List

/// <summary>
/// Detects rapid vertical shaking of the GameObject this script is attached to.
/// </summary>
public class ShakeDetector : MonoBehaviour
{
    [Header("Shake Detection Settings")]
    [Tooltip("Minimum acceleration threshold (in units/sec^2) to register a shake movement.")]
    public float shakeThreshold = 25f;

    [Tooltip("The time window (in seconds) within which shakes must occur.")]
    public float shakeDetectionTime = 0.5f;

    [Tooltip("Minimum number of acceleration peaks within the time window to trigger a shake event.")]
    public int minShakeCount = 3;

    [Tooltip("Cooldown period (in seconds) after a shake is detected to prevent immediate re-triggering.")]
    public float shakeCooldown = 1.0f;

    [Header("Events")]
    [Tooltip("Event triggered when a shake is detected.")]
    public UnityEvent OnShake; // Event that other scripts or components can subscribe to via the Inspector

    // Private variables for tracking movement
    private float lastPositionY;
    private float lastVelocityY;
    private float lastAccelerationY; // Keep track of the last acceleration direction

    // List to store timestamps of detected shake peaks
    private List<float> shakeTimestamps = new List<float>();

    // Cooldown timer
    private float shakeCooldownTimer = 0f;

    /// <summary>
    /// Called once per frame before the first frame update.
    /// Initializes position and velocity tracking.
    /// </summary>
    void Start()
    {
        // Initialize position and velocity
        lastPositionY = transform.position.y;
        lastVelocityY = 0f;
        lastAccelerationY = 0f;

        // If no functions are assigned to OnShake in the inspector, add a default debug log.
        if (OnShake == null)
        {
            OnShake = new UnityEvent();
        }
        if (OnShake.GetPersistentEventCount() == 0)
        {
            OnShake.AddListener(ShakeDetectedAction); // Add a default listener
        }
    }

    /// <summary>
    /// Called at a fixed interval, suitable for physics calculations.
    /// Calculates acceleration and checks for shake patterns.
    /// </summary>
    void FixedUpdate()
    {
        // --- Cooldown Check ---
        if (shakeCooldownTimer > 0)
        {
            shakeCooldownTimer -= Time.fixedDeltaTime;
            // Reset velocity/acceleration tracking during cooldown to avoid false positives right after
            lastPositionY = transform.position.y;
            lastVelocityY = 0f;
            lastAccelerationY = 0f;
            shakeTimestamps.Clear(); // Clear timestamps during cooldown
            return;
        }

        // --- Calculate Current Velocity and Acceleration ---
        float currentPositionY = transform.position.y;
        // Velocity = distance / time
        float currentVelocityY = (currentPositionY - lastPositionY) / Time.fixedDeltaTime;
        // Acceleration = change in velocity / time
        float currentAccelerationY = (currentVelocityY - lastVelocityY) / Time.fixedDeltaTime;

        // --- Detect Acceleration Peaks ---
        // Check if the acceleration exceeds the threshold AND changes direction significantly
        // (e.g., from strong up to strong down or vice-versa)
        if (Mathf.Abs(currentAccelerationY) > shakeThreshold && Mathf.Sign(currentAccelerationY) != Mathf.Sign(lastAccelerationY))
        {
            // Record the time of this significant acceleration change
            shakeTimestamps.Add(Time.time);
            // Optional: Log the detected peak for debugging
            // Debug.Log($"Shake Peak Detected: Accel={currentAccelerationY:F2} at Time={Time.time:F2}");
        }

        // --- Clean Up Old Timestamps ---
        // Remove timestamps that are older than the detection window
        shakeTimestamps.RemoveAll(timestamp => Time.time - timestamp > shakeDetectionTime);

        // --- Check for Shake Event ---
        if (shakeTimestamps.Count >= minShakeCount)
        {
            // --- Shake Detected! ---
            TriggerShakeEvent();

            // Clear timestamps and start cooldown
            shakeTimestamps.Clear();
            shakeCooldownTimer = shakeCooldown;
        }

        // --- Update Values for Next Frame ---
        lastPositionY = currentPositionY;
        lastVelocityY = currentVelocityY;
        // Store the acceleration only if it was significant, helps track direction changes
        if (Mathf.Abs(currentAccelerationY) > shakeThreshold * 0.5f) // Store if reasonably large
        {
            lastAccelerationY = currentAccelerationY;
        }
    }

    /// <summary>
    /// Called when a shake event is successfully detected.
    /// Invokes the OnShake UnityEvent.
    /// </summary>
    void TriggerShakeEvent()
    {
        Debug.Log("Shake Detected!"); // Log message to console
        OnShake.Invoke(); // Trigger the UnityEvent
    }

    /// <summary>
    /// A default public function that can be called when a shake is detected.
    /// You can replace this or assign other functions via the Unity Inspector.
    /// </summary>
    public void ShakeDetectedAction()
    {
        Debug.Log("ShakeDetectedAction() was called!");
        // --- Add your custom shake response code here! ---
        // For example:
        // - Play a sound effect
        // - Trigger an animation
        // - Instantiate a particle effect
        // - Increase a score
        // - etc.
    }
}
