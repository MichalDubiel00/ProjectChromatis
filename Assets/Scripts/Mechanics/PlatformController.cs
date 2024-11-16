using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform posA, posB;
    bool moveOn = false;
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
        switch (currentColor) 
        {
            case ColorPicker.ColorEnum.Blue:
                _SpriteRenderer.color = new Color(0,0,255);
                MovePlatform();
                break;
            case ColorPicker.ColorEnum.Red: 
                _SpriteRenderer.color = new Color(255,0,0);
                break;
            case ColorPicker.ColorEnum.Yellow: 
                _SpriteRenderer.color = Color.yellow;
                break; 
        }

    }

    void MovePlatform()
    {
        if (Vector2.Distance(transform.position, posA.position) <= .1f)
            targetPos = posB.position;
        if (Vector2.Distance(transform.position, posB.position) <= .1f)
            targetPos = posA.position;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, 1 * Time.deltaTime);
    }
}
