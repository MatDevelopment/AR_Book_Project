using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [SerializeField]
    public List<Question> questions = new List<Question>();

    public Transform QuestionsParent;
    public GameObject questionBoxPrefab;
    public GameObject togglePrefab;

    [SerializeField]
    private RectTransform questionsPanel;

    private bool panelHidden = false;
    void Start()
    {
        // Insatntaite question boxes for all questions in the list
        for (int i = 0; i < questions.Count; i++)
        {
            GameObject questionBox = Instantiate(questionBoxPrefab, QuestionsParent);
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = questions[i].questionText;

            List<Toggle> toggles = new List<Toggle>();
            for (int j = 0; j < questions[i].possibleAnswers.Count; j++)
            {
                GameObject toggle = Instantiate(togglePrefab, questionBox.GetComponentInChildren<VerticalLayoutGroup>().transform);
                toggle.GetComponentInChildren<TextMeshProUGUI>().text = questions[i].possibleAnswers[j];
                Toggle toggleComponent = toggle.GetComponent<Toggle>();
                toggles.Add(toggleComponent);

                toggle.GetComponent<ToggleRelatedToggles>().otherToggles = toggles;
            }
        }
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
        questionsPanel.anchoredPosition = new Vector2(0, -1521.5f);
    }
    private void ShowQuestionsPanel()
    {
        questionsPanel.anchoredPosition = new Vector2(0, 0);
    }
}