using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Vector3 lastCheckpoint;
    public Dictionary<ColorPicker.ColorEnum, int> checkPointColors = new Dictionary<ColorPicker.ColorEnum, int>();
    public static CheckpointManager instance { get; private set; }
    Player player;
    PlayerHealth health;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
        health = FindAnyObjectByType<PlayerHealth>();
        lastCheckpoint = player.transform.position;
        instance = this;
    }

    public void UpdateCheckpoint(Transform checkpoint)
    {
        checkPointColors = player.Colors;
        lastCheckpoint = checkpoint.position;
    }

    public void Respawn() 
    {
        Rigidbody2D rigidbody2D = player.GetComponent<Rigidbody2D>();
        var movement = player.GetComponent<PlayerMovement>();
        movement.isOnPlatform = false;
        rigidbody2D.velocity = Vector3.zero;
        player.transform.position = lastCheckpoint;
        if (checkPointColors.Count != 0)
            player.Colors = checkPointColors;
        health.Heal(health.maxHealth);
    }

}
