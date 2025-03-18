using UnityEngine;
using System.Collections;

public class WaterCollision : MonoBehaviour
{
    public GameObject waterSphere;

    private bool waterInitiated = false;

    private void Awake()
    {
        // When instantiated wait for x amount of seconds before being able to merge
        StartCoroutine(WaitBeforeInitializing(2f));
    }

    // “other” refers to the collider that is touching this collider
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("paddle") && waterInitiated)
        {
            if (other.relativeVelocity.magnitude > 1)
            {
                Debug.Log("Water is split in two");

                // Spawn two new water spheres
                GameObject newWater1 = Instantiate(waterSphere, transform.position, transform.rotation);
                newWater1.transform.localScale = gameObject.transform.localScale * 0.5f;

                GameObject newWater2 = Instantiate(waterSphere, transform.position, transform.rotation);
                newWater2.transform.localScale = gameObject.transform.localScale * 0.5f;

                // Destroy original WaterSphere
                Destroy(gameObject);

            }

            Debug.Log($"A collider has made contact with the water wit a speed of: {other.relativeVelocity.magnitude}");
        }
        else if (other.gameObject.CompareTag("waterSphere") && waterInitiated)
        {
            // When two water droplets collide they form one bigger water
            GameObject newWaterCombined = Instantiate(waterSphere, other.GetContact(0).point, transform.rotation);
            newWaterCombined.transform.localScale = gameObject.transform.localScale + other.gameObject.transform.localScale;

            // Destroy both old smaller droplets
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
    
    IEnumerator WaitBeforeInitializing(float waitTime)
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(waitTime);
        waterInitiated = true;
    }
}
