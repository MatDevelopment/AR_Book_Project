using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_PulseColor : MonoBehaviour // From Claude 3.7 Sonnet!
{
    [Header("Color Settings")]
    private Color firstColor = new Color(0.5756497f, 0.8301887f, 0.8007889f, 1f);
    private Color secondColor = new Color(1f, 1f, 1f, 1f);

    [Header("Pulse Settings")]
    private float pulseSpeed = 3f;
    [Range(0f, 1f)] private float smoothness = 1f;

    // Component references
    private TextMeshProUGUI tmpText;
    private Image uiImage;

    // Tracking variables
    private float timeCounter = 0f;
    private bool hasValidComponent = false;

    void Start()
    {
        // Try to get the TextMeshPro component
        tmpText = GetComponent<TextMeshProUGUI>();

        // Try to get the Image component if TextMeshPro doesn't exist
        if (tmpText == null)
        {
            uiImage = GetComponent<Image>();
        }

        // Check if we have a valid component to work with
        hasValidComponent = (tmpText != null || uiImage != null);

        if (!hasValidComponent)
        {
            Debug.LogWarning("UI_PulseColor script requires either a TextMeshProUGUI or Image component.", this);
        }
    }

    void Update()
    {
        if (!hasValidComponent) return;

        // Increment time counter
        timeCounter += Time.deltaTime * pulseSpeed;

        // Calculate pulse value using sine wave (0 to 1)
        float pulseValue = Mathf.SmoothStep(0f, 1f, (Mathf.Sin(timeCounter) + 1f) / 2f);

        // Lerp between the two colors based on the pulse value
        Color lerpedColor = Color.Lerp(firstColor, secondColor, pulseValue);

        // Apply the color to the appropriate component
        if (tmpText != null)
        {
            tmpText.color = lerpedColor;
        }
        else if (uiImage != null)
        {
            uiImage.color = lerpedColor;
        }
    }
}