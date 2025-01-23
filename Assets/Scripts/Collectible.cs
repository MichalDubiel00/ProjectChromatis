using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using Color = UnityEngine.Color;

public class Collectible : MonoBehaviour
{
    [SerializeField] GameObject collectible;


    enum Type
    {
        Droplet,
        Heart,
        Note
    }
    [SerializeField] Type type = Type.Droplet;

    // Reference to the SpriteRenderer
    [SerializeField] SpriteRenderer spriteRenderer;

    // Sprites for each collectible type
    [SerializeField] Sprite dropletSprite;
    [SerializeField] Sprite heartSprite;
    [SerializeField] Sprite noteSprite;



    [SerializeField] ColorPicker.ColorEnum color;
    [SerializeField] int amount = 1;



    [SerializeField] Camera mainCamera; // The main camera in the scene
    SoundManager audioManager;
    
    private void Awake()
    {
     
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    void UpdateSprite()
    {
        switch (type)
        {
            case Type.Droplet:
                spriteRenderer.sprite = dropletSprite;
                SetByPicker();
                break;
            case Type.Heart:
                spriteRenderer.sprite = heartSprite;
                spriteRenderer.color = Color.white;
                break;
            case Type.Note:
                spriteRenderer.sprite = noteSprite;
                spriteRenderer.color = Color.white;
                break;
        }
    }

    //not used in current game state
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        switch (type)
        {
            case Type.Droplet:
                CollectColor(amount, color, player);
                break;

            case Type.Heart:
                CollectHeart(player);
                break;

            case Type.Note:
                CollectNote(player);
                break;

            default:
                Debug.LogWarning("Unhandled collectible type!", this);
                break;
        }
    }

    void CollectColor(int amount, ColorPicker.ColorEnum color, Player player)
    {
        //Destroys Color Drop
        Destroy(gameObject);
        audioManager.PlaySFX(audioManager.collectDrop);

        player.Colors[color] = Mathf.Min(player.Colors[color] + amount, player.MaxCapacity);
        player.ChangeColor(color);
        player.UpdateColorUI(color);
        //spawnCollectible();


    }

    void CollectHeart(Player player)
    {
        Destroy(gameObject);
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.Heal(amount);
        //spawnCollectible();
    }

    void CollectNote(Player player)
    {
        Destroy(gameObject);
        player.notes += amount;
        //spawnCollectible();
    }


    void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void spawnCollectible(Vector2? position = null, ColorPicker.ColorEnum _color = ColorPicker.ColorEnum.Gray)
    {
        Vector3 spawnPosition;
        // Get the camera's viewport boundaries in world space
        if (position == null)
        {
            float cameraWidth = mainCamera.orthographicSize * mainCamera.aspect; // Horizontal size in world space
            float cameraHeight = mainCamera.orthographicSize; // Vertical size in world space

            // Generate random positions within the camera's viewport
            float spawnX = Random.Range(-cameraWidth, cameraWidth);
            float spawnY = Random.Range(-cameraHeight, cameraHeight);

            // Create a position vector for the spawn location
            spawnPosition = new Vector3(spawnX, spawnY, 0);
        }
        else
        {
            spawnPosition = new Vector3(position.Value.x, position.Value.y, 0);
        }

        // Instantiate the color droplet prefab at the random position
        // Instantiate the color droplet prefab at the random position
        GameObject spawnedCollectible = Instantiate(collectible, spawnPosition, Quaternion.identity);
        if (_color != ColorPicker.ColorEnum.Gray) 
        {
            Collectible collectible = spawnedCollectible.GetComponent<Collectible>();
            collectible.color = _color;
            collectible.SetByPicker();
        }


    }
    int GetRandomColor()
    {
        // Randomly select a color from the available options
        return Random.Range(0, 3); // 0, 1, or 2 for Red, Blue, Yellow

    }

    void SetByPicker()
    {
            Color _color = Color.white;
            switch (color)
            {
                case ColorPicker.ColorEnum.Red:
                    _color = Color.red;
                    break;
                case ColorPicker.ColorEnum.Blue:
                    _color = Color.blue;
                    break;
                case ColorPicker.ColorEnum.Yellow
                :
                    _color = Color.yellow;
                    break;
            }
            spriteRenderer.color = _color;
    }
}
