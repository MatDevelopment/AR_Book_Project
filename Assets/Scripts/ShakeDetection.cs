using UnityEngine;

public class ShakeDetection : MonoBehaviour
{
    // Adjustable parameters
    public float shakeThreshold = 2.0f;      // Threshold to detect shaking
    public float decreaseRate = 1.0f;        // How fast the float decreases per second
    public float targetValue = 100.0f;       // The float value to decrease

    private Vector3 lastAcceleration;
    private float lowPassFilterFactor = 0.2f;       // LOW PASS FILTER PROBABLY TOO LOW //

    void Start()
    {
        lastAcceleration = Input.acceleration;
    }

    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        Vector3 deltaAcceleration = acceleration - lastAcceleration;
        lastAcceleration = Vector3.Lerp(lastAcceleration, acceleration, lowPassFilterFactor);

        if (deltaAcceleration.sqrMagnitude >= shakeThreshold * shakeThreshold)          // If the difference in acceleration from the last update is more than or equal to the shakeThreshold, then a shake is detected
        {
            // Shake detected, decrease the value
            targetValue -= decreaseRate * Time.deltaTime;
            targetValue = Mathf.Max(targetValue, 0); // Prevent it from going below 0
            Debug.Log("Shaking! Value: " + targetValue);
        }
    }
}
