using UnityEngine;

public class PlanetCollisionTrigger : MonoBehaviour
{
    // Script should sit on the rocket that collides with the planets

    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Earth":
                // Earth something
                Debug.Log("You hit Earth");
                break;
            case "Moon":
                // Moon Exercise
                Debug.Log("You hit the Moon");
                break;
            case "Mars":
                Debug.Log("You hit Mars");
                break;
        }
    }
}
