using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject sunObject;
    private RotateOverTime sunRotateScript;
    private float sunInitialRotation;

    // Inserting all the gameobjects that needs to be controlled by the script
    [SerializeField]
    private GameObject[] planetObjects = new GameObject[9];

    // Creating a class that holds the initial information about each planets rotation speeds
    public class Planet
    {
        public float initialRotateSolar;
        public float initialRotateSelf;
        public RotateAroundCenter rotateScript;
    }

    // Internal list that updates based on the amount of planets given
    private List<Planet> planets = new List<Planet>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sunRotateScript = sunObject.GetComponent<RotateOverTime>();
        sunInitialRotation = sunRotateScript._degreesPerSecondToRotateSelf;
        
        // Going through each gameobject given and taking the information from their scripts
        foreach (GameObject p in planetObjects)
        {
            Planet currentPlanet = new Planet();
            currentPlanet.rotateScript = p.GetComponent<RotateAroundCenter>();
            currentPlanet.initialRotateSolar = currentPlanet.rotateScript._degreesPerSecondToRotateAroundSun;
            currentPlanet.initialRotateSelf = currentPlanet.rotateScript._degreesPerSecondToRotateSelf;
            planets.Add(currentPlanet);
        }
    }

    // Function used to pause all rotations
    public void StopAllPlanetsRotating()
    {
        sunRotateScript._degreesPerSecondToRotateSelf = 0f;
        
        foreach (Planet planet in planets)
        {
            planet.rotateScript._degreesPerSecondToRotateAroundSun = 0;
            planet.rotateScript._degreesPerSecondToRotateSelf = 0;
        }
    }

    // Function used to start the rotations again after pausing
    public void StartAllPlanetsRotating()
    {
        sunRotateScript._degreesPerSecondToRotateSelf = sunInitialRotation;
        
        foreach (Planet planet in planets)
        {
            planet.rotateScript._degreesPerSecondToRotateAroundSun = planet.initialRotateSolar;
            planet.rotateScript._degreesPerSecondToRotateSelf = planet.initialRotateSelf;
        }
    }
}
