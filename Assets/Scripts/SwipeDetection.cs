using System;
using TMPro;
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


    public TextMeshProUGUI gravityStrengthText;
    //gravityStrengthText.text = gravityStrength.ToString();


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

        float yDistance = endPosition.y - startPosition.y;

        Debug.Log("Swipe detected-   distance: " + distance + "   timeDifference: " + timeDifference +  "     y Doistance: " + yDistance +"    y Doistance times: " + yDistance * 100);

        if (distance >= minimumDistance && timeDifference <= maximumTime)
        {
            Debug.DrawLine(startPosition, endPosition, Color.red, 5f);

            SpawnTennisBall(yDistance);
        }
    }

    private void SpawnTennisBall(float yDistance)
    {
        float forwardStrength = yDistance * 100;

        GameObject tennisBall = Instantiate(Resources.Load("LowPoly_TennisBall")) as GameObject;
        tennisBall.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 1.3f;
        Vector3 direction = endPosition - startPosition;
        tennisBall.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        tennisBall.GetComponent<Rigidbody>().AddForce(direction.normalized * 4, ForceMode.Impulse);
        tennisBall.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * forwardStrength, ForceMode.Impulse);
    }
}