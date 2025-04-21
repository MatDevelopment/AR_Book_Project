using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SUIManager : MonoBehaviour
{
    private class MetricsSnapshot
    {
        public float fps;
        public float allocatedMB;
        public float reservedMB;
        public float monoMB;

        public MetricsSnapshot(float fps, float alloc, float reserved, float mono)
        {
            this.fps = fps;
            this.allocatedMB = alloc;
            this.reservedMB = reserved;
            this.monoMB = mono;
        }
    }

    private bool isLogging = false;
    private float logInterval = 1f;
    private float timer = 0f;
    private float totalSessionTime = 0f;
    private List<MetricsSnapshot> samples = new List<MetricsSnapshot>();

    private string filePath;

    // Frame Stutter Detection
    [Header("Frame Stutters")]
    public float stutterThreshold = 15f;

    private int stutterCount = 0;

    [Header("UI Elements")]
    [SerializeField] private string currentCondition = "Condition: ";
    [SerializeField] private TextMeshProUGUI currentConditionText;
    [SerializeField] private TextMeshProUGUI logLocationText;
    [SerializeField] private GameObject SUIStartScreen;
    [SerializeField] private GameObject SUIStopScreen;

    void Start()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        filePath = Path.Combine(Application.persistentDataPath, $"{currentCondition}_session_{timestamp}.txt");

        currentConditionText.text = currentCondition;

        SUIStartScreen.SetActive(true);
        SUIStopScreen.SetActive(false);
    }

    void Update()
    {
        if (!isLogging) return;

        timer += Time.deltaTime;
        totalSessionTime += Time.deltaTime;
        if (timer >= logInterval)
        {
            timer = 0f;
            LogSnapshot();
        }
    }

    private void LogSnapshot()
    {
        float fps = 1.0f / Time.deltaTime;
        float allocated = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
        float reserved = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
        float mono = System.GC.GetTotalMemory(false) / (1024f * 1024f);

        samples.Add(new MetricsSnapshot(fps, allocated, reserved, mono));

        // Detect stutters
        if (fps < stutterThreshold)
        {
            stutterCount++;
            // stutterTimestamps.Add(Time.time);
        }
    }

    public void StartSession()
    {
        samples.Clear();
        isLogging = true;
        timer = 0f;
        totalSessionTime = 0f;
        Debug.Log("Performance session started.");

        SUIStartScreen.SetActive(false);
    }

    public void StopSession()
    {
        isLogging = false;
        if (samples.Count == 0)
        {
            Debug.LogWarning("No data was collected during the session.");
            return;
        }

        float avgFps = 0f;
        float avgAlloc = 0f;
        float avgReserved = 0f;
        float avgMono = 0f;

        float minFps = float.MaxValue;
        float maxFps = float.MinValue;

        foreach (var snap in samples)
        {
            avgFps += snap.fps;
            avgAlloc += snap.allocatedMB;
            avgReserved += snap.reservedMB;
            avgMono += snap.monoMB;

            minFps = Mathf.Min(minFps, snap.fps);
            maxFps = Mathf.Max(maxFps, snap.fps);
        }

        int count = samples.Count;
        avgFps /= count;
        avgAlloc /= count;
        avgReserved /= count;
        avgMono /= count;

        string result = $"Session Summary ({System.DateTime.Now}):\n" +
                        $"Total Session Time: {totalSessionTime}\n" +
                        $"Samples: {count}\n" +
                        $"Average FPS: {avgFps:F1}, Min: {minFps:F1}, Max: {maxFps:F1}\n" +
                        $"Stutters Detected (< {stutterThreshold} FPS): {stutterCount}\n" +
                        $"Average Allocated Memory (MB): {avgAlloc:F2}\n" +
                        $"Average Reserved Memory (MB): {avgReserved:F2}\n" +
                        $"Average Mono Memory (MB): {avgMono:F2}\n\n";

        File.AppendAllText(filePath, result);
        Debug.Log("Performance session ended. Summary saved to file:\n" + filePath);

        logLocationText.text = "Session gemt som log fil:\n" + filePath;
        SUIStopScreen.SetActive(true);
    }
}
