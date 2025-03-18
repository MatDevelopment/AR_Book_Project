using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class RippleAtMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] ParticleSystem rippleParticleSystem;
    [SerializeField] float rippleSensitivity = 0.1f;

    private bool inWater = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocity.x > 0.1 * rippleSensitivity || rb.linearVelocity.y > 0.1 * rippleSensitivity || rb.linearVelocity.z > 0.1 * rippleSensitivity && inWater)
        {
            rippleParticleSystem.Play();
        } 
        else if (rippleParticleSystem.isPlaying)
        {
            rippleParticleSystem.Pause();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("A collider has entered the Water trigger");
        inWater = true;
    }

    void OnTriggerExit(Collider other)
    {
        // Debug.Log("A collider has exited the Water trigger");
        inWater = false;
    }
}
