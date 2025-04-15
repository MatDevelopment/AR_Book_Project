using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Artifact
{
    // Based on the finding of the mars rovers:
    // https://science.nasa.gov/mission/mars-2020-perseverance/mars-rock-samples/
    // https://science.nasa.gov/mission/msl-curiosity/science-highlights/
    // https://science.nasa.gov/missions/mars-science-laboratory/nasas-curiosity-rover-detects-largest-organic-molecules-found-on-mars/
    public string name;
    public string description;
    public string chemicalProperties;
    public string biosignature;
    public string geographicProperties;
}

[System.Serializable]
public class ArtifactHint
{
    public string artifactName;
    public string hintText;
}

public class RoverRockCrushing : MonoBehaviour
{
    // Idea for cracking the stones open:
    // 3D View: https://www.youtube.com/watch?v=qXQxBwTujnA or https://www.youtube.com/watch?v=8yzpjkoE0YA

    [SerializeField]
    private GameObject rockUI;
    [SerializeField]
    private GameObject splitRockPrefab;
    private GameObject splitRock;

    [SerializeField]
    private int rockHealth = 3;
    private int startingHealth;

    [SerializeField]
    private GameObject finishButton;

    [Header("Artifacts")]
    public List<Artifact> artifacts = new List<Artifact>();
    public ArtifactHint[] artifactHints = new ArtifactHint[3];
    public TextMeshProUGUI[] hintTexts = new TextMeshProUGUI[3];
    private int currentHint = 0;
    private bool artifactGiven = false;
    private List<Artifact> foundArtifacts = new List<Artifact>();

    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI chemicalPropertiesText;
    public TextMeshProUGUI biosignatureText;
    public TextMeshProUGUI geographicPropertiesText;

    // Shaking
    [Header("Shaking")]
    [SerializeField]
    private ParticleSystem rockParticles;
    [SerializeField]
    private ParticleSystem dustParticles;
    [SerializeField]
    private float shakeDuration = 0.5f;
    [SerializeField]
    private float shakeMagnitude = 0.1f;
    private Vector3 originalPosition;
    private bool isShaking = false;

    // Audio
    private SoundEffectManager rockAudioManager;

    void Start()
    {
        originalPosition = rockUI.transform.localPosition;
        startingHealth = rockHealth;

        rockAudioManager = GetComponent<SoundEffectManager>();
    }

    public void RockClick()
    {
        // If pressing before shaking is over
        if (isShaking)
        {
            StopCoroutine(RockShake());
            rockParticles.Stop();
            dustParticles.Stop();
            rockUI.transform.localPosition = originalPosition;
            isShaking = false;
        }
        
        // Hitting the rock
        if (rockHealth > 0 && !isShaking && !artifactGiven)
        {
            StartCoroutine(RockShake());

            rockParticles.Play();
            dustParticles.Play();
        }
    }

    IEnumerator RockShake()
    {
        isShaking = true;
        float elapsed = 0f;

        rockAudioManager.PlaySingleSound("rock_hit");

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
            rockAudioManager.PlaySingleSound("rock_crush");

            splitRock = Instantiate(splitRockPrefab, rockUI.transform.position, Quaternion.identity);
            rockUI.SetActive(false);
            GiveArtifact();
        }

        rockUI.transform.localPosition = originalPosition;
        isShaking = false;
    }

    private void GiveArtifact()
    {
        if (artifacts.Count > 0)
        {
            // Select artifact
            int index = Random.Range(0, artifacts.Count);
            Artifact chosenArtifact = artifacts[index];

            // Add to list of found artifacts
            foundArtifacts.Add(chosenArtifact);

            // Activate Hints
            foreach (ArtifactHint hint in artifactHints)
            {
                if (hint.artifactName == chosenArtifact.name && currentHint < hintTexts.Length)
                {
                    hintTexts[currentHint].text = hint.hintText;
                    currentHint++;
                }
            }

            // Update UI
            StopAllCoroutines();
            StartCoroutine(TypingSequence(1f,
                $"{foundArtifacts.Count} {chosenArtifact.name}",
                chosenArtifact.chemicalProperties,
                chosenArtifact.biosignature,
                chosenArtifact.geographicProperties,
                chosenArtifact.description));

            // Remove from artifact list
            artifacts.Remove(chosenArtifact);
        } else {
            // Update UI
            nameText.text = $"Normal Sten";
            chemicalPropertiesText.text = $"Små Mineraler";
            biosignatureText.text = $"Intet tegn på liv";
            geographicPropertiesText.text = $"Findes overalt";
            descriptionText.text = $"Missionen er fuldført, alle tegn på liv er fundet";
        }

        artifactGiven = true;

        finishButton.SetActive(true);

    }

    public void ResetRockCrushing()
    {
        rockHealth = startingHealth;
        Destroy(splitRock);
        rockUI.SetActive(true);
        finishButton.SetActive(false);

        // Remove Previous Text
        nameText.text = "";
        chemicalPropertiesText.text = "";
        biosignatureText.text = "";
        geographicPropertiesText.text = "";
        descriptionText.text = "";

        // Write new text
        StopAllCoroutines();
        StartCoroutine(TypingSequence(1f,
            "Find ud af stenens indhold",
            "Kemiske Indhold",
            "Biosignatur",
            "Geografisk Placering",
            "Beskrivelse"));

        artifactGiven = false;
    }

    // Typewriting effect function
    private IEnumerator TypeText(string fullText, TextMeshProUGUI textElement)
    {
        float typingSpeed = 0.05f;
        textElement.text = "";
        foreach (char letter in fullText)
        {
            textElement.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator TypingSequence(float speed, string name, string chemical, string biosignature, string geographic, string description)
    {
        rockAudioManager.PlaySound("typing");
        
        StartCoroutine(TypeText(name, nameText));
        yield return new WaitForSeconds(speed * 1.5f);
        StartCoroutine(TypeText(chemical, chemicalPropertiesText));
        yield return new WaitForSeconds(speed);
        StartCoroutine(TypeText(biosignature, biosignatureText));
        yield return new WaitForSeconds(speed);
        StartCoroutine(TypeText(geographic, geographicPropertiesText));
        yield return new WaitForSeconds(speed);
        StartCoroutine(TypeText(description, descriptionText));
        yield return new WaitForSeconds(speed);

        rockAudioManager.PauseSound();
    }
}
