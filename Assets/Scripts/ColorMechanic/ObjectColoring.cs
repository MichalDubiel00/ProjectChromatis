using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] LayerMask enemyLayer;
    int groundMask;
    int ghostMask;

    private Color targetColor;
    [SerializeField] float coloringTime = 1f;
    private float lerpTime = 1f; // Time to fully transition between colors
    private float lerpTimer = 0f;

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

    void Update()
    {
        // Update the color lerp over time
        if (lerpTimer < lerpTime)
        {
            lerpTimer += Time.deltaTime * coloringTime;
            _SpriteRenderer.color = Color.Lerp(_SpriteRenderer.color, targetColor, lerpTimer / lerpTime);
        }
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
                    targetColor = new Color(0, 0, 1); // Set target color to Blue
                    lerpTimer = 0f; // Reset the lerp timer
                    platformController.moveOn = true;
                }
                break;
            case ColorPicker.ColorEnum.Red:
                if (canBeRed)
                {
                    targetColor = new Color(1, 0, 0, 0.3f); // Set target color to Red with transparency
                    lerpTimer = 0f; // Reset the lerp timer
                    gameObject.layer = ghostMask;
                    if (_Effector != null)
                        _Effector.useColliderMask = false;
                    if (_PolygonCollider != null)
                    {
                        _PolygonCollider.excludeLayers = playerLayer;
                        _PolygonCollider.excludeLayers += enemyLayer;
                    }
                    if (_Collider != null)
                    {
                        _Collider.excludeLayers = playerLayer;
                        _Collider.excludeLayers += enemyLayer;
                    }

                    platformController.moveOn = false;
                }
                break;
            case ColorPicker.ColorEnum.Yellow:
                if (canBeYellow)
                {
                    targetColor = Color.yellow; // Set target color to Yellow
                    lerpTimer = 0f; // Reset the lerp timer
                    gameObject.layer = groundMask;
                    if (_Effector != null)
                        _Effector.useColliderMask = true;
                    if (_Collider != null)
                        _Collider.excludeLayers = 0;
                    if (_PolygonCollider != null)
                        _PolygonCollider.excludeLayers = 0;
                    platformController.moveOn = false;
                }
                break;
            default:
                targetColor = Color.gray; // Set target color to Gray
                lerpTimer = 0f; // Reset the lerp timer
                break;
        }
    }
}
