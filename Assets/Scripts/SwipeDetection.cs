using System;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

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

    public float throwStrengthMultiplier = 10f;
    public float upStrengthMultiplier = 1;
    public float sideStrengthMultiplier = 1;

    public bool mode2Enabled = true;

    [SerializeField]
    [Tooltip("Material for the moon surface")]
    Material m_moonMaterial;

    [SerializeField]
    [Tooltip("Material for the mars surface")]
    Material marsMaterial;

    public TextMeshProUGUI gravityStrengthText, text_mode, text_multiplier, text_multiplierUp, text_multiplierSide;
    //gravityStrengthText.text = gravityStrength.ToString();

    enum planetType
    {
        Earth,
        Moon,
        Mars
    }
    [SerializeField]
    planetType m_planetType = planetType.Moon;

    [SerializeField]
    private float gravityStrength = -1f;

    public GameObject trackablesParent;

    private Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main;
        inputManager = Gravity_InputManager.Instance;

    }

    public Button m_PlanetButton;

    /// <summary>
    /// Button that opens the create menu.
    /// </summary>
    public Button planetButton
    {
        get => m_PlanetButton;
        set => m_PlanetButton = value;
    }

    public Slider m_MultiplierSlider;
    public Slider m_multiplierSlider
    {
        get => m_MultiplierSlider;
        set => m_MultiplierSlider = value;
    }

    public Slider m_MultiplierSliderUp;
    public Slider m_multiplierSliderUp
    {
        get => m_MultiplierSliderUp;
        set => m_MultiplierSliderUp = value;
    }

    public Slider m_MultiplierSliderSide;

    //public Button m_ModeButton;

    ///// <summary>
    ///// Button that opens the create menu.
    ///// </summary>
    //public Button m_modeButton
    //{
    //    get => m_ModeButton;
    //    set => m_ModeButton = value;
    //}

    private void Update()
    {
        text_multiplier.text = "Throw strength multiplier: " + m_MultiplierSlider.value.ToString();
        text_multiplierUp.text = "Up multiplier: " + m_MultiplierSliderUp.value.ToString();

        text_multiplierSide.text = "Side direction multiplier: " + m_MultiplierSliderSide.value.ToString();
        //EnableOnlyLowestPlane();

    }

    //private void EnableOnlyLowestPlane() Unused code!
    //{
    //    Transform lowestPlane;

    //    float lowestY = float.MaxValue;

    //    for (int i = 0, count = trackablesParent.transform.childCount; i < count; i++)
    //    {
    //        if(trackablesParent.transform.GetChild(i).position.y < lowestY)
    //        {
    //            lowestY = trackablesParent.transform.GetChild(i).position.y;
    //            lowestPlane = trackablesParent.transform.GetChild(i);
    //            lowestPlane.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            trackablesParent.transform.GetChild(i).gameObject.SetActive(false);
    //        }
    //    }
    //}

    private void Start()
    {
        trackablesParent = GameObject.Find("Trackables");
        m_PlanetButton.gameObject.SetActive(true);
        //m_ModeButton.gameObject.SetActive(true);

        gravityStrength = -1.62f;
        Physics.gravity = new Vector3(0, gravityStrength, 0);
        gravityStrengthText.text = gravityStrength.ToString();

    }


    private void OnEnable() // Subscribing to events
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
        m_PlanetButton.onClick.AddListener(ChangePlanet);
        //m_ModeButton.onClick.AddListener(ChangeMode);
        m_MultiplierSlider.onValueChanged.AddListener(SetMultiplierValue);
        m_MultiplierSliderUp.onValueChanged.AddListener(SetMultiplierUpValue);
        m_MultiplierSliderSide.onValueChanged.AddListener(SetMultiplierSideValue);

    }

    private void OnDisable() // Unsubscribing to events
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
        m_PlanetButton.onClick.RemoveListener(ChangePlanet);
        //m_ModeButton.onClick.RemoveListener(ChangeMode);
        m_MultiplierSlider.onValueChanged.RemoveListener(SetMultiplierValue);
        m_MultiplierSliderUp.onValueChanged.RemoveListener(SetMultiplierUpValue);
        m_MultiplierSliderSide.onValueChanged.RemoveListener(SetMultiplierSideValue);

    }

    //private void ChangeMode()
    //{
    //    if (mode2Enabled)
    //    {
    //        mode2Enabled = false;
    //        text_mode.text = "Mode 1";
    //    }
    //    else
    //    {
    //        mode2Enabled = true;
    //        text_mode.text = "Mode 2";
    //    }
    //}

    void ChangePlanet()
    {
        if (m_planetType == planetType.Mars)
        {
            m_planetType = planetType.Moon;
            gravityStrength = -1.62f;
            Physics.gravity = new Vector3(0, gravityStrength, 0);
        }
        else if (m_planetType == planetType.Moon)
        {
            m_planetType = planetType.Mars;
            gravityStrength = -3.71f;
            Physics.gravity = new Vector3(0, gravityStrength, 0);
        }

        gravityStrengthText.text = gravityStrength.ToString();
        ChangeMaterialsOnChildren();
    }
    void ChangeMaterialsOnChildren()
    {
        if (trackablesParent == null || trackablesParent.transform.childCount == 0)
        {
            Debug.LogWarning("No children found in the trackables parent. Cannot change materials.");
            return;
        }
        foreach (Transform child in trackablesParent.transform)
        {
            if (child.gameObject.TryGetComponent<Renderer>(out var renderer))
            {
                if (m_planetType == planetType.Mars)
                {
                    renderer.material = marsMaterial;
                }
                else if (m_planetType == planetType.Moon)
                {
                    renderer.material = m_moonMaterial;
                }
                //else
                //{
                //    renderer.material = m_earthMaterial;
                //}
            }
        }
    }

    private void SetMultiplierValue(float input)
    {
        throwStrengthMultiplier = input;
    }
    private void SetMultiplierUpValue(float input)
    {
        upStrengthMultiplier = input;
    }
    private void SetMultiplierSideValue(float input)
    {
        sideStrengthMultiplier = input;
    }
    private void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        Debug.Log("startPosition: " + position);
        startTime = time;
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        endPosition = position;
        Debug.Log("endPosition: " + position);

        endTime = time;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        float distance = Vector3.Distance(startPosition, endPosition);
        float timeDifference = endTime - startTime;

        float yDistance = endPosition.y - startPosition.y;

        //Debug.Log("Swipe detected-   distance: " + distance + "   timeDifference: " + timeDifference + "     y Doistance: " + yDistance + "    y Doistance times: " + yDistance * 100);

        if (distance >= minimumDistance && timeDifference <= maximumTime)
        {
            Debug.DrawLine(startPosition, endPosition, Color.red, 5f);

            SpawnTennisBall(yDistance, timeDifference);
        }
    }
    public float CalculateThrowStrength(float distance, float timeDifference)
    {
        // Avoid division by zero.
        if (timeDifference <= 0)
        {
            return 0;
        }

        // Calculate swipe speed.
        float speed = distance / timeDifference;

        // Use a multiplier to adjust the final strength value.
        // Adjust this constant as needed.
        float strength = speed * throwStrengthMultiplier;

        return strength;
    }

    public float CalculateUpStrength(float distance, float timeDifference)
    {
        // Avoid division by zero.
        if (timeDifference <= 0)
        {
            return 0;
        }

        // Calculate swipe speed.
        float speed = distance / timeDifference;

        // Use a multiplier to adjust the final strength value.
        // Adjust this constant as needed.
        float strength = speed * upStrengthMultiplier;

        return strength;
    }

    private void SpawnTennisBall(float yDistance, float timeDifference)
    {
        float throwSpeed = CalculateThrowStrength(yDistance, timeDifference);
        float upSpeed = CalculateUpStrength(yDistance, timeDifference);

        GameObject tennisBall = Instantiate(Resources.Load("LowPoly_TennisBall")) as GameObject;

        // Position the ball relative to the camera.
        tennisBall.transform.position = mainCamera.transform.position +
                                          mainCamera.transform.forward * 0.4f +
                                          mainCamera.transform.up * -0.2f;
        tennisBall.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        Rigidbody ballRigidbody = tennisBall.GetComponent<Rigidbody>();

        // Calculate the world-space swipe direction.
        Vector2 swipeDirection = (startPosition - endPosition).normalized;
        Debug.Log("swipeDirection: " + swipeDirection + "  sX:" + startPosition.x + "  eX:" + endPosition.x);

        // Combine the throw directions:
        // - Use the camera's forward direction multiplied by the calculated throw speed.
        // - Add the swipe direction multiplied by the side strength multiplier.
        // - Add an upward component.
        Vector3 finalThrowDirection =
            mainCamera.transform.forward * throwSpeed +
            mainCamera.transform.right * (swipeDirection.x * sideStrengthMultiplier) +
            Vector3.up * upSpeed;

        // Apply the combined force.
        ballRigidbody.AddForce(finalThrowDirection, ForceMode.Impulse);
    }
}