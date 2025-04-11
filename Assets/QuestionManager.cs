using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [SerializeField]
    public List<Question> questions = new List<Question>();

    public Transform QuestionsParent;
    public GameObject questionBoxPrefab;
    public GameObject togglePrefab;

    void Start()
    {
        // Insatntaite question boxes for all questions in the list
        for (int i = 0; i < questions.Count; i++)
        {
            GameObject questionBox = Instantiate(questionBoxPrefab, QuestionsParent);
            for (int j = 0; j < questions[i].possibleAnswers.Count; j++)
            {
                GameObject toggle = Instantiate(togglePrefab, questionBox.GetComponentInChildren<VerticalLayoutGroup>().transform);
                toggle.GetComponentInChildren<Text>().text = questions[i].possibleAnswers[j];
                Toggle toggleComponent = toggle.GetComponent<Toggle>();


            }

        }

    }

}
