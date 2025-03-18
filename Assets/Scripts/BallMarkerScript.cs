using System;
using System.Collections;
using UnityEngine;


public class BallMarkerScript : MonoBehaviour
{
    [SerializeField] private GameObject TennisBallPosition_Top;
    [SerializeField] private GameObject TennisBallPosition_Bottom;

    [SerializeField] private GameObject TennisBallPosition_Side1;
    [SerializeField] private GameObject TennisBallPosition_Side2;
    [SerializeField] private GameObject TennisBallPosition_Side3;
    [SerializeField] private GameObject TennisBallPosition_Side4;

    private GameObject newTransformPosition;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public IEnumerator MoveObjectFunction(string TennisBallSideShowing)
    {
        DecideTransformFromQRcode(TennisBallSideShowing);
        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, newTransformPosition.transform.position, timeSinceStarted);

            // If the object has arrived, stop the coroutine
            if (gameObject.transform.position == newTransformPosition.transform.position)
            {
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }

    private void DecideTransformFromQRcode(string tennisBallSideShowing)
    {
        switch (tennisBallSideShowing)
        {
            case "top":
                newTransformPosition = TennisBallPosition_Top;
                Debug.Log("top detected");
                break;
            case "bottom":
                newTransformPosition = TennisBallPosition_Bottom;
                Debug.Log("bottom detected");
                break;
            case "side 1":
                newTransformPosition = TennisBallPosition_Side1;
                Debug.Log("side 1 detected");
                break;
            case "side 2":
                newTransformPosition = TennisBallPosition_Side2;
                Debug.Log("side 2 detected");
                break;
            case "side 3":
                newTransformPosition = TennisBallPosition_Side3;
                Debug.Log("side 3 detected");
                break;
            case "side 4":
                newTransformPosition = TennisBallPosition_Side4;
                Debug.Log("side 4 detected");
                break;
        }
    }
}
