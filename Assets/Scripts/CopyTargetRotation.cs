using System.Collections;
using UnityEngine;

public class CopyTargetRotation : MonoBehaviour
{
    // Target to copy
    [SerializeField]
    GameObject target;

    private void FixedUpdate()
    {
        transform.rotation = target.transform.rotation;
    }
}
