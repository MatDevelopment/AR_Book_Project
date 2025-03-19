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

    private XRIDefaultInputActions xriDefaultActions;
    private Camera mainCamera;

    private void Awake()
    {
        xriDefaultActions = new XRIDefaultInputActions();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        xriDefaultActions.Enable();
    }

    private void OnDisable()
    {
        xriDefaultActions.Disable();
    }

    void Start()
    {
        //Subscribing to events
        xriDefaultActions.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        xriDefaultActions.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null) OnStartTouch(Utils.ScreenToWorld(mainCamera, xriDefaultActions.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
        Debug.Log("StartTouchPrimary");
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null) OnEndTouch(Utils.ScreenToWorld(mainCamera, xriDefaultActions.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);
        Debug.Log("EndTouchPrimary");

    }

    public Vector2 PrimaryPostition()
    {
        return Utils.ScreenToWorld(mainCamera, xriDefaultActions.Touch.PrimaryPosition.ReadValue<Vector2>());
    }

}