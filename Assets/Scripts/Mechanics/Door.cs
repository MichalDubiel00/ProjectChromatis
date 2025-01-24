using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour
{
    //TODO: find fiting sprites animate it
    //TODO: make it open with button not just on entering or that you need key or something
    [SerializeField] Sprite openDoorSprite;
    [SerializeField] Sprite closedDoorSprite;
    [SerializeField] AudioClip openDoorAudioClip;
    [SerializeField] AudioClip closedDoorAudioClip;

    [SerializeField] Loader.Scene scene;
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
        FindAnyObjectByType<Fade>().startFade = true;
        StartCoroutine(LoadNewLevel());

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null ) 
            CloseDoor();        
    }
    private System.Collections.IEnumerator LoadNewLevel()
    {
        
        yield return new WaitForSeconds(2);
        Loader.Load(scene);
    }
    private void CloseDoor()
    {
        if (isOpen)
        {
            AudioSource.PlayClipAtPoint(closedDoorAudioClip, transform.position, 1f);
            spriteRenderer.sprite = closedDoorSprite;
            isOpen = false;
        }
    }
    void OpenDoor()
    {
        if (!isOpen)
        {
            spriteRenderer.sprite = openDoorSprite;
            AudioSource.PlayClipAtPoint(openDoorAudioClip, transform.position, 1f);

            isOpen = true;
        }
    }
}