using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//Can be Removed
public class Collector : MonoBehaviour
{
    public static Collector instance { get; set; }

    [SerializeField] Player player;
    [SerializeField] GameObject colorDropletPrefab; // Prefab of the color droplet
    [SerializeField] Camera mainCamera; // The main camera in the scene
	SoundManager audioManager;
	private void Awake()
	{
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
		audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
	}
	private void OnTriggerEnter2D(Collider2D collision)
    {
        ColorPicker picker = collision.GetComponent<ColorPicker>();
        if (picker != null)
        {
            CollectColor(collision.gameObject,1, picker.MyColor);
			audioManager.PlaySFX(audioManager.collectDrop);
		}
    }
    void CollectColor(GameObject colorObj, int amount, ColorPicker.ColorEnum color)
    {
        //Destroys Color Drop
        Destroy(colorObj);

        player.Colors[color] = Mathf.Min(player.Colors[color] + amount, player.MaxCapacity);
        player.ChangeColor(color);
        player.UpdateColorUI(color);
        //spawnCollectible();


    }
    public void spawnCollectible(Vector2? position = null,ColorPicker.ColorEnum color  = ColorPicker.ColorEnum.Gray)
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
        spawnPosition = new Vector3(spawnX, spawnY,0);
        }
        else 
        {
            spawnPosition = new Vector3(position.Value.x,position.Value.y,0);
        }

        // Instantiate the color droplet prefab at the random position
        // Instantiate the color droplet prefab at the random position
        GameObject spawnedDroplet = Instantiate(colorDropletPrefab, spawnPosition, Quaternion.identity);

      
        DropletColor dropletColor = spawnedDroplet.GetComponent<DropletColor>();
        ColorPicker colorPicker = spawnedDroplet.GetComponent<ColorPicker>();

        if (dropletColor != null && colorPicker != null)
        {
            Color dColor = Color.white;
            int c;
            if (color == ColorPicker.ColorEnum.Gray)
                c = GetRandomColor();
            else
                c = (int)color;
            switch (c)
            {
                case 0:
                    colorPicker.MyColor = ColorPicker.ColorEnum.Red;
                    dColor = Color.red;
                    break;
                case 1:
                    colorPicker.MyColor = ColorPicker.ColorEnum.Blue;
                    dColor = Color.blue;
                    break;
                case 2:
                    colorPicker.MyColor = ColorPicker.ColorEnum.Yellow;
                    dColor = Color.yellow;
                    break;
            }
            dropletColor.SetColor(dColor);
        }

    }
    int GetRandomColor()
    {
        // Randomly select a color from the available options
        return Random.Range(0, 3); // 0, 1, or 2 for Red, Blue, Yellow
        
    }
}
