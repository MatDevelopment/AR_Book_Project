using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    // Average lowest 1% framerate
    // Counter for when model target tracking
    private int modelTrackingCount = 0;
    private List<float> trackingTimestamps = new List<float>();

    // Frame Stutter Detection
    [Header("Frame Stutters")]
    public float stutterThreshold = 15f;

    private int stutterCount = 0;
    private List<float> stutterTimestamps = new List<float>();

    [Header("UI Elements")]
    [SerializeField] private string currentCondition = "Condition: ";
    [SerializeField] private TextMeshProUGUI currentConditionText;
    [SerializeField] private TextMeshProUGUI currentConditionTextVertical;
    [SerializeField] private TextMeshProUGUI logLocationText;
    [SerializeField] private TextMeshProUGUI logLocationTextVertical;
    [SerializeField] private GameObject SUIStartScreen;
    [SerializeField] private GameObject SUIStartScreenVertical;
    [SerializeField] private GameObject SUIStopScreen;
    [SerializeField] private GameObject SUIStopScreenVertical;

    [SerializeField] private RocketControl rocketControl;

    void Start()
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        filePath = Path.Combine(Application.persistentDataPath, $"{currentCondition}_session_{timestamp}.txt");

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Convert.ToInt32(Math.Round(Screen.currentResolution.refreshRateRatio.value));

        if (rocketControl.tangibleRotation)
        {
            SUIStartScreen = SUIStartScreenVertical;
            SUIStopScreen = SUIStopScreenVertical;
            currentConditionText = currentConditionTextVertical;
            logLocationText = logLocationTextVertical;
        }

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
            float currentSessionTime = totalSessionTime;
            stutterTimestamps.Add(currentSessionTime);
        }
    }

    public void CountModelTracking()
    {
        modelTrackingCount++;
        float currentSessionTime = totalSessionTime;
        trackingTimestamps.Add(currentSessionTime);
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

        // 1% low FPS calculation
        // Sort FPS values (slowest to fastest)
        var fpsValues = samples.Select(s => s.fps).OrderBy(fps => fps).ToList();

        int onePercentCount = Mathf.Max(1, Mathf.FloorToInt(fpsValues.Count * 0.1f));
        float onePercentLowAvg = fpsValues.Take(onePercentCount).Average();

        int count = samples.Count;
        avgFps /= count;
        avgAlloc /= count;
        avgReserved /= count;
        avgMono /= count;

        string result = $"Session Summary ({currentCondition}) ({System.DateTime.Now}):\n" +
                        $"Total Session Time: {totalSessionTime}\n" +
                        $"Samples: {count}\n" +
                        $"Average FPS: {avgFps:F1}, Min: {minFps:F1}, Max: {maxFps:F1}, 10%LowAvg: {onePercentLowAvg:F1}\n" +
                        $"Stutters Detected (< {stutterThreshold} FPS): {stutterCount}\n" +
                        $"Tracking Detected: {modelTrackingCount}\n" +
                        $"Average Allocated Memory (MB): {avgAlloc:F2}\n" +
                        $"Average Reserved Memory (MB): {avgReserved:F2}\n" +
                        $"Average Mono Memory (MB): {avgMono:F2}\n\n";

        if (stutterTimestamps.Count > 0)
        {
            result += "Stutter Times (sec): " + string.Join(", ", stutterTimestamps.ConvertAll(t => t.ToString("F1"))) + "\n";
        }

        if (trackingTimestamps.Count > 0)
        {
            result += "Tracking Times (sec): " + string.Join(", ", trackingTimestamps.ConvertAll(t => t.ToString("F1"))) + "\n";
        }

        File.AppendAllText(filePath, result);
        Debug.Log("Performance session ended. Summary saved to file:\n" + filePath);

        logLocationText.text = "Session gemt som log fil:\n" + filePath;
        SUIStopScreen.SetActive(true);
    }
}
