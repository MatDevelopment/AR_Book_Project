using System.Collections;
using UnityEngine;

public class DespawnEndText : MonoBehaviour
{
    
    IEnumerator ShowUIForDuration_NoTextEdit_(GameObject _UI_Gameobject, float _Duration)
    {
        _UI_Gameobject.SetActive(true);
        yield return new WaitForSeconds(_Duration);
        _UI_Gameobject.SetActive(false);
    }

    public void Method_ShowUIForDuration_NoTextEdit_(GameObject UI_Gameobject_, float Duration_)
    {
        StartCoroutine(ShowUIForDuration_NoTextEdit_(UI_Gameobject_, Duration_));
    }
}
