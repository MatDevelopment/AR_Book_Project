using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Transform clickedObject;

    // Update is called once per frame
    void Update()
    {
        // check for touch input
        if(Input.touchCount > 0){
            if(Input.GetTouch(0).phase == TouchPhase.Began){
                handleClick(Input.GetTouch(0).position);
            }
        }
        // check for left mouse button clicked
        if (Input.GetButtonDown("Fire1")){
            handleClick(Input.mousePosition);
        }
    }

    void handleClick(Vector3 screenClickPosition)
    {
        // draw a Ray perpendicular to the camera at the clicked position
        // and create a RayHit to capture the first object that our Ray intersects
        Ray ray = Camera.main.ScreenPointToRay(screenClickPosition);
        RaycastHit rayHit;
        if(Physics.Raycast(ray.origin, ray.direction, out rayHit)){
            if(rayHit.transform.tag == "Clickable"){
                // do something with the object that was clicked
                clickedObject = rayHit.transform;
                if (clickedObject.GetChild(0).gameObject.activeInHierarchy)
                {
                    clickedObject.GetChild(0).gameObject.SetActive(false);
                }
                else if (clickedObject.GetChild(0).gameObject.activeInHierarchy == false)
                {
                    clickedObject.GetChild(0).gameObject.SetActive(true);
                }
                // reset ray and rayHit for the next click
                ray = new Ray();
                rayHit = new RaycastHit();
            }
        }
    }
}

