using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using Color = UnityEngine.Color;

public class CharacterRanged : MonoBehaviour
{
    [SerializeField] private GameObject colorDroplet;
    [SerializeField] private GameObject pivotPointThrow;
    [SerializeField] private float strength = 2.0f;
    Player player;

	SoundManager audioManager;
	private Vector3 mousePosition;
    private Vector2 characterPosition;
    private PlayerMovement movementScript;
	private void Awake()
	{
		audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
	}
	// Start is called before the first frame update
	void Start()
    {        
        movementScript = gameObject.GetComponent<PlayerMovement>();
        GameInput.instance.OnThrowAction += Instance_OnThrowAction;
        player = gameObject.GetComponent<Player>();
    }

    private void Instance_OnThrowAction(object sender, System.EventArgs e)
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        characterPosition = mousePosition - gameObject.transform.position;
        characterPosition.Normalize();
        throwDroplet(characterPosition);

        if (movementScript.IsFacingRight && transform.position.x > mousePosition.x)
        {
            movementScript.CheckDirectionToFace(!movementScript.IsFacingRight);
        }
        if (!movementScript.IsFacingRight && transform.position.x < mousePosition.x)
        {
            movementScript.CheckDirectionToFace(!movementScript.IsFacingRight);
        }
    }


    private void throwDroplet(Vector2 relativeMousePosition)
    {
        ColorPicker.ColorEnum choosenColor = player.CurrentColor;
        if (player.Colors[choosenColor] <= 0)
            return;
        else
        {
            player.Colors[choosenColor]--;
            player.UpdateColorUI(choosenColor);
            var ball = Instantiate<GameObject>(colorDroplet, pivotPointThrow.transform.position, Quaternion.identity);
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            var throwVelocity = relativeMousePosition *strength;
            rb.AddForce(throwVelocity, ForceMode2D.Impulse);
			audioManager.PlaySFX(audioManager.shootDrop);
			DropletColor dropletColor = ball.GetComponent<DropletColor>();
            ColorDroppletController colorDroppletController = ball.GetComponent<ColorDroppletController>();
            colorDroppletController.currentColor = choosenColor;
            colorDroppletController.isThrown = true;
            if (dropletColor != null) 
            {
                Color color = Color.white; // Default color in case of no match
                switch (player.CurrentColor)
                {
                    case ColorPicker.ColorEnum.Red:
                        color = Color.red;
                        break;
                    case ColorPicker.ColorEnum.Blue:
                        color = Color.blue;
                        break;
                    case ColorPicker.ColorEnum.Yellow:
                        color = Color.yellow;
                        break;
                }
                Debug.Log(player.CurrentColor.ToString());
                dropletColor.SetColor(color);
            }
        }
    }
}
