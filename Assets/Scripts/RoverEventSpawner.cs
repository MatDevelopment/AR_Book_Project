using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class RoverEventSpawner : MonoBehaviour
{
    // Idea is to take inspiration from: http://youtube.com/watch?v=oBKrdRI_NGI
    // To instantiate certain randomly chosen points of interests from a list around the scanned environment
    // Then when the rover gets near, it should activate an action to start digging for artifacts

    // Unity Documentation on ARPlaneManager API: https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.1/manual/features/plane-detection/arplanemanager.html 
    // Mars Rock 3D Models From: https://sketchfab.com/3d-models/mars-rocks-9f5c946255a24f1cb630ea95dceea587

    [SerializeField]
    private ARPlaneManager arPlaneManager;

    [SerializeField]
    private RoverControl roverControl;

    [SerializeField]
    private GameObject placedObjectPrefab;

    [SerializeField]
    [Range(0f, 1f)]
    private float spawnRate = 0.1f;

    private int planesAdded = 0;
    private bool roverActivated = false;

    private void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
    {
        foreach (var plane in changes.added)
        {
            // handle added planes
            // Checks if the plane is horizontal plane with a up normal vector
            Debug.Log($"Plane Added: {plane.alignment.ToString()}");
            if (plane.alignment.ToString() == "HorizontalUp")
            {
                // Defining the rate in which the objects spawn on newly created surfaces
                float randomNumber = Random.Range(0f, 1f);
                if (randomNumber <= spawnRate)
                {
                    GameObject placedObject = Instantiate(placedObjectPrefab, plane.transform.position, Quaternion.identity);
                    roverControl.UpdateListOfRoverEvents();
                    Debug.Log("Object Spawned on Add");
                }

                planesAdded++;

                if (planesAdded >= 3 && !roverActivated)
                {
                    roverControl.ReadyToSpawnRover();
                    roverActivated = true;
                }
            }
        }

        foreach (var plane in changes.updated)
        {
            // handle updated planes
            // Checks if the plane is horizontal plane with a up normal vector
            //Debug.Log($"Plane Updated: {plane.alignment.ToString()}");
            //if (plane.alignment.ToString() == "HorizontalUp")
            //{
            //    // Defining the rate in which the objects spawn on newly created surfaces
            //    float randomNumber = Random.Range(0f, 1f);
            //    if (randomNumber <= spawnRate)
            //    {
            //        GameObject placedObject = Instantiate(placedObjectPrefab, plane.transform.position, Quaternion.identity);
            //        Debug.Log("Object Spawned on Update");
            //    }
            //}
        }

        foreach (var plane in changes.removed)
        {
            // handle removed planes
        }
    }
}
