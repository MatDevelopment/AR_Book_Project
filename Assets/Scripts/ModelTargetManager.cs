using UnityEngine;

public class ModelTargetManager : MonoBehaviour
{
    [SerializeField]
    private GameObject modelTarget;

    public void ToggleModelTargetActive()
    {
        if (modelTarget.activeInHierarchy)
        {
            modelTarget.SetActive(false);
        } 
        else if (!modelTarget.activeInHierarchy)
        {
            modelTarget.SetActive(true);
        }
    }
}
