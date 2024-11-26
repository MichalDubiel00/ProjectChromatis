using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput instance { get; private set; }

    private PlayerInput input;

    // Event Handlers
    public event EventHandler OnPauseAction;
    public event EventHandler OnDebugAction;
    public event EventHandler OnToggleAction;
    public event EventHandler OnNextColorAction;
    public event EventHandler OnPreviousColorAction;
    public event EventHandler OnThrowAction;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        input = new PlayerInput();
        input.Player.Enable();

        BindActions();
    }

    private void BindActions()
    {
        input.Player.Pause.performed += ctx => OnPauseAction?.Invoke(this, EventArgs.Empty);
        input.Player.Debug.performed += ctx => OnDebugAction?.Invoke(this, EventArgs.Empty);
        input.Player.Toggle.performed += ctx => OnToggleAction?.Invoke(this, EventArgs.Empty);
        input.Player.NextColor.performed += ctx => OnNextColorAction?.Invoke(this, EventArgs.Empty);
        input.Player.PreviousColor.performed += ctx => OnPreviousColorAction?.Invoke(this, EventArgs.Empty);
        input.Player.Throw.performed += ctx => OnThrowAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        input.Dispose();
    }

    public bool GetJump()
    {
        return input.Player.Jump.triggered;
    }

    public bool GetUpJump()
    {
        return input.Player.Jump.WasReleasedThisFrame();
    }

    public Vector2 GetMovmentInput()
    {
        return input.Player.Move.ReadValue<Vector2>();
    }
}
