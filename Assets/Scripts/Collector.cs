using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject colorDropletPrefab; // Prefab of the color droplet
    [SerializeField] Camera mainCamera; // The main camera in the scene

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ColorPicker picker = collision.GetComponent<ColorPicker>();
        if (picker != null) 
        {
            CollectColor(collision.gameObject, picker.Amount, picker.MyColor.ToString());
        }
    }
    void CollectColor(GameObject colorObj, int amount, string color)
    {
        //Destroys Color Drop
        Destroy(colorObj);

        player.Colors[color] = Mathf.Min(player.Colors[color] + amount, player.MaxCapacity);
        player.ChangeColor(color);
        player.UpdateColorUI(color);
        spawnCollectible();


    }
    void spawnCollectible()
    {
        // Get the camera's viewport boundaries in world space
        float cameraWidth = mainCamera.orthographicSize * mainCamera.aspect; // Horizontal size in world space
        float cameraHeight = mainCamera.orthographicSize; // Vertical size in world space

        // Generate random positions within the camera's viewport
        float spawnX = Random.Range(-cameraWidth, cameraWidth);
        float spawnY = Random.Range(-cameraHeight, cameraHeight);

        // Create a position vector for the spawn location
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        // Instantiate the color droplet prefab at the random position
        // Instantiate the color droplet prefab at the random position
        GameObject spawnedDroplet = Instantiate(colorDropletPrefab, spawnPosition, Quaternion.identity);

      
        DropletColor dropletColor = spawnedDroplet.GetComponent<DropletColor>();
        ColorPicker colorPicker = spawnedDroplet.GetComponent<ColorPicker>();

        if (dropletColor != null && colorPicker != null)
        {
            Color randomColor = Color.white;
            switch (GetRandomColor())
            {
                case 0:
                    colorPicker.MyColor = ColorPicker.ColorEnum.Red;
                    randomColor = Color.red;
                    break;
                case 1:
                    colorPicker.MyColor = ColorPicker.ColorEnum.Blue;
                    randomColor = Color.blue;
                    break;
                case 2:
                    colorPicker.MyColor = ColorPicker.ColorEnum.Yellow;
                    randomColor = Color.yellow;
                    break;
            }
            dropletColor.SetColor(randomColor);
        }

    }
    int GetRandomColor()
    {
        // Randomly select a color from the available options
        return Random.Range(0, 3); // 0, 1, or 2 for Red, Blue, Yellow
        
    }
}
