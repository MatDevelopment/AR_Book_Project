using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DrillMovement1 : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float drillStepPercent = 0.08f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drillLoopSFX;
    [SerializeField] private AudioClip idleLoopSFX;
    [SerializeField] private float stopDelay = 1f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    public float layerTemperature;
    public float riseInTemperaturePerClick;
    
    private Vector3 startPosition;
    public float progress = 0f;
    private float targetY;

    private float lastPressTime = Mathf.NegativeInfinity;
    private bool isFadingOut = false;
    private bool isIdling = false;

    [SerializeField] private AndroidVibrator _androidVibrator;
    
    // Reset position for the drill and the sliced Earth
    [SerializeField] private SpawnDrill _spawnDrill;
    [SerializeField] private GameObject ResetPosition_UIElement;
    private bool _isResettingPosition = false;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not assigned.");
            return;
        }

        startPosition = transform.position;
        targetY = startPosition.y;

        if (audioSource != null)
        {
            audioSource.loop = true;
        }
    }

    private void Update()
    {
        

        if (_isResettingPosition == false)
        {
            // Smooth movement
            Vector3 currentPosition = transform.position;
            float newY = Mathf.Lerp(currentPosition.y, targetY, Time.deltaTime * moveSpeed);
            // Optional: Add drill vibration
            float shakeStrength = 0.001f;
            if (Mathf.Abs(targetY - currentPosition.y) > 0.001f)
            {
                float shakeX = Random.Range(-shakeStrength, shakeStrength);
                float shakeZ = Random.Range(-shakeStrength, shakeStrength);
                transform.position = new Vector3(startPosition.x + shakeX, newY, startPosition.z + shakeZ);
            }
            else
            {
                transform.position = new Vector3(startPosition.x, newY, startPosition.z);
            }
        }
        
        // Start fade if no press recently
        if (audioSource != null && audioSource.isPlaying && !isFadingOut && !isIdling && (Time.time - lastPressTime > stopDelay))
        {
            StartCoroutine(FadeOutDrillThenIdle());
        }
    }

    
    public void DrillStep()
    {
        if (target == null || progress >= 100f) return;
        
        layerTemperature += riseInTemperaturePerClick;

        progress = Mathf.Clamp(progress + drillStepPercent, 0f, 100f);
        float newY = Mathf.Lerp(startPosition.y, target.position.y, progress / 100f);
        targetY = newY;

        // Update last press time
        lastPressTime = Time.time;

        // Cancel fading if user presses again
        if (isFadingOut)
        {
            StopAllCoroutines();
            isFadingOut = false;
        }

        // If idling, stop idle and switch to drilling
        if (isIdling)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = drillLoopSFX;
                audioSource.volume = 1f;
                audioSource.Play();
            }
            isIdling = false;
        }

        // Start playing drill sound if not already
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = drillLoopSFX;
            audioSource.volume = 1f;
            audioSource.Play();
        }
        
        TriggerVibration();
        
    }


    private IEnumerator FadeOutDrillThenIdle()
    {
        isFadingOut = true;

        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.Stop();
        isFadingOut = false;


        if (idleLoopSFX != null)
        {
            audioSource.clip = idleLoopSFX;
            audioSource.volume = 0.5f; // idling is usually quieter
            audioSource.Play();
            isIdling = true;
        }
    }

    public void TriggerVibration()
    {
        _androidVibrator.Vibrate(50, 200);
        Handheld.Vibrate();
    }

    private IEnumerator StartPositionResetForDrillAndEarth()
    {
        _isResettingPosition = true;
        ResetPosition_UIElement.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        
        _spawnDrill.ResetDrillPosition();
        startPosition = transform.position;
        
        yield return new WaitForSeconds(2);
        _isResettingPosition = false;
        ResetPosition_UIElement.SetActive(true);
    }

    public void Method_StartPositionResetForDrillAndEarth()
    {
        StartCoroutine(StartPositionResetForDrillAndEarth());
    }
    
}
