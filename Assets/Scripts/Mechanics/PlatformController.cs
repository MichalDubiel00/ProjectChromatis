using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform posA, posB;
    bool moveOn = false;
    [SerializeField] bool canBeBlue = true, canBeRed = true, canBeYellow = true;
    ColorPicker.ColorEnum currentColor = ColorPicker.ColorEnum.Gray;
    private SpriteRenderer _SpriteRenderer;


    public ColorPicker.ColorEnum CurrentColor
    {
        get => currentColor;
        set => currentColor = value;
    }

    Vector2 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _SpriteRenderer.color = Color.gray;
        targetPos = posB.position;
        GameInput.instance.OnDebugAction += Instance_OnDebugAction;

    }

    private void Instance_OnDebugAction(object sender, System.EventArgs e)
    {
        moveOn = !moveOn;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveOn)
            MovePlatform();

    }

    void MovePlatform()
    {
        if (Vector2.Distance(transform.position, posA.position) <= .1f)
            targetPos = posB.position;
        if (Vector2.Distance(transform.position, posB.position) <= .1f)
            targetPos = posA.position;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, 1 * Time.deltaTime);
    }

    public void ChangePlatformProporties(ColorPicker.ColorEnum color)
    {
        switch (color)
        {
            case ColorPicker.ColorEnum.Blue:
                if (canBeBlue)
                {
                    Debug.Log("Hello");

                    if (_SpriteRenderer.color != new Color(0, 0, 255))
                        _SpriteRenderer.color = new Color(0, 0, 255);
                    moveOn = true;
                }
                break;
            case ColorPicker.ColorEnum.Red:
                if (canBeRed)
                {
                    if (_SpriteRenderer.color != new Color(255, 0, 0))
                        _SpriteRenderer.color = new Color(255, 0, 0);
                    moveOn = false;
                }

                break;
            case ColorPicker.ColorEnum.Yellow:
                if (canBeYellow)
                {
                    if (_SpriteRenderer.color != Color.yellow)
                        _SpriteRenderer.color = Color.yellow;
                    moveOn = false;
                }
                break;
        }
    }
}
