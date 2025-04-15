using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleAnswer : MonoBehaviour
{
    public int answerIndex = -1;
    public Question parentQuestion;
    public ToggleGroup parentToggleGroup;
    private void Start()
    {
        Toggle.ToggleEvent toggleEvent = GetComponent<Toggle>().onValueChanged;
        toggleEvent.AddListener(delegate {ToggleAnswerInParent(); });
    }
    public void ToggleAnswerInParent()
    {
        parentQuestion.userAnswer = answerIndex;
        parentToggleGroup.allowSwitchOff = false;
    }
}