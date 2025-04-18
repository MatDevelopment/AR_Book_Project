using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public float _degreesPerSecondToRotateSelf;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.up, _degreesPerSecondToRotateSelf * Time.deltaTime, Space.Self);
    }
}
