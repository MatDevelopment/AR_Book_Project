using System;
using UnityEngine;

public class RoverControl : MonoBehaviour
{
    public GameObject mainCamera;
    public float spawnDistance = 1f; // Distance in front of the camera
    public GameObject roverPrefab;

    public GameObject spawnButton;
    public GameObject despawnButton;

    private GameObject rover;
    public Rigidbody roverRB;
    public float moveSpeed = 20f;
    public float rotateSpeed = 10f;

    private bool movingForward = false;
    private bool movingBackward = false;
    private bool rotatingLeft = false;
    private bool rotatingRight = false;

    private void Start()
    {
        spawnButton.SetActive(true);
        despawnButton.SetActive(false);
    }

    public void SpawnRover()
    {
        // Determine spawn position in front of the camera
        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * spawnDistance;

        // Spawn the rover
        rover = Instantiate(roverPrefab, spawnPosition, Quaternion.identity);
        roverRB = rover.GetComponent<Rigidbody>();

        // Hide spawn rover button
        spawnButton.SetActive(false);
        despawnButton.SetActive(true);
    }

    public void DespawnRover()
    {
        // Remove the rover
        Destroy(rover);

        // Show spawn rover again
        spawnButton.SetActive(true);
        despawnButton.SetActive(false);
    }

    void FixedUpdate()
    {
        if (roverRB != null)
        {
            if (movingForward)
            {
                roverRB.AddForce(rover.transform.forward * moveSpeed, ForceMode.Force);
            }
            if (movingBackward)
            {
                roverRB.AddForce(-rover.transform.forward * moveSpeed, ForceMode.Force);
            }
            if (rotatingLeft)
            {
                roverRB.AddTorque(Vector3.up * -rotateSpeed);
            }
            if (rotatingRight)
            {
                roverRB.AddTorque(Vector3.up * rotateSpeed);
            }

            // **Limit Maximum Speed**
            float maxSpeed = 4f;  // Adjust as needed
            roverRB.linearVelocity = Vector3.ClampMagnitude(roverRB.linearVelocity, maxSpeed);

            // **Limit Maximum Rotation Speed**
            float maxAngularSpeed = 5f;  // Adjust as needed
            roverRB.angularVelocity = Vector3.ClampMagnitude(roverRB.angularVelocity, maxAngularSpeed);
        }
    }

    // Methods for Event Trigger assignment

    public void StartMovingForward()
    {
        movingForward = true;
        Debug.Log("Moving Forward Started");
    }

    public void StopMovingForward()
    {
        movingForward = false;
        Debug.Log("Moving Forward Stopped");
    }
    //public void StartMovingForward() => movingForward = true;
    //public void StopMovingForward() => movingForward = false;
    public void StartMovingBackward() => movingBackward = true;
    public void StopMovingBackward() => movingBackward = false;

    public void StartRotatingLeft() => rotatingLeft = true;
    public void StopRotatingLeft() => rotatingLeft = false;

    public void StartRotatingRight() => rotatingRight = true;
    public void StopRotatingRight() => rotatingRight = false;

    // Old Button Inputs
    //public void MoveForward(float speed)
    //{
    //    roverRB.AddForce(rover.transform.forward * speed, ForceMode.Impulse);
    //}

    //public void MoveBackward(float speed)
    //{
    //    roverRB.AddForce(-rover.transform.forward * speed, ForceMode.Impulse);
    //}

    //public void TurnLeft(float rotateSpeed)
    //{
    //    roverRB.AddTorque(Vector3.up * -rotateSpeed, ForceMode.Impulse);
    //}

    //public void TurnRight(float rotateSpeed)
    //{
    //    roverRB.AddTorque(Vector3.up * rotateSpeed, ForceMode.Impulse);
    //}
}
