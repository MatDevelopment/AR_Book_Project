using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[System.Serializable]
public class Question 
{
    public string questionText;

    public List<string> possibleAnswers = new List<string>();
    public int correctAnswerIndex;

   public void InitializeQuestionBox()
    {

    }
}
