using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class Gravity_InputManager : Singleton<Gravity_InputManager>
{
    #region Events
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;

    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    #endregion

    private Gravity_InputActions _gravity_InputActions;
    private Camera mainCamera;

    private void Awake()
    {
        _gravity_InputActions = new Gravity_InputActions();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _gravity_InputActions.Enable();
    }

    private void OnDisable()
    {
        _gravity_InputActions.Disable();
    }

    void Start()
    {
        //Subscribing to events
        _gravity_InputActions.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        _gravity_InputActions.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null) OnStartTouch(Utils.ScreenToWorld(mainCamera, _gravity_InputActions.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
        Debug.Log("StartTouchPrimary");
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null) OnEndTouch(Utils.ScreenToWorld(mainCamera, _gravity_InputActions.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);
        Debug.Log("EndTouchPrimary");

    }

    public Vector2 PrimaryPostition()
    {
        return Utils.ScreenToWorld(mainCamera, _gravity_InputActions.Touch.PrimaryPosition.ReadValue<Vector2>());
    }

}