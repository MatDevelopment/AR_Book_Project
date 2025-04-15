using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[System.Serializable]
public class Question 
{
    public int userAnswer = -1; // -1 means no answer given
    public string questionText;

    public List<string> possibleAnswers = new List<string>();
    public int correctAnswerIndex;

}
