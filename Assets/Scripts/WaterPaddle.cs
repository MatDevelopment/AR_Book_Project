using UnityEngine;

public class WaterPaddle : MonoBehaviour
{
    public Transform target; // The target to follow

    // New Physics Based Lerp

    public float forceStrength = 10f; // Strength of the applied force
    public float torqueStrength = 5f; // Strength of the applied torque
    public float positionThreshold = 0.1f; // Distance threshold to stop movement
    public float rotationThreshold = 1f; // Rotation angle threshold to stop torque
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("SmoothFollowPhysics: Rigidbody component missing.");
        }
    }

    void FixedUpdate()
    {
        if (target == null || rb == null)
        {
            Debug.LogWarning("SmoothFollowPhysics: No target assigned or Rigidbody missing.");
            return;
        }

        // Calculate position difference
        Vector3 forceDirection = (target.position - transform.position);
        if (forceDirection.magnitude > positionThreshold)
        {
            rb.AddForce(forceDirection.normalized * forceStrength, ForceMode.Acceleration);
        }
        else
        {
            rb.linearVelocity = Vector3.zero; // Stop movement if within threshold
        }

        // Calculate rotation difference and apply torque
        Quaternion targetRotation = target.rotation;
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float torqueAngle, out Vector3 torqueAxis);

        if (!float.IsNaN(torqueAngle) && torqueAngle > rotationThreshold)
        {
            rb.AddTorque(torqueAxis * torqueAngle * torqueStrength, ForceMode.Acceleration);
        }
        else
        {
            rb.angularVelocity = Vector3.zero; // Stop rotation if within threshold
        }
    }

    // Deprecated Lerp Transform

    //public float positionLerpSpeed = 5f; // Speed of position interpolation
    //public float rotationLerpSpeed = 5f; // Speed of rotation interpolation

    //void Update()
    //{
    //    if (target == null)
    //    {
    //        Debug.LogWarning("SmoothFollow: No target found.");
    //        return;
    //    }

    //    // Interpolating position
    //    transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * positionLerpSpeed);

    //    // Interpolating rotation
    //    transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationLerpSpeed);
    //}
}
