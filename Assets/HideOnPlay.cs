using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    private void Start()
    {
#if UNITY_EDITOR
        GetComponent<Renderer>().enabled = false;
       

#endif
    }

    private void OnEnable()
    {
    }
    public void HideThisObject()
    {
        GetComponent<Renderer>().enabled = false;
    }
}