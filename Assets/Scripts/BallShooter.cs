using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallShooter : MonoBehaviour
{
    public TMP_Text phaseDisplayText;
    
    private Touch theTouch;

    private float timeTouchEnded;
    private float displayTime = 0.5f;
    private float ball_Thrust = 100f;

    [SerializeField] private GameObject  ballGameObject;
    
    [SerializeField] private Transform  ARcameraTransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)   //If the amount of current touches on the screen is more than 0...
        {
            theTouch = Input.GetTouch(0);
            if (theTouch.phase == TouchPhase.Began)
            {
                GameObject instantiatedBall = Instantiate(ballGameObject, ARcameraTransform);
                instantiatedBall.GetComponent<Rigidbody>().AddForce(Vector3.forward * ball_Thrust, ForceMode.Impulse);
            }
            theTouch = Input.GetTouch(0);
            if (theTouch.phase == TouchPhase.Ended)
            {
                phaseDisplayText.text = theTouch.phase.ToString();
                timeTouchEnded = Time.time;
            }
            else if (Time.time - timeTouchEnded > displayTime)
            {
                phaseDisplayText.text = theTouch.phase.ToString();
                timeTouchEnded = Time.time;
            }
            
        }
        else if (Time.time -timeTouchEnded > displayTime)
        {
            phaseDisplayText.text = "";
        }
    }
}
