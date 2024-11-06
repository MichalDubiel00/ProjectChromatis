using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour
{
    //TODO: find fiting sprites animate it
    //TODO: make it open with button not on just entering or that you need key or something
    [SerializeField] Sprite openDoorSprite;
    [SerializeField] Sprite closedDoorSprite;
    private bool isOpen = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = closedDoorSprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
            OpenDoor();
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null ) 
            CloseDoor();
    }
    private void CloseDoor()
    {
        if (isOpen)
        {
            spriteRenderer.sprite = closedDoorSprite;
            isOpen = false;
        }
    }
    void OpenDoor()
    {
        if (!isOpen)
        {
            spriteRenderer.sprite = openDoorSprite;
            isOpen = true;
        }
    }
}
