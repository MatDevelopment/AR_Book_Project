using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ControlTimeWithDistance : MonoBehaviour
{
    private UI_BigBang uiBigBang;
    private AddSliderMarks addSliderMarks;
    private const int particleSystemMaxPlaybackTime = 10;

    private double distanceStart = 1.3; // Start of visualization
    private double distanceEnd = 4.0;   // End of visualization

    private double timeStart = 0.0;
    private double timeEnd = 13700000000; // 13.7 billion years (13.7 milliarder år) // changed from 13.7e9

    private Vector3 solarSystemMinimumScale = Vector3.zero;
    private Vector3 solarSystemMaximumScale = new Vector3(1f, 1f, 1f);

    [SerializeField]
    private float distanceForSolarSystemToSpawn = 8f;

    [SerializeField]
    private Transform solarSystemSpawnPointTransform;

    //[SerializeField]
    //private GameObject solarSystemPrefab;


    [SerializeField]
    private GameObject solarSystemObject;


    [SerializeField]
    private List<ParticleSystem> childSystems;
    public TextMeshProUGUI distanceText;
    [SerializeField]
    float distanceToCamera; // You can change this in the Inspector to test.
    [SerializeField]
    float timeByDistance;
    private Camera cam;

    public bool targetFound = false;
    private bool canUpdateMarkers = false;
    void Start()
    {

#if UNITY_EDITOR
        CallThisOnTargetFound();
#endif
    }
    public void CallThisOnTargetFound()
    {
        if(targetFound) return;

        uiBigBang = GetComponent<UI_BigBang>();
        uiBigBang.HideStartText();
        uiBigBang.ShowBigBangInformation();

        //solarSystemObject = Instantiate(solarSystemPrefab, solarSystemSpawnPointTransform.position, Quaternion.identity);
      
        addSliderMarks = FindAnyObjectByType<AddSliderMarks>();
        childSystems.Add(GetComponent<ParticleSystem>());
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ParticleSystem>() == null) continue;
            childSystems.Add(child.GetComponent<ParticleSystem>());
        }
        //ps = GetComponent<ParticleSystem>();
        cam = Camera.main;
        // Ensure the system does not start automatically.

        foreach (ParticleSystem child in childSystems)
        {
            child.Stop();
        }

        Invoke("SetCanUpdateMarkers", 0.2f);
        targetFound = true;
        //ps.Stop();
    }
    private void SetCanUpdateMarkers()
    {
        canUpdateMarkers = true;
    }
    private void OnEnable()
    {
     
    }
    void Update()
    {
        if (!targetFound) return;



        // Optionally, compute distance automatically:
        distanceToCamera = Vector3.Distance(cam.transform.position, transform.position);

        //distanceToCamera -= 0.5f; // Offset to make the particles start at the camera position
        //USED TO BE 0.8F
        if(distanceToCamera < 0)
        {
            distanceToCamera = 0;
        }

        float DistanceMinusMinumumStartDistance = (float)distanceToCamera - (float)distanceStart;

        timeByDistance = (float)ExtensionMethods.Remap((float)DistanceMinusMinumumStartDistance, (float)0, (float)distanceEnd, (float)0, (float)particleSystemMaxPlaybackTime);

        uiBigBang.SetInformationText((float)timeByDistance);

        distanceText.text = "dist: " + distanceToCamera.ToString("F2") + "m";
        foreach (ParticleSystem child in childSystems)
        {
            // Reset the simulation
            child.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // Simulate the particle system up to the time corresponding to your distance.
            // The second parameter (withChildren) applies the simulation to child systems.
            // The third parameter (restart) resets the simulation so it starts from time 0.
            child.Simulate((float)timeByDistance, withChildren: true, restart: true);

            // Play the system so that the simulated particles are visible.
            child.Play();
        }

        uiBigBang.UpdateTimeText(timeByDistance);
        uiBigBang.UpdateTimeSlider(timeByDistance);

        if(canUpdateMarkers)
        addSliderMarks.UpdateSliderMarks(timeByDistance);

        if (timeByDistance <= 7.8)
        {
            solarSystemObject.transform.localScale = solarSystemMinimumScale;
        }
        else if (timeByDistance > 7.8 && timeByDistance < 8.3f)
        {
            float t = ((float)timeByDistance - 8f) / 0.5f;
            solarSystemObject.transform.localScale = Vector3.Lerp(solarSystemMinimumScale, solarSystemMaximumScale, t);
        }
        else if (timeByDistance >= 8.3f && timeByDistance <= 9.4f)
        {

            solarSystemObject.transform.localScale = solarSystemMaximumScale;
        }
        else if (timeByDistance > 9.4f && timeByDistance < 9.9f)
        {
            float t = ((float)timeByDistance - 8.6f) / 0.5f;
            solarSystemObject.transform.localScale = Vector3.Lerp(solarSystemMaximumScale, solarSystemMinimumScale, t);
        }
        else
        {
            solarSystemObject.transform.localScale = solarSystemMinimumScale;
        }

        solarSystemObject.transform.position = solarSystemSpawnPointTransform.position;

    }
}
