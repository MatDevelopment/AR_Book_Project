using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaterRain : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // UI element for keeping up the score
    public int score = 0; // The score variable counting amount of times you have hit the water
    public GameObject raindropPrefab; // Assign the raindrop prefab in the Inspector
    public Vector2 spawnAreaSize = new Vector2(0.5f, 0.5f); // Width & Depth of the spawn area
    public Vector2 rainSizes = new Vector2(0.05f, 0.1f); // Minimum scale and maximum scale of raindrops
    public Transform spawnCenter; // Assign a GameObject to be the center of the spawn area
    public float spawnHeight = 10f; // Height from which rain starts
    public float rainDuration = 10f; // How long the rain lasts
    public float spawnRate = 1f; // How often raindrops are instantiated
    public float fallForce = 0.5f; // Force applied downward

    private bool isRaining = false;

    public void StartRain()
    {
        if (!isRaining)
        {
            StartCoroutine(RainEffect());
        }
    }

    public void ReturnToMainMenu()
    {
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.LoadMainMenu();
        }
        else
        {
            Debug.LogWarning("MainMenuManager instance not found!");
        }
    }

    private IEnumerator RainEffect()
    {
        isRaining = true;
        float timer = 0f;

        while (timer < rainDuration)
        {
            SpawnRaindrop();
            timer += spawnRate;
            yield return new WaitForSeconds(spawnRate);
        }

        isRaining = false;
    }

    private void SpawnRaindrop()
    {
        // Generate a random spawn position within the defined area
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        Vector3 spawnPosition = new Vector3(spawnCenter.position.x + randomX, spawnCenter.position.y + spawnHeight, spawnCenter.position.z + randomZ);

        // Instantiate the raindrop
        GameObject raindrop = Instantiate(raindropPrefab, spawnPosition, Quaternion.identity);

        float raindropRatio = Random.Range(rainSizes.x, rainSizes.y);
        Vector3 raindropSize = new Vector3(raindropRatio, raindropRatio, raindropRatio);

        raindrop.transform.localScale = raindropSize;

        // Apply a downward force if it has a Rigidbody
        Rigidbody rb = raindrop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.down * fallForce, ForceMode.Impulse);
        }
    }
}
