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
    public event EventHandler OnPauseAction;




    private void Awake()
    {
        instance = this;
        input = new PlayerInput();
        input.Player.Enable();

        input.Player.Pause.performed += Pause_performed;


    }

    private void OnDestroy()
    {
        input.Player.Pause.performed -= Pause_performed;

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

    public Vector2 GetInput()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        // Create a force vector based on input
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        return movement;
    }
}
