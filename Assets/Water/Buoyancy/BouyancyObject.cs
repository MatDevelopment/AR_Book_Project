using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class BouyancyObject : MonoBehaviour
{
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;

    public float waterHeight = 0f;

    Rigidbody rb;

    bool isUnderwater;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float difference = transform.position.y - waterHeight;

        if (difference < 0 && isUnderwater)
        {
            rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), transform.position, ForceMode.Force);
        }
    }

    void SwitchStates(bool underwater)
    {
        if (underwater)
        {
            rb.linearDamping = underWaterDrag;
            rb.angularDamping = underWaterAngularDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
            rb.angularDamping = airAngularDrag;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("A collider has entered the Water trigger");
        isUnderwater = true;
        SwitchStates(true);
    }

    void OnTriggerExit(Collider other)
    {
        // Debug.Log("A collider has exited the Water trigger");
        isUnderwater = false;
        SwitchStates(false);
    }
}
