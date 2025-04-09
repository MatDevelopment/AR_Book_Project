using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DetectTap : MonoBehaviour, Gravity_InputActions.ITouchActions
{
    [Header("Events")]
    public UnityEvent OnTapDetected;

    [SerializeField]
    private Gravity_InputActions inputActions;
    [SerializeField]
    private Camera mainCamera;

    private Vector2 lastTouchPosition;
    private bool isTouching = false;

    private void Awake()
    {
        inputActions = new Gravity_InputActions();
        inputActions.Touch.SetCallbacks(this);
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Called whenever PrimaryContact (touch press) is triggered
    public void OnPrimaryContact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsPointerOverUI()) return;

            Ray ray = mainCamera.ScreenPointToRay(lastTouchPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    OnTapDetected?.Invoke();
                }
            }
        }
    }

    // Called whenever PrimaryPosition (touch position) changes
    public void OnPrimaryPosition(InputAction.CallbackContext context)
    {
        lastTouchPosition = context.ReadValue<Vector2>();
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject(); // Editor uses mouse
#else
        // Mobile: use fingerId = 0 for primary touch
        return EventSystem.current.IsPointerOverGameObject(0);
#endif
    }
}
