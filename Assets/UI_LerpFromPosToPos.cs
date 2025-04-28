using UnityEngine;

public class UI_LerpFromPosToPos : MonoBehaviour
{
    public Vector2 startPos; // Set in Inspector
    public Vector2 endPos;   // Set in Inspector
    public float speed = 1f; // How fast it moves (set in Inspector)

    private RectTransform rectTransform;
    private float t = 0f;
    private bool goingForward = true;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("UI_LerpFromPosToPos requires a RectTransform component!");
        }
    }

    void Update()
    {
        if (rectTransform == null) return;

        // Update interpolation value
        if (goingForward)
        {
            t += Time.deltaTime * speed;
            if (t >= 1f)
            {
                t = 1f;
                goingForward = false;
            }
        }
        else
        {
            t -= Time.deltaTime * speed;
            if (t <= 0f)
            {
                t = 0f;
                goingForward = true;
            }
        }

        // Apply the lerped position
        rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
    }
}
