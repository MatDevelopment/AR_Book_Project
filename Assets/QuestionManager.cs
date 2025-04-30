using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [SerializeField]
    public List<Question> questions = new List<Question>();

    public bool AllQuestionsAnswered = false, EndSessionFinished = false, ShowDebugFinishButton = true, isUIHorizontal = false;
    public Transform QuestionsParent;
    public GameObject questionBoxPrefab;
    public GameObject togglePrefab;
    public TextMeshProUGUI AnswerAllQuestionText, afterQuizUserAnswerText, afterQuizCorrectAnswerText, afterQuizQuestionText, staticUserAnswer, staticCorrectAnswer;
    public Button OpenQuestionsPanelButton, debugfinishButton;
    public CanvasGroup afterQuizContainerGroup, IconCorrect, IconIncorrect, CG_NextExerciseButton, CG_AfterQuizSection, CG_IconQuestionmark, CG_IconArrow;

    [SerializeField]
    private RectTransform questionsPanel;

    [SerializeField]
    private bool panelHidden = true;


    // Animation parameters
    private float buttonScaleDuration = 0.3f;
    private float buttonPingScale = 1.2f;
    private float panelAnimationDuration = 0.7f; // Reduced from 1.0f to make it faster

    public SoundEffectManager SoundEffectManager;
    public bool hasStartedAfterQuizSession = false;
    void Start()
    {
        SoundEffectManager= GetComponentInChildren<SoundEffectManager>();
        if (debugfinishButton)
            debugfinishButton.gameObject.SetActive(true);

        for (int i = 0; i < questions.Count; i++)
        {
            GameObject questionBox = Instantiate(questionBoxPrefab, QuestionsParent);
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = questions[i].questionText;
            ToggleGroup toggleGroup = questionBox.GetComponentInChildren<ToggleGroup>();
            List<Toggle> toggles = new List<Toggle>();
            for (int j = 0; j < questions[i].possibleAnswers.Count; j++)
            {
                GameObject toggle = Instantiate(togglePrefab, questionBox.GetComponentInChildren<VerticalLayoutGroup>().transform);
                toggle.GetComponent<Toggle>().group = toggleGroup;
                toggle.GetComponentInChildren<TextMeshProUGUI>().text = questions[i].possibleAnswers[j];
                Toggle toggleComponent = toggle.GetComponent<Toggle>();
                toggle.GetComponent<ToggleAnswer>().parentQuestion = questions[i];
                toggle.GetComponent<ToggleAnswer>().answerIndex = j;
                toggle.GetComponent<ToggleAnswer>().parentToggleGroup = questionBox.GetComponentInChildren<ToggleGroup>();
                toggles.Add(toggleComponent);
            }
        }

        SetClearTextColorAllAfterQuizText();
        //HideAfterQuizSection();
        HideYouNeedToAnswerAllQuestionsText();
        //HideCorrectIncorrectIcons();
        //HideNextExerciseButton();
        HideAllUIWithoutCoroutines();
    }
    private void HideAllUIWithoutCoroutines()
    {
        CG_AfterQuizSection.alpha = 0;
        IconCorrect.alpha = 0;
        IconIncorrect.alpha = 0;
        CG_NextExerciseButton.alpha = 0;
    }
    public void CheckForAllQuestionsAnswered()
    {

        if (AllQuestionsAnswered)
        {
            HideQuestionsPanel();
            panelHidden = true;
            ShowAfterQuizSection();
            StartAfterquizSession();
        }
    }
    private void FixedUpdate()
    {
        foreach (Question question in questions)
        {
            if (question.userAnswer == -1)
            {
                AllQuestionsAnswered = false;
                ShowYouNeedToAnswerAllQuestionsText();
                break;
            }
            else
            {
                AllQuestionsAnswered = true;
            }
        }
    }

    public void StartAfterquizSession()
    {
        // Fade in the afterQuizContainerGroup
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(afterQuizContainerGroup, true, 1f));
        StartCoroutine(ShowQuestionsSequentially());
        hasStartedAfterQuizSession = true;
    }

    private IEnumerator ShowQuestionsSequentially()
    {
        yield return new WaitForSeconds(0.4f); // Delay before starting

        foreach (Question question in questions)
        {
            yield return StartCoroutine(ShowQuestionInformation(question));
            yield return new WaitForSeconds(1f); // Delay between clearing 
            SetClearTextColorAllAfterQuizText();
            HideCorrectIncorrectIcons();
            yield return new WaitForSeconds(1f); // Delay between showing next question
        }

        // Fade out the afterQuizContainerGroup when all questions are shown
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(afterQuizContainerGroup, false, 1f));

        yield return new WaitForSeconds(1f);
        EndSessionFinished = true;
        ShowNextExerciseButton();
    }

    public void ShowNextExerciseButton()
    {
        CG_NextExerciseButton.gameObject.SetActive(true);
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(CG_NextExerciseButton, true, 1f));
    }

    public void HideNextExerciseButton()
    {
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(CG_NextExerciseButton, false, 0.5f));
        StartCoroutine(DisableGameObjectAfterFade(CG_NextExerciseButton.gameObject, 0.5f));
    }

    private IEnumerator DisableGameObjectAfterFade(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        if (EndSessionFinished)
        {
            SetCompletedSceneBoolTrue();
            DataLogger.Instance.AddQuestionsToAnsweredQuetions(questions);
            //DataLogger.Instance.AddTimeSpentInScene();
         

            if (DataLogger.Instance.userCompletedBigBangScene && DataLogger.Instance.userCompletedRoverScene && DataLogger.Instance.userCompletedDiggingScene)
            {
                DataLogger.Instance.SaveLogAtEnd();
         
                StartCoroutine(LoadSceneWithTransition("EndScene"));
            }
            else
                StartCoroutine(LoadSceneWithTransition(sceneName));
        }
    }

    public void SetCompletedSceneBoolTrue()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "BigBang":
                DataLogger.Instance.userCompletedBigBangScene = true;
                break;
            case "RoverScene":
                DataLogger.Instance.userCompletedRoverScene = true;
                break;
            case "DiggingMechanic":
                DataLogger.Instance.userCompletedDiggingScene = true;
                break;
        }
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        // Optional: Add a fade-out effect here before loading the scene
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ShowQuestionInformation(Question question)
    {
        HideCorrectIncorrectIcons();
        SetTextToNothing();

        // Wait before showing question details
        yield return new WaitForSeconds(1);
        StartCoroutine(SetTextColorAfterDelay(afterQuizQuestionText, 0));
        afterQuizQuestionText.text = question.questionText;
        Debug.Log("answer: " + question.possibleAnswers[question.userAnswer]);

        // Show user answer
        StartCoroutine(SetTextColorAfterDelay(staticUserAnswer, 0));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SetTextColorAfterDelay(afterQuizUserAnswerText, 0));
        yield return new WaitForSeconds(0.8f);
        afterQuizUserAnswerText.text = question.possibleAnswers[question.userAnswer];

        yield return new WaitForSeconds(0.8f);

        // Show correct answer

        StartCoroutine(SetTextColorAfterDelay(staticCorrectAnswer, 0));
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(SetTextColorAfterDelay(afterQuizCorrectAnswerText, 0));
        yield return new WaitForSeconds(0.8f);
        afterQuizCorrectAnswerText.text = question.possibleAnswers[question.correctAnswerIndex];

        yield return new WaitForSeconds(1.5f);
        // Smoothly show correct/incorrect icons
        if (question.userAnswer == question.correctAnswerIndex)
        {
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(IconCorrect, true, 0.5f));
            SoundEffectManager.PlaySingleSound("CorrectAnswer");
        }
        else
        {
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(IconIncorrect, true, 0.5f));
            SoundEffectManager.PlaySingleSound("WrongAnswer");
        }
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator SetTextColorAfterDelay(TextMeshProUGUI textToShow, float delay)
    {
        SoundEffectManager.PlaySingleSound("Beep");
        yield return new WaitForSeconds(delay);

        // Smoothly fade in the text
        Color startColor = textToShow.color;
        Color targetColor = Color.black;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textToShow.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            yield return null;
        }
        textToShow.color = targetColor;
    }

    private void HideCorrectIncorrectIcons()
    {
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(IconCorrect, false, 0.0f));
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(IconIncorrect, false, 0.0f));
    }

    private void ShowCorrectIncorrectIcons(bool isCorrect)
    {
        if (isCorrect)
        {
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(IconCorrect, true, 0.5f));
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(IconIncorrect, false, 0.5f));
            SoundEffectManager.PlaySingleSound("CorrectAnswer");
        }
        else
        {
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(IconCorrect, false, 0.5f));
            StartCoroutine(ExtensionMethods.FadeCanvasGroup(IconIncorrect, true, 0.5f));
            SoundEffectManager.PlaySingleSound("WrongAnswer");
        }
    }

    private void SetClearTextColorAllAfterQuizText()
    {
        afterQuizUserAnswerText.color = new Color (1,1,1,0);
        afterQuizCorrectAnswerText.color = new Color(1, 1, 1, 0);
        afterQuizQuestionText.color = new Color(1, 1, 1, 0);
        staticUserAnswer.color = new Color(1, 1, 1, 0);
        staticCorrectAnswer.color = new Color(1, 1, 1, 0);
    }

    public void ShowYouNeedToAnswerAllQuestionsText()
    {
        AnswerAllQuestionText.gameObject.SetActive(true);
        StartCoroutine(ExtensionMethods.FadeTextIn(AnswerAllQuestionText, 0.3f));
        StartCoroutine(PingOpenQuestionPanelButton());
        StartCoroutine(HideTextAfterDelay(AnswerAllQuestionText, 3f));
    }

    private IEnumerator HideTextAfterDelay(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Fade out the text
        float elapsed = 0f;
        float duration = 0.5f;
        Color startColor = text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        text.color = endColor;
        AnswerAllQuestionText.gameObject.SetActive(false);
    }

    public void HideYouNeedToAnswerAllQuestionsText()
    {
        AnswerAllQuestionText.gameObject.SetActive(false);
    }

    private void SetTextToNothing()
    {
        afterQuizUserAnswerText.text = "";
        afterQuizCorrectAnswerText.text = "";
    }
    public IEnumerator PingOpenQuestionPanelButton()
    {
        RectTransform buttonRect = OpenQuestionsPanelButton.GetComponent<RectTransform>();
        Vector3 originalScale = new Vector3(1f, 1f, 1f);
        Vector3 targetScale = new Vector3(buttonPingScale, buttonPingScale, 1f);

        // Scale up
        float elapsed = 0f;
        while (elapsed < buttonScaleDuration / 2)
        {
            elapsed += Time.deltaTime;
            buttonRect.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (buttonScaleDuration / 2));
            yield return null;
        }

        // Scale down
        elapsed = 0f;
        while (elapsed < buttonScaleDuration / 2)
        {
            elapsed += Time.deltaTime;
            buttonRect.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / (buttonScaleDuration / 2));
            yield return null;
        }

        buttonRect.localScale = originalScale;
    }

    public void ShowAfterQuizSection()
    {
        CG_AfterQuizSection.gameObject.SetActive(true);
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(CG_AfterQuizSection, true, 1f));
    }

    public void HideAfterQuizSection()
    {
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(CG_AfterQuizSection, false, 0.5f));
        StartCoroutine(DisableGameObjectAfterFade(CG_AfterQuizSection.gameObject, 0.5f));
    }

    public void StartShowAnswerSection()
    {
        // This function would handle the logic for showing the answer section 
        // and transition to the next exercise scene.
    }

    public void TogglePanelVisibility()
    {
        if(FindAnyObjectByType<ControlTimeWithDistance>() != null)
        {
            FindAnyObjectByType<ControlTimeWithDistance>().HideTutorialInformation();
        }
      

        SoundEffectManager.PlaySingleSound("MenuHover");
        if (panelHidden)
        {
            ShowQuestionsPanel();
            panelHidden = false;
        }
        else
        {
            HideQuestionsPanel();
            panelHidden = true;
        }
    }

    // Updated easing function for smoother animation
    private float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }

    // Updated coroutine with easing function for snappier movement
    private IEnumerator MoveRecttransform(RectTransform rectTransform, Vector2 targetPos, float duration)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration; // Value between 0-1
            float easedTime = EaseOutQuad(normalizedTime); // Apply easing function
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, easedTime);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
    }

    private IEnumerator RotateRecttransform(RectTransform rectTransform, float targetRotation, float duration)
    {
        float startRotation = rectTransform.rotation.z;
        float curRotation = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            float easedTime = EaseOutQuad(normalizedTime);
            curRotation = Mathf.Lerp(startRotation, targetRotation, easedTime);
            rectTransform.localRotation = Quaternion.Euler(0, 0, curRotation);

            yield return null;
        }
        rectTransform.localRotation = Quaternion.Euler(0, 0, targetRotation);
    }

    private IEnumerator ScaleRectTransformScaleRectTransform(RectTransform rectTransform, Vector3 targetScale, float duration)
    {
        Vector3 startScale = rectTransform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            float easedTime = EaseOutQuad(normalizedTime);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, easedTime);
            yield return null;
        }
        rectTransform.localScale = targetScale;
    }

    private void HideQuestionsPanel()
    {
        if (!isUIHorizontal)
        {
            StartCoroutine(MoveRecttransform(questionsPanel, new Vector2(0, -1521.5f), panelAnimationDuration));
        }
        else if (isUIHorizontal)
        {
            StartCoroutine(MoveRecttransform(questionsPanel, new Vector2(-1075, 0), panelAnimationDuration));
        }

        StartCoroutine(ExtensionMethods.FadeCanvasGroup(CG_IconQuestionmark, true, 0.8f));
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(CG_IconArrow, false, 0.3f));
        StartCoroutine(RotateRecttransform(CG_IconArrow.GetComponent<RectTransform>(), 90, 0.8f));
    }

    private void ShowQuestionsPanel()
    {
        if (!isUIHorizontal)
        {
            StartCoroutine(MoveRecttransform(questionsPanel, new Vector2(0, 0), panelAnimationDuration));
        }
        else if (isUIHorizontal)
        {
            StartCoroutine(MoveRecttransform(questionsPanel, new Vector2(-100f, 0), panelAnimationDuration));
        }

        StartCoroutine(ExtensionMethods.FadeCanvasGroup(CG_IconQuestionmark, false, 0.3f));
        StartCoroutine(ExtensionMethods.FadeCanvasGroup(CG_IconArrow, true, 0.8f));
        StartCoroutine(RotateRecttransform(CG_IconArrow.GetComponent<RectTransform>(), -90, 0.8f));
    }
}