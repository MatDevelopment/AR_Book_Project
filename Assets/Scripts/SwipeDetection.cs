using System;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
   private Gravity_InputManager inputManager;

    [SerializeField] // So we can access this private variable in the inspector
    private float minimumDistance = 0.2f;

    [SerializeField]
    private float maximumTime = 1;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    private Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main;
        inputManager = Gravity_InputManager.Instance;
    }

    private void OnEnable() // Subscribing to events
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable() // Unsubscribing to events
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        startTime = time;
        Debug.Log("SwipeStart");

    }

    private void SwipeEnd(Vector2 position, float time)
    {
        endPosition = position;
        endTime = time;
        DetectSwipe();
        Debug.Log("SwipeEnd!");

    }

    private void DetectSwipe()
    {
         float distance = Vector3.Distance(startPosition, endPosition);
         float timeDifference = endTime - startTime;


            Debug.Log("Swipe detected-   distance: " + distance + "   timeDifference: " + timeDifference);
        if (distance >= minimumDistance && timeDifference <= maximumTime)
        {
            Debug.DrawLine(startPosition, endPosition, Color.red, 5f);
         
            SpawnTennisBall();
        }

       
    }

    private void SpawnTennisBall()
    {
        GameObject tennisBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tennisBall.AddComponent<Rigidbody>();
        tennisBall.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 1.3f;
        Vector3 direction = endPosition - startPosition + mainCamera.transform.forward * 1.3f;
        tennisBall.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        tennisBall.GetComponent<Rigidbody>().AddForce(direction.normalized * 4, ForceMode.Impulse);
    }
}

