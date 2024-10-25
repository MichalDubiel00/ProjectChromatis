using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class GameInput : MonoBehaviour
{
    public static GameInput instance { get; private set; }
  
    PlayerInput input;



    private void Awake()
    {
        input = new PlayerInput();
        input.Player.Enable();

      
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
