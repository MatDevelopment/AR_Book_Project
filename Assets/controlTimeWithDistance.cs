using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class ControlTimeWithDistance : MonoBehaviour
{
    //private ParticleSystem ps;

    [SerializeField]
    private List<ParticleSystem> childSystems;
    public TextMeshProUGUI distanceText;
    [SerializeField]
    float distanceToCamera; // You can change this in the Inspector to test.
    private Camera cam;

    void Start()

    {
        childSystems.Add(GetComponent<ParticleSystem>());
        foreach (Transform child in transform)
        {
            childSystems.Add(child.GetComponent<ParticleSystem>());
        }
        //ps = GetComponent<ParticleSystem>();
        cam = Camera.main;
        // Ensure the system does not start automatically.

        foreach (ParticleSystem child in childSystems)
        {
            child.Stop();
        }
        //ps.Stop();
    }

    void Update()
    {
        // Optionally, compute distance automatically:
        distanceToCamera = Vector3.Distance(cam.transform.position, transform.position);

        distanceToCamera -= 0.8f; // Offset to make the particles start at the camera position

        if(distanceToCamera < 0)
        {
            distanceToCamera = 0;
        }

        distanceText.text = "dist: " + distanceToCamera.ToString("F2") + "m";
        foreach (ParticleSystem child in childSystems)
        {
            // Reset the simulation
            child.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // Simulate the particle system up to the time corresponding to your distance.
            // The second parameter (withChildren) applies the simulation to child systems.
            // The third parameter (restart) resets the simulation so it starts from time 0.
            child.Simulate(distanceToCamera, withChildren: true, restart: true);

            // Play the system so that the simulated particles are visible.
            child.Play();
        }
      
    }
}
