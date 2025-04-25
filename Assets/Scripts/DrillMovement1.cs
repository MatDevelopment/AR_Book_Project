using UnityEngine;

public class DrillMovement1 : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 5f; // How fast to move toward targetY
    [SerializeField] private float drillStepPercent = 0.08f; // Each button press moves this % closer

    private Vector3 startPosition;
    public float progress = 0f; // Progress from 0 to 100
    private float targetY;

    public float layerTemperature;
    public float riseInTemperaturePerClick;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not set.");
            return;
        }

        startPosition = transform.position;
        targetY = startPosition.y;
    }

    private void Update()
    {
        // Smoothly move toward the targetY
        Vector3 currentPosition = transform.position;
        float newY = Mathf.Lerp(currentPosition.y, targetY, Time.deltaTime * moveSpeed);
        
        if (Mathf.Abs(targetY - currentPosition.y) > 0.001f)
        {
            // Apply shake
            // Add subtle shake on X and Z while moving
            float shakeStrength = 0.02f;
            float shakeX = Random.Range(-shakeStrength, shakeStrength);
            float shakeZ = Random.Range(-shakeStrength, shakeStrength);
            
            transform.position = new Vector3(startPosition.x + shakeX, newY, startPosition.z + shakeZ);
        }
        else
        {
            // No shake, stay still
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

    }

    /// <summary>
    /// Call this to drill forward a bit. Each call adds to the target Y position.
    /// </summary>
    public void DrillStep()
    {
        layerTemperature += riseInTemperaturePerClick;
        if (target == null || progress >= 100f) return;
        
        progress = Mathf.Clamp(progress + drillStepPercent, 0f, 100f);
        float newY = Mathf.Lerp(startPosition.y, target.position.y, progress / 100f);
        targetY = newY;
    }
}