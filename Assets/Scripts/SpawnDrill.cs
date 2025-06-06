using UnityEngine;

public class SpawnDrill : MonoBehaviour
{
    [SerializeField] private GameObject XROrigin_Transform;

    [SerializeField] private GameObject DrillToSpawn;

    private float distanceFromCamera = 1f;
    
    public void SpawnDrill_ButtonMethod()
    {
        Vector3 spawnPosition = XROrigin_Transform.transform.position +
                                XROrigin_Transform.transform.forward * distanceFromCamera;
        // Instantiate(DrillToSpawn, spawnPosition, Quaternion.identity);

        DrillToSpawn.SetActive(true);
        DrillToSpawn.transform.position = spawnPosition;
    }

    public void ResetDrillPosition()
    {
        Vector3 spawnPosition = XROrigin_Transform.transform.position +
                                XROrigin_Transform.transform.forward * distanceFromCamera;
        
        DrillToSpawn.transform.position = spawnPosition;
    }
}
