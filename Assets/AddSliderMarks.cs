using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddSliderMarks : MonoBehaviour
{
    //public ControlTimeWithDistance controlTimeWithDistance;
    public List<Image> sliderMarks = new List<Image>();
    public List<float> sliderMarkValues = new List<float>();
    public GameObject sliderMarkPrefab;

    [Header("Marker Settings")]
    public float animationDuration = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float min, max = 750;
    private Vector2 minSize = new Vector2(50, 50);
    private Vector2 maxSize = new Vector2(60, 60);
    private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    private Color activeColor = new Color(1f, 1f, 1f, 1f);

    // Track which markers are currently active to detect state changes
    private List<bool> markerActiveStates = new List<bool>();
    // Store ongoing animations to avoid conflicts
    private Dictionary<int, Coroutine> activeAnimations = new Dictionary<int, Coroutine>();

    void Start()
    {
        //controlTimeWithDistance = FindAnyObjectByType<ControlTimeWithDistance>();
        AddMarks();
        // Initialize all markers as inactive
        for (int i = 0; i < sliderMarks.Count; i++)
        {
            markerActiveStates.Add(false);
            sliderMarks[i].color = inactiveColor;
            sliderMarks[i].rectTransform.sizeDelta = minSize;
        }
    }

    public void AddMarks()
    {
        for (int i = 0; i < sliderMarkValues.Count; i++)
        {
            GameObject mark = Instantiate(sliderMarkPrefab, transform) as GameObject;
            float xPos = sliderMarkValues[i]; // this is the actual value you want to remap
            float remappedX = (float)ExtensionMethods.Remap(xPos, 0, 10, min, max);
            mark.GetComponent<RectTransform>().anchoredPosition = new Vector2(remappedX-15, 32);
            sliderMarks.Add(mark.GetComponent<Image>());
        }
    }

    public void UpdateSliderMarks(float inputDistance)
    {
        for (int i = 0; i < sliderMarks.Count; i++)
        {
            bool shouldBeActive = inputDistance >= sliderMarkValues[i] - 0.1f && inputDistance < sliderMarkValues[i] + 1;

            // Only animate if state is changing
            if (shouldBeActive != markerActiveStates[i])
            {
                markerActiveStates[i] = shouldBeActive;

                // Stop any ongoing animation for this marker
                if (activeAnimations.ContainsKey(i) && activeAnimations[i] != null)
                {
                    StopCoroutine(activeAnimations[i]);
                }

                // Start a new animation
                activeAnimations[i] = StartCoroutine(AnimateMarker(i, shouldBeActive));
            }
        }
    }

    private IEnumerator AnimateMarker(int markerIndex, bool toActive)
    {
        Image marker = sliderMarks[markerIndex];
        float elapsedTime = 0;

        // Starting values
        Color startColor = marker.color;
        Vector2 startSize = marker.rectTransform.sizeDelta;

        // Target values
        Color targetColor = toActive ? activeColor : inactiveColor;
        Vector2 targetSize = toActive ? maxSize : minSize;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / animationDuration;
            float curveValue = animationCurve.Evaluate(normalizedTime);

            // Apply interpolated values
            marker.color = Color.Lerp(startColor, targetColor, curveValue);
            marker.rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, curveValue);

            yield return null;
        }

        // Ensure final values are exactly as specified
        marker.color = targetColor;
        marker.rectTransform.sizeDelta = targetSize;

        // Remove from active animations
        activeAnimations.Remove(markerIndex);
    }
}