using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class WaterCollision : MonoBehaviour
{
    public GameObject waterSphere;
    private WaterRain rainManager;

    private bool waterInitiated = false;
    public bool collidedWithWater = false;

    private float forceMultiplier = 1f; // Adjust force applied
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("CollisionForce: Rigidbody component missing.");
        }

        rainManager = GameObject.Find("/RainManager").GetComponent<WaterRain>();

        // When instantiated wait for x amount of seconds before being able to merge
        StartCoroutine(WaitBeforeInitializing(1f));
    }

    // “other” refers to the collider that is touching this collider
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("paddle") && waterInitiated)
        {
            Debug.Log($"A collider has made contact with the water wit a speed of: {other.relativeVelocity.magnitude}");
            
            rainManager.score += 1;
            rainManager.scoreText.text = rainManager.score.ToString();

            if (other.relativeVelocity.magnitude > 2)
            {
                Debug.Log("Water is split in two");

                // Spawn two new water spheres
                GameObject newWater1 = Instantiate(waterSphere, transform.position, transform.rotation);
                newWater1.transform.localScale = gameObject.transform.localScale * 0.5f;

                GameObject newWater2 = Instantiate(waterSphere, transform.position, transform.rotation);
                newWater2.transform.localScale = gameObject.transform.localScale * 0.5f;

                // Destroy original WaterSphere
                Destroy(gameObject);

            } else {
                // Get direction and force
                Vector3 forceDirection = other.contacts[0].point - transform.position;
                forceDirection = -forceDirection.normalized; // Reverse direction to apply force away from impact

                float impactStrength = other.relativeVelocity.magnitude;

                // Apply force based on impact strength
                rb.AddForce(forceDirection * impactStrength * forceMultiplier, ForceMode.Impulse);
            }
        }
        else if (other.gameObject.CompareTag("waterSphere") && waterInitiated)
        {
            // To avoid having both sphere spawn a new one we check the others script if it has started
            if (!other.gameObject.GetComponent<WaterCollision>().collidedWithWater)
            {
                collidedWithWater = true;

                rainManager.score += 1;
                rainManager.scoreText.text = rainManager.score.ToString();

                // When two water droplets collide they form one bigger water
                GameObject newWaterCombined = Instantiate(waterSphere, other.GetContact(0).point, transform.rotation);
                newWaterCombined.transform.localScale = gameObject.transform.localScale + other.gameObject.transform.localScale;

                // Destroy both old smaller droplets
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
    }
    
    IEnumerator WaitBeforeInitializing(float waitTime)
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(waitTime);
        waterInitiated = true;
    }
}
