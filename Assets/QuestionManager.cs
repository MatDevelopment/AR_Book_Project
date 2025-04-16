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
    public CanvasGroup afterQuizContainerGroup, IconCorrect, IconIncorrect, CG_NextExerciseButton, CG_AfterQuizSection;

    [SerializeField]
    private RectTransform questionsPanel;

    private bool panelHidden = true;

    void Start()
    {
        if (debugfinishButton)
            debugfinishButton.gameObject.SetActive(true);

        // Instantiate question boxes for all questions in the list
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
        HideAfterQuizSection();
        HideYouNeedToAnswerAllQuestionsText();
        HideCorrectIncorrectIcons();
        HideNextExerciseButton();
    }

    public void CheckForAllQuestionsAnswered()
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

        if (AllQuestionsAnswered)
        {
            ShowAfterQuizSection();
            StartAfterquizSession();
        }
    }

    public void StartAfterquizSession()
    {
        // For example, fade in the afterQuizContainerGroup while fading out others
        StartCoroutine(FadeCanvasGroup(afterQuizContainerGroup, true, 1f));
        StartCoroutine(ShowQuestionsSequentially());
    }

    private IEnumerator ShowQuestionsSequentially()
    {
        yield return new WaitForSeconds(0.5f); // Delay bbefore starting

        foreach (Question question in questions)
        {
            yield return StartCoroutine(ShowQuestionInformation(question));
            yield return new WaitForSeconds(1.3f); // Delay between clearing 
            SetClearTextColorAllAfterQuizText();
            HideCorrectIncorrectIcons();
            yield return new WaitForSeconds(1.3f); // Delay between showing next question
        }
        EndSessionFinished = true;
        ShowNextExerciseButton();
    }
    public void ShowNextExerciseButton()
    {
        CG_NextExerciseButton.gameObject.SetActive(true);   
        FadeCanvasGroup(CG_NextExerciseButton, true, 1f);
    }
    public void HideNextExerciseButton()
    {
        CG_NextExerciseButton.gameObject.SetActive(false);
    }
    public void LoadScene_PlanetsAndStars()
    {
        if (EndSessionFinished)
            SceneManager.LoadScene("PlanetsAndStars");
    }
    public void LoadScene_Rover()
    {
        if (EndSessionFinished)
            SceneManager.LoadScene("RoverScene");
    }
    public void LoadScene_End()
    {
        if (EndSessionFinished)
            SceneManager.LoadScene("End");
    }

    private IEnumerator ShowQuestionInformation(Question question)
    {
        HideCorrectIncorrectIcons();
        // Wait before showing question details
        yield return new WaitForSeconds(1);
        StartCoroutine(SetTextColorAfterDelay(afterQuizQuestionText, 0));
        afterQuizQuestionText.text = question.questionText;
        Debug.Log("answer: " + question.possibleAnswers[question.userAnswer]);

        // Show user answer after 4 seconds
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(SetTextColorAfterDelay(staticUserAnswer, 0));
        StartCoroutine(SetTextColorAfterDelay(afterQuizUserAnswerText, 0));
        afterQuizUserAnswerText.text = question.possibleAnswers[question.userAnswer];

        // Show correct answer after 6 seconds
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(SetTextColorAfterDelay(staticCorrectAnswer, 0));
        StartCoroutine(SetTextColorAfterDelay(afterQuizCorrectAnswerText, 0));
        afterQuizCorrectAnswerText.text = question.possibleAnswers[question.correctAnswerIndex];

        yield return new WaitForSeconds(1);
        if (question.userAnswer == question.correctAnswerIndex)
        {
            ShowCorrectIncorrectIcons(true);
        }
        else
        {
            ShowCorrectIncorrectIcons(false);
        }
    }

    private IEnumerator SetTextColorAfterDelay(TextMeshProUGUI textToShow, float delay)
    {
        yield return new WaitForSeconds(delay);
        textToShow.color = Color.white;
    }

    private void HideCorrectIncorrectIcons()
    {
        IconCorrect.alpha = 0;
        IconIncorrect.alpha = 0;
    }

    private void ShowCorrectIncorrectIcons(bool isCorrect)
    {
        if (isCorrect)
        {
            IconCorrect.alpha = 1;
            IconIncorrect.alpha = 0;
        }
        else
        {
            IconCorrect.alpha = 0;
            IconIncorrect.alpha = 1;
        }
    }

    /// <summary>
    /// Fades the given CanvasGroup either in or out smoothly over the specified duration.
    /// When fading in, it also fades out a preset list of other CanvasGroups.
    /// </summary>
    /// <param name="canvasGroup">The CanvasGroup to fade.</param>
    /// <param name="fadeIn">If true, fades in; if false, fades out.</param>
    /// <param name="duration">The duration of the fade.</param>
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn, float duration)
    {
        // When fading in, fade out all other canvas groups defined in this component.
        if (fadeIn)
        {
            //List<CanvasGroup> otherGroups = new List<CanvasGroup>
            //{
            //    afterQuizContainerGroup,
            //    IconCorrect,
            //    IconIncorrect,
            //    CG_NextExerciseButton,
            //    CG_AfterQuizSection
            //};

            //foreach (CanvasGroup cg in otherGroups)
            //{
            //    if (cg != canvasGroup)
            //    {
                    // Fade out any other canvas group that is not the current one.
                    //StartCoroutine(FadeCanvasGroup(cg, false, duration));
            //    }
            //}
        }

        float targetAlpha = fadeIn ? 1f : 0f;
        float initialAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    private void SetClearTextColorAllAfterQuizText()
    {
        afterQuizUserAnswerText.color = Color.clear;
        afterQuizCorrectAnswerText.color = Color.clear;
        afterQuizQuestionText.color = Color.clear;
        staticUserAnswer.color = Color.clear;
        staticCorrectAnswer.color = Color.clear;
    }

    public void ShowYouNeedToAnswerAllQuestionsText()
    {
        AnswerAllQuestionText.gameObject.SetActive(true);
        PingOpenQuestionPanelButton();
        Invoke("HideYouNeedToAnswerAllQuestionsText", 3f);
    }

    public void HideYouNeedToAnswerAllQuestionsText()
    {
        AnswerAllQuestionText.gameObject.SetActive(false);
    }

    private void PingOpenQuestionPanelButton()
    {
        OpenQuestionsPanelButton.GetComponent<RectTransform>().localScale = new Vector2(1.2f, 1.2f);
        Invoke("ReturnButtonToOldSizw", 0.4f);
    }

    private void ReturnButtonToOldSizw()
    {
        OpenQuestionsPanelButton.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
    }

    public void ShowAfterQuizSection()
    {
        // Assuming AfterQuizSection is a GameObject you want to enable,
        // you could also fade it in if you add a CanvasGroup component.
        CG_AfterQuizSection.gameObject.SetActive(true);
        // For example, fade it in:
        StartCoroutine(FadeCanvasGroup(CG_AfterQuizSection, true, 1f));
    }

    public void HideAfterQuizSection()
    {
        CG_AfterQuizSection.gameObject.SetActive(false);
    }

    public void StartShowAnswerSection()
    {
        // This function would handle the logic for showing the answer section 
        // and transition to the next exercise scene.
    }

    public void TogglePanelVisibility()
    {
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

    private void HideQuestionsPanel()
    {
        if (!isUIHorizontal)
        {
            questionsPanel.anchoredPosition = new Vector2(0, -1521.5f);
        }
        else if (isUIHorizontal)
        {
            questionsPanel.anchoredPosition = new Vector2(-1075, 0);
        }
    }

    private void ShowQuestionsPanel()
    {
        if (!isUIHorizontal)
        {
            questionsPanel.anchoredPosition = new Vector2(0, 0);
        }
        else if (isUIHorizontal)
        {
            questionsPanel.anchoredPosition = new Vector2(-100f, 0);
        }
    }
}
