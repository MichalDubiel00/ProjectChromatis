using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using Color = UnityEngine.Color;

public class ObjectColoring : MonoBehaviour
{
    [SerializeField] bool canBeBlue = true, canBeRed = true, canBeYellow = true;
    [SerializeField] PlatformController platformController;


    [SerializeField] ColorPicker.ColorEnum currentColor = ColorPicker.ColorEnum.Gray;

    SpriteRenderer _SpriteRenderer;

    Collider2D _Collider;
    PolygonCollider2D _PolygonCollider;
    PlatformEffector2D _Effector;
    [SerializeField] LayerMask playerLayer;
    int groundMask;
    int ghostMask;



    public ColorPicker.ColorEnum CurrentColor
    {
        get => currentColor;
        set => currentColor = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        groundMask = LayerMask.NameToLayer("Ground");
        ghostMask = LayerMask.NameToLayer("Ghost");


        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _Effector = GetComponent<PlatformEffector2D>();
        _PolygonCollider = GetComponent<PolygonCollider2D>();
        _Collider = GetComponent<Collider2D>();
        _SpriteRenderer.color = Color.gray;

        ChangePlatformProporties(currentColor);
    }


    public void ChangePlatformProporties(ColorPicker.ColorEnum color)
    {
        ColorPicker.ColorEnum prevColor = currentColor;
        currentColor = color;

        switch (color)
        {
            case ColorPicker.ColorEnum.Blue:
                if (canBeBlue)
                {
                    Debug.Log("Hello");
                    if (prevColor == ColorPicker.ColorEnum.Yellow || prevColor == ColorPicker.ColorEnum.Gray)
                        _SpriteRenderer.color = new Color(0, 0, 1);
                    if (prevColor == ColorPicker.ColorEnum.Red)
                    {
                        _SpriteRenderer.color = new Color(0, 0, 1, 0.3f);
                    }
                    platformController.moveOn = true;
                }
                break;
            case ColorPicker.ColorEnum.Red:
                if (canBeRed)
                {
                    gameObject.layer = ghostMask;
                    if (_SpriteRenderer.color != new Color(1, 0, 0.3f))
                        _SpriteRenderer.color = new Color(1, 0, 0, 0.3f);
                    if (_Effector != null)
                        _Effector.useColliderMask = false;
                    if (_PolygonCollider != null)
                    {
                        Debug.Log("collider");
                        _PolygonCollider.excludeLayers = playerLayer;
                    }
                    if (_Collider != null)
                        _Collider.excludeLayers = playerLayer;
                    platformController.moveOn = false;
                }

                break;
            case ColorPicker.ColorEnum.Yellow:
                if (canBeYellow)
                {
                    gameObject.layer = groundMask;
                    if (_SpriteRenderer.color != Color.yellow)
                        _SpriteRenderer.color = Color.yellow;
                    if (_Effector != null)
                        _Effector.useColliderMask = true;
                    if (_Collider != null)
                        _Collider.includeLayers = playerLayer;
                    if (_PolygonCollider != null)
                        _PolygonCollider.excludeLayers = playerLayer;
                    platformController.moveOn = false;
                }
                break;
            default:
                _SpriteRenderer.color = Color.gray;
                break;
        }
    }
    //Logic for moving player with the platform

}
