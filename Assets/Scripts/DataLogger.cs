using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
// For memory profiling
using UnityEngine.Profiling;

public class DataLogger : MonoBehaviour
{
    public static DataLogger Instance;

    [Header("Has user completed these scenes?")]
    public bool userCompletedBigBangScene = false;
    public bool userCompletedRoverScene = false;
    public bool userCompletedDiggingScene = false;

    public List<Question> AnsweredUserQuestions = new List<Question>();
    private List<string> AnsweredQuestionSceneNames = new List<string>();

    // Track total time and average FPS per scene
    public Dictionary<string, float> TimeSpentInScenes = new Dictionary<string, float>();
    public Dictionary<string, float> AvgFpsPerScene = new Dictionary<string, float>();

    private string currentSceneName;
    private float startTimeForCurrentScene;
    private float timeSpentInCurrentScene;
    private int frameCountCurrentScene;

    // Track battery usage
    private float initialBatteryLevel;
    private BatteryStatus initialBatteryStatus;

    //public Button EndButton;
    //public CanvasGroup ThankYouText;
    public bool LogSaved = false;

    private void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // record initial battery
        initialBatteryLevel = SystemInfo.batteryLevel;
        initialBatteryStatus = SystemInfo.batteryStatus;

        // Initialize for first scene
        currentSceneName = SceneManager.GetActiveScene().name;
        startTimeForCurrentScene = Time.time;
        frameCountCurrentScene = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // Track time and frames
        timeSpentInCurrentScene = Time.time - startTimeForCurrentScene;
        frameCountCurrentScene++;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Before switching, record the stats for the old scene
        RecordSceneStats();

        // Start tracking for new scene
        currentSceneName = scene.name;
        startTimeForCurrentScene = Time.time;
        frameCountCurrentScene = 0;

        //if (scene.name == "EndScene")
        //    ShowEndButton();
    }

    private void RecordSceneStats()
    {
        // calculate average FPS
        float fps = frameCountCurrentScene > 0
            ? frameCountCurrentScene / timeSpentInCurrentScene
            : 0f;

        // accumulate time
        if (TimeSpentInScenes.ContainsKey(currentSceneName))
            TimeSpentInScenes[currentSceneName] += timeSpentInCurrentScene;
        else
            TimeSpentInScenes[currentSceneName] = timeSpentInCurrentScene;

        // store fps (overwrite or set)
        AvgFpsPerScene[currentSceneName] = fps;

        // reset counters
        timeSpentInCurrentScene = 0f;
        frameCountCurrentScene = 0;
    }

    public void AddQuestionsToAnsweredQuetions(List<Question> questionsToAdd)
    {
        foreach (var q in questionsToAdd)
        {
            AnsweredUserQuestions.Add(q);
            AnsweredQuestionSceneNames.Add(currentSceneName);
        }
    }

    public void SaveLogAtEnd()
    {
        // record final scene stats
        RecordSceneStats();
        SaveLogToFile();
        LogSaved = true;

        // show end UI
        //ShowEndButton();
    }

    //private void ShowEndButton()
    //{
    //    EndButton.gameObject.SetActive(true);
    //}

    private void SaveLogToFile()
    {
        var sb = new StringBuilder();

        sb.AppendLine("DEVICE & PERFORMANCE INFO:");
        sb.AppendLine("===========================");
        sb.AppendLine($"Device Model: {SystemInfo.deviceModel}");
        sb.AppendLine($"Device Name: {SystemInfo.deviceName}");
        sb.AppendLine($"OS: {SystemInfo.operatingSystem}");
        sb.AppendLine($"CPU: {SystemInfo.processorType}");
        sb.AppendLine($"GPU: {SystemInfo.graphicsDeviceName}");
        sb.AppendLine($"Total System RAM: {SystemInfo.systemMemorySize} MB");
        sb.AppendLine($"Graphics RAM: {SystemInfo.graphicsMemorySize} MB");
        sb.AppendLine($"Allocated Memory: {Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)} MB");
        sb.AppendLine($"Reserved Memory: {Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)} MB");
        sb.AppendLine($"Unused Reserved Memory: {Profiler.GetTotalUnusedReservedMemoryLong() / (1024 * 1024)} MB");
        sb.AppendLine($"Target Frame Rate: {Application.targetFrameRate}");

        // battery at start
        sb.AppendLine($"Battery Level at Start: {initialBatteryLevel * 100f:F0}%");
        sb.AppendLine($"Battery Status at Start: {initialBatteryStatus}");

        // battery at end
        float finalBatteryLevel = SystemInfo.batteryLevel;
        sb.AppendLine($"Battery Level at End: {finalBatteryLevel * 100f:F0}%");
        sb.AppendLine($"Battery Status at End: {SystemInfo.batteryStatus}");

        // battery usage
        float usage = (initialBatteryLevel - finalBatteryLevel) * 100f;
        sb.AppendLine($"Battery Usage Over Session: {usage:F0}%");
        sb.AppendLine();

        sb.AppendLine("SCENE STATISTICS:");
        sb.AppendLine("=================");
        float totalTime = 0f;

        foreach (var sceneName in TimeSpentInScenes.Keys.ToList())
        {
            float sceneTime = TimeSpentInScenes[sceneName];
            float sceneFps = AvgFpsPerScene.ContainsKey(sceneName) ? AvgFpsPerScene[sceneName] : 0f;
            int mins = Mathf.FloorToInt(sceneTime / 60f);
            int secs = Mathf.FloorToInt(sceneTime % 60f);
            sb.AppendLine($"Scene: {sceneName}");
            sb.AppendLine($"    Time Spent: {mins}m {secs}s");
            sb.AppendLine($"    Avg FPS: {sceneFps:F1}");
            totalTime += sceneTime;
        }

        int totalMins = Mathf.FloorToInt(totalTime / 60f);
        int totalSecs = Mathf.FloorToInt(totalTime % 60f);
        sb.AppendLine($"Total Session Time: {totalMins}m {totalSecs}s");
        sb.AppendLine();

        sb.AppendLine("USER ANSWERED QUESTIONS:");
        sb.AppendLine("=========================");
        string lastScene = null;
        for (int i = 0; i < AnsweredUserQuestions.Count; i++)
        {
            var q = AnsweredUserQuestions[i];
            var sceneForQ = i < AnsweredQuestionSceneNames.Count
                ? AnsweredQuestionSceneNames[i]
                : "Unknown Scene";
            if (sceneForQ != lastScene)
            {
                sb.AppendLine($"\n--- Scene: {sceneForQ} ---");
                lastScene = sceneForQ;
            }
            sb.AppendLine($"Q: {q.questionText}");
            sb.AppendLine($"A: {q.possibleAnswers[q.userAnswer]}");
            sb.AppendLine($"Correct: {q.possibleAnswers[q.correctAnswerIndex]}");
            sb.AppendLine("----------");
        }

        string timestamp = System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
        string fileName = $"[AR BOOK] Log_{timestamp}.txt";
        foreach (char c in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(c, '_');

        string path = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            File.WriteAllText(path, sb.ToString());
#if UNITY_ANDROID
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var context = activity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    var mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection");
                    mediaScanner.CallStatic("scanFile", context, new string[] { path }, null, null);
                }
                Debug.Log("Log file scanned by Android media scanner");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Media scanner registration failed: " + e.Message);
            }
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save log: " + e.Message);
        }
    }
    //public void AddTimeSpentInScene()
    //{
    //    if (TimeSpentInScenes.ContainsKey(currentSceneName))
    //    {
    //        TimeSpentInScenes[currentSceneName] += timeSpentInCurrentScene;
    //    }
    //    else
    //    {
    //        TimeSpentInScenes.Add(currentSceneName, timeSpentInCurrentScene);
    //    }

    //    startTimeForCurrentScene = Time.time;
    //    timeSpentInCurrentScene = 0;
    //    currentSceneName = SceneManager.GetActiveScene().name;
    //}
}
