using System;
using UnityEngine;

public class RotateAroundCenter : MonoBehaviour
{
    [SerializeField] private GameObject SunGameobject_SolarSystemCenter;

    public float _degreesPerSecondToRotateAroundSun;
    public float _degreesPerSecondToRotateSelf;
    //[SerializeField] private Vector3 selfRotateVector;
    

    private void FixedUpdate()
    {
        gameObject.transform.RotateAround(SunGameobject_SolarSystemCenter.transform.position,Vector3.up, _degreesPerSecondToRotateAroundSun * Time.deltaTime);
        gameObject.transform.Rotate(Vector3.up, _degreesPerSecondToRotateSelf * Time.deltaTime, Space.Self);
        
        
    }
}


