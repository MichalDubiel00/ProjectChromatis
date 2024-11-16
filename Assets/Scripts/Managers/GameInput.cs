using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class GameInput : MonoBehaviour
{
    public static GameInput instance { get; private set; }
  
    PlayerInput input;

    //Event Handlers
    public event EventHandler OnPauseAction;
    public event EventHandler OnDebugAction;
    public event EventHandler OnToggleAction;
    public event EventHandler OnNextColorAction;
    public event EventHandler OnPreviousColorAction;
    public event EventHandler OnThrowAction;




    private void Awake()
    {
        instance = this;
        input = new PlayerInput();
        input.Player.Enable();

        input.Player.Pause.performed += Pause_performed;
        input.Player.Debug.performed += Debug_performed;
        input.Player.Toggle.performed += Toggle_performed;
        input.Player.NextColor.performed += NextColor_performed;
        input.Player.PreviousColor.performed += PreviousColor_performed;
        input.Player.Throw.performed += Throw_performed;

    }

    private void Throw_performed(InputAction.CallbackContext obj)
    {
        OnThrowAction?.Invoke(this, EventArgs.Empty);
    }

    private void PreviousColor_performed(InputAction.CallbackContext obj)
    {
        OnPreviousColorAction?.Invoke(this, EventArgs.Empty);

    }

    private void NextColor_performed(InputAction.CallbackContext obj)
    {
        OnNextColorAction?.Invoke(this, EventArgs.Empty);

    }

    private void Toggle_performed(InputAction.CallbackContext obj)
    {
        OnToggleAction?.Invoke(this, EventArgs.Empty);
    }

    private void Debug_performed(InputAction.CallbackContext obj)
    {
        OnDebugAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        input.Player.Pause.performed -= Pause_performed;
        input.Player.Debug.performed -= Debug_performed;
        input.Player.Toggle.performed -= Toggle_performed;
        input.Player.NextColor.performed -= NextColor_performed;
        input.Player.PreviousColor.performed -= PreviousColor_performed;
        input.Player.Throw.performed -= Throw_performed;

        input.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    public bool GetJump() 
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public bool GetUpJump() 
    {
        return Input.GetKeyUp(KeyCode.Space);
    }

    public Vector2 GetMovmentInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        // Create a force vector based on input
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        return movement;
    }
}
