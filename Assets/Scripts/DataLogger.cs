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
    public Dictionary<string, float> TimeSpentInScenes = new Dictionary<string, float>();

    public float timeSpentInCurrentScene = 0;
    public float startTimeForCurrentScene = 0;
    private string currentSceneName;

    public Button EndButton;
    public CanvasGroup ThankYouText;

    public bool LogSaved = false;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive between scenes
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
    }

    public void AddTimeSpentInScene() // Should be called whenever a scene is unloaded
    {
        if (TimeSpentInScenes.ContainsKey(currentSceneName))
        {
            TimeSpentInScenes[currentSceneName] += timeSpentInCurrentScene;
        }
        else
        {
            TimeSpentInScenes.Add(currentSceneName, timeSpentInCurrentScene);
        }

        // Reset the timer for the next scene
        startTimeForCurrentScene = Time.time;
        timeSpentInCurrentScene = 0;
        // Update the current scene name
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    // Called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Record time spent in previous scene
        AddTimeSpentInScene();
        // Update current scene name
        currentSceneName = scene.name;
        // Reset timer for new scene
        startTimeForCurrentScene = Time.time;

        if (scene.name == "EndScene")
        {
            ShowEndButton();
        }
    }
    

    private void OnEnable()
    {
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //private void OnApplicationPause(bool pauseStatus)
    //{
    //    if (pauseStatus) // If the application is being paused
    //    {
    //        AddTimeSpentInScene(); // Add the scene time
    //        SaveLog("Saved on application pause!");
    //    }
    //}

    [System.Serializable]
    public class SceneTimeInfo
    {
        public string sceneName;
        public float timeSpent;
    }

    private void SaveLog(string addedTextAtStart)
    {
        // First add the current scene's time
        float currentTime = timeSpentInCurrentScene;
        AddTimeSpentInScene();

        // Create a formatted text content
        StringBuilder textContent = new StringBuilder();
        textContent.AppendLine(addedTextAtStart);
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

        foreach (var question in AnsweredUserQuestions)
        {
            textContent.AppendLine($"Question: {question.questionText}");
            textContent.AppendLine($"Answer: {question.possibleAnswers[question.userAnswer]}");
            textContent.AppendLine($"Correct: {question.possibleAnswers[question.correctAnswerIndex]}");
            textContent.AppendLine("----------");
        }

        // Create a file path with new date format - dd/MM/yyyy/HH:mm
        string dateTimeFormat = System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
        string fileName = "[AR BOOK] Log_ " + dateTimeFormat + ".txt";

        // Replace any invalid filename characters with underscores
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }

        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            // Write the text content to a file
            System.IO.File.WriteAllText(filePath, textContent.ToString());

            // For Android: Make sure the file is visible to other apps like file managers
#if UNITY_ANDROID
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    // Notify the system that a new file was created
                    AndroidJavaClass mediaScannerClass = new AndroidJavaClass("android.media.MediaScannerConnection");
                    mediaScannerClass.CallStatic("scanFile", context, new string[] { filePath }, null, null);
                }
                Debug.Log("File registered with Android media scanner");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Could not register file with Android media scanner: " + e.Message);
                // Non-critical error, so we continue
            }
#endif 
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save log file: " + e.Message);
        }

        // Restore the current scene's time that was reset in AddTimeSpentInScene
        timeSpentInCurrentScene = currentTime;
    }

    //private void OnApplicationQuit()
    //{
    //    QuitGame();
    //}

    public void SaveLogAtEnd()
    {
        ExtensionMethods.FadeCanvasGroup(EndButton.GetComponent<CanvasGroup>(), false, 0.5f); // Fade out the thank you text
        ThankYouText.gameObject.SetActive(true);
        ExtensionMethods.FadeCanvasGroup(ThankYouText, true, 0.6f); // Fade out the thank you text

        AddTimeSpentInScene(); // Add the final scene time
        SaveLog("Saved on application Quit!");
    }
}
//#if UNITY_ANDROID
//        // First minimize the app
//        MoveAndroidApplicationToBack();

//        // Then use more forceful method to quit on Android
//        try
//        {
//            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
//            activity.Call("finishAndRemoveTask");

//            // As a fallback, also try to kill the process directly
//            AndroidJavaObject process = new AndroidJavaClass("android.os.Process");
//            int pid = process.CallStatic<int>("myPid");
//            process.CallStatic("killProcess", pid);
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError("Failed to exit Android app: " + e.Message);
//            // Fallback to standard quit
//            Application.Quit(0);
//        }
//#else
//        Application.Quit();
//#endif
//    }

//    public static void MoveAndroidApplicationToBack()
//    {
//#if UNITY_ANDROID
//        try
//        {
//            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
//            activity.Call<bool>("moveTaskToBack", true);
//            Debug.Log("Application minimized successfully");
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError("Failed to minimize application: " + e.Message);
//        }
//#endif
//    }
