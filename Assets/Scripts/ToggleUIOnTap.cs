using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // Needed to check UI elements

public class ToggleUIOnTap : MonoBehaviour
{
    public GameObject uiPanel; // The UI GameObject to toggle
    public Camera mainCamera; // Assign the main camera
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.TouchActions.Tap.performed += _ => HandleTap();
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.TouchActions.Tap.performed -= _ => HandleTap();
        inputActions.Disable();
    }

    private void HandleTap()
    {
        if (IsPointerOverUI()) return; // Avoid UI interference

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject) // Check if tapped object is this GameObject
            {
                uiPanel.SetActive(!uiPanel.activeSelf);
            }
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}