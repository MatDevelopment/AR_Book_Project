using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiggingTouchManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        //touchPressAction = _playerInput.actions.FindAction("TouchPress");     // Does the same as _playerInput.actions["TouchPress"]
        touchPressAction = _playerInput.actions["TouchPress"];
        touchPositionAction = _playerInput.actions["TouchPosition"];
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        Debug.Log(value);
        // buttonValue = context.ReadValueAsButton();
    }
}
