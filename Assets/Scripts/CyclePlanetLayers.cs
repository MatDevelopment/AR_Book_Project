using UnityEngine;

public class CyclePlanetLayers : MonoBehaviour
{
    private int LayerIndex = 0;

    [SerializeField] private GameObject[] LayerUIs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CycleThroughPlanetLayers()
    {
        if (LayerIndex>= 5)
        {
            LayerIndex = 0;
        }
        LayerIndex++;

        switch (LayerIndex)
        {
            case 1:
                foreach (var t in LayerUIs)
                {
                    t.SetActive(false);
                }
                LayerUIs[0].SetActive(true);
                break;
            case 2:
                foreach (var t in LayerUIs)
                {
                    t.SetActive(false);
                }
                LayerUIs[1].SetActive(true);
                break;
            case 3:
                foreach (var t in LayerUIs)
                {
                    t.SetActive(false);
                }
                LayerUIs[2].SetActive(true);
                break;
            case 4:
                foreach (var t in LayerUIs)
                {
                    t.SetActive(false);
                }
                LayerUIs[3].SetActive(true);
                break;
            default:
                foreach (var t in LayerUIs)
                {
                    t.SetActive(false);
                }
                break;
        }
        if (LayerIndex>= 5)
        {
            LayerIndex = 0;
        }
    }
}
