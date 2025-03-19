using System;
using TMPro;
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

    [SerializeField]
    [Tooltip("Material for the moon surface")]
    Material m_moonMaterial;

    [SerializeField]
    [Tooltip("Material for the mars surface")]
    Material marsMaterial;

    public TextMeshProUGUI gravityStrengthText;
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

    private void Start()
    {
        trackablesParent = GameObject.Find("Trackables");
        m_PlanetButton.gameObject.SetActive(true);
    }

  
    private void OnEnable() // Subscribing to events
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
        m_PlanetButton.onClick.AddListener(ChangePlanet);
    }

    private void OnDisable() // Unsubscribing to events
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
        m_PlanetButton.onClick.RemoveListener(ChangePlanet);

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