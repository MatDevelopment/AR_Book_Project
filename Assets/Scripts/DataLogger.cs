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

public class DataLogger : MonoBehaviour
{
    public static DataLogger Instance;
    [Header("Has user completed these scenes?")]
    public bool userCompletedBigBangScene = false;
    public bool userCompletedRoverScene = false;
    public bool userCompletedDiggingScene = false;

    public List<Question> AnsweredUserQuestions = new List<Question>();
    private List<string> AnsweredQuestionSceneNames = new List<string>(); // <-- New list to track scenes for questions
    public Dictionary<string, float> TimeSpentInScenes = new Dictionary<string, float>();

    public float timeSpentInCurrentScene = 0;
    public float startTimeForCurrentScene = 0;
    private string currentSceneName;

    public Button EndButton;
    public CanvasGroup ThankYouText;

    public bool LogSaved = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "EndScene")
        {
            ShowEndButton();
        }
        startTimeForCurrentScene = Time.time;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private void ShowEndButton()
    {
        EndButton.gameObject.SetActive(true);
    }

    private void Update()
    {
        timeSpentInCurrentScene = Time.time - startTimeForCurrentScene;
    }

    public void AddQuestionsToAnsweredQuetions(List<Question> questionsToAdd)
    {
        AnsweredUserQuestions.AddRange(questionsToAdd);

        foreach (var q in questionsToAdd)
        {
            AnsweredQuestionSceneNames.Add(currentSceneName); // Track scene per question
        }
    }

    public void AddTimeSpentInScene()
    {
        if (TimeSpentInScenes.ContainsKey(currentSceneName))
        {
            TimeSpentInScenes[currentSceneName] += timeSpentInCurrentScene;
        }
        else
        {
            TimeSpentInScenes.Add(currentSceneName, timeSpentInCurrentScene);
        }

        startTimeForCurrentScene = Time.time;
        timeSpentInCurrentScene = 0;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddTimeSpentInScene();
        currentSceneName = scene.name;
        startTimeForCurrentScene = Time.time;

        if (scene.name == "EndScene")
        {
            ShowEndButton();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [System.Serializable]
    public class SceneTimeInfo
    {
        public string sceneName;
        public float timeSpent;
    }

    private void SaveLog(string addedTextAtStart)
    {
        float currentTime = timeSpentInCurrentScene;
        AddTimeSpentInScene();

        StringBuilder textContent = new StringBuilder();
        textContent.AppendLine(addedTextAtStart);
        textContent.AppendLine("====================");
        textContent.AppendLine("DEVICE & PERFORMANCE INFO:");
        textContent.AppendLine("====================");
        textContent.AppendLine($"Device Model: {SystemInfo.deviceModel}");
        textContent.AppendLine($"Device Name: {SystemInfo.deviceName}");
        textContent.AppendLine($"Operating System: {SystemInfo.operatingSystem}");
        textContent.AppendLine($"Processor Type: {SystemInfo.processorType}");
        textContent.AppendLine($"Graphics Device Name: {SystemInfo.graphicsDeviceName}");
        textContent.AppendLine($"System Memory Size: {SystemInfo.systemMemorySize} MB");
        textContent.AppendLine($"Graphics Memory Size: {SystemInfo.graphicsMemorySize} MB");
        textContent.AppendLine($"Target Frame Rate: {Application.targetFrameRate}");
        textContent.AppendLine($"Approximate FPS: {(1.0f / Time.deltaTime):F2}");
        textContent.AppendLine();

        textContent.AppendLine("====================");
        textContent.AppendLine("SCENES TIME INFORMATION:");
        textContent.AppendLine("====================");

        float totalTime = 0f;
        foreach (var kvp in TimeSpentInScenes)
        {
            textContent.AppendLine($"Scene: {kvp.Key}, Time: {kvp.Value:F2} seconds");
            totalTime += kvp.Value;
        }

        textContent.AppendLine($"Total Time: {totalTime:F2} seconds");
        textContent.AppendLine();
        textContent.AppendLine("====================");
        textContent.AppendLine("USER ANSWERED QUESTIONS:");
        textContent.AppendLine("====================");

        string lastSceneName = "";
        for (int i = 0; i < AnsweredUserQuestions.Count; i++)
        {
            string sceneNameForQuestion = (i < AnsweredQuestionSceneNames.Count) ? AnsweredQuestionSceneNames[i] : "Unknown Scene";

            if (sceneNameForQuestion != lastSceneName)
            {
                textContent.AppendLine($"\n--- Scene: {sceneNameForQuestion} ---\n");
                lastSceneName = sceneNameForQuestion;
            }

            var question = AnsweredUserQuestions[i];
            textContent.AppendLine($"Question: {question.questionText}");
            textContent.AppendLine($"Answer: {question.possibleAnswers[question.userAnswer]}");
            textContent.AppendLine($"Correct: {question.possibleAnswers[question.correctAnswerIndex]}");
            textContent.AppendLine("----------");
        }

        string dateTimeFormat = System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
        string fileName = "[AR BOOK] Log_ " + dateTimeFormat + ".txt";

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }

        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            System.IO.File.WriteAllText(filePath, textContent.ToString());

#if UNITY_ANDROID
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    AndroidJavaClass mediaScannerClass = new AndroidJavaClass("android.media.MediaScannerConnection");
                    mediaScannerClass.CallStatic("scanFile", context, new string[] { filePath }, null, null);
                }
                Debug.Log("File registered with Android media scanner");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Could not register file with Android media scanner: " + e.Message);
            }
#endif 
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save log file: " + e.Message);
        }

        timeSpentInCurrentScene = currentTime;
    }

    public void SaveLogAtEnd()
    {
        ExtensionMethods.FadeCanvasGroup(EndButton.GetComponent<CanvasGroup>(), false, 0.5f);
        ThankYouText.gameObject.SetActive(true);
        ExtensionMethods.FadeCanvasGroup(ThankYouText, true, 0.6f);

        AddTimeSpentInScene();
        SaveLog("Saved on application Quit!");
    }
}
