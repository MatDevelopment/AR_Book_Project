using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.XR.CoreUtils;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraRaycastSelection : MonoBehaviour
{
    //LayerMask _layerMask = LayerMask.GetMask("Planet", "Star");

    //private string[] _tagsPlanetsAndStars = new string[4]{"Sun_Core", "Sun_RadiativeZone", "Sun_ConvectionZone", "PhotoSphere"};
    [SerializeField] private GameObject[] _gameobjectLayersOfPlanetsAndStars;

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity))
        {
            for (int i = 0; i < _gameobjectLayersOfPlanetsAndStars.Length; i++)
            {
                if (hit.transform.gameObject.name == (_gameobjectLayersOfPlanetsAndStars[i].name))
                {
                    _gameobjectLayersOfPlanetsAndStars[i].transform.GetChild(0).gameObject.SetActive(true);       //Make sure that the UI information box canvas is the first child of the object, or then it will not work.
                    Debug.DrawRay(transform.position, Vector3.forward * hit.distance, Color.red); 
                    Debug.Log("Pointing at something interactable");
                }
                else
                {
                    _gameobjectLayersOfPlanetsAndStars[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Not pointing at something interactable");
        }
    }
}
