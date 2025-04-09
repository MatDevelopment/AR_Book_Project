using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Artifact
{
    public string name;
    public string description;
}

public class RoverRockCrushing : MonoBehaviour
{
    // Idea for cracking the stones open:
    // 3D View: https://www.youtube.com/watch?v=qXQxBwTujnA or https://www.youtube.com/watch?v=8yzpjkoE0YA

    [SerializeField]
    private GameObject rockUI;

    [SerializeField]
    private int rockHealth = 3;
    private int startingHealth;

    [Header("Artifacts")]
    public Artifact[] artifacts = new Artifact[3];
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    private bool artifactGiven = false;

    // Shaking
    [Header("Shaking")]
    [SerializeField]
    private ParticleSystem rockParticles;
    [SerializeField]
    private float shakeDuration = 0.5f;
    [SerializeField]
    private float shakeMagnitude = 0.1f;
    private Vector3 originalPosition;
    private bool isShaking = false;

    void Start()
    {
        originalPosition = rockUI.transform.localPosition;
        startingHealth = rockHealth;
    }

    public void RockClick()
    {
        // If pressing before shaking is over
        if (isShaking)
        {
            StopCoroutine(RockShake());
            rockParticles.Stop();
            rockUI.transform.localPosition = originalPosition;
            isShaking = false;
        }
        
        // Hitting the rock
        if (rockHealth > 0 && !isShaking && !artifactGiven)
        {
            StartCoroutine(RockShake());

            rockParticles.Play();
        }
    }

    IEnumerator RockShake()
    {
        isShaking = true;
        float elapsed = 0f;

        // Defining the shake amount based on the magnitude given and the health left
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude * ((startingHealth + 1) - rockHealth);
            float y = Random.Range(-1f, 1f) * shakeMagnitude * ((startingHealth + 1) - rockHealth);

            rockUI.transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reducing the health
        rockHealth--;
        Debug.Log($"Rock Health: {rockHealth}");

        // Condition to remove rock
        if (rockHealth <= 0)
        {
            rockUI.SetActive(false);
            GiveArtifact();
        }

        rockUI.transform.localPosition = originalPosition;
        isShaking = false;
    }

    private void GiveArtifact()
    {
        int index = Random.Range(0, artifacts.Length);
        Artifact chosenArtifact = artifacts[index];

        nameText.text = $"Du Fandt: {chosenArtifact.name}!";
        descriptionText.text = $"{chosenArtifact.description}";
        artifactGiven = true;
    }

    public void ResetRockCrushing()
    {
        rockHealth = startingHealth;
        rockUI.SetActive(true);

        nameText.text = "";
        descriptionText.text = "";

        artifactGiven = false;
    }
}
