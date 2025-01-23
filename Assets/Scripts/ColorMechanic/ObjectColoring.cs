using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;

public class ObjectColoring : MonoBehaviour
{
    [SerializeField] bool canBeBlue = true, canBeRed = true, canBeYellow = true;
    [SerializeField] PlatformController platformController;

    [SerializeField] ColorPicker.ColorEnum currentColor = ColorPicker.ColorEnum.Gray;
    [SerializeField] float maxSpread = 1.3f;

    [HideInInspector]
    public SpriteRenderer _SpriteRenderer;
    Collider2D _Collider;
    PolygonCollider2D _PolygonCollider;
    PlatformEffector2D _Effector;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask enemyLayer;
    int groundMask;
    int ghostMask;

    private Color targetColor;

    private Material materialInstance; // Unique material instance for this object    public Vector3 hitPoint;
    private Vector3 hitPoint;
    public float spread;

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

        materialInstance = Instantiate(_SpriteRenderer.material);
        _SpriteRenderer.material = materialInstance;

        _Effector = GetComponent<PlatformEffector2D>();
        _PolygonCollider = GetComponent<PolygonCollider2D>();
        _Collider = GetComponent<Collider2D>();

        //material = GetComponent<Material>();
        if (currentColor != ColorPicker.ColorEnum.Gray )
            ChangePlatformProporties(currentColor,Vector2.zero);
    }

    void Update()
    {

        // Update the color lerp over time
        materialInstance.SetVector("_HitPoint", hitPoint);
        materialInstance.SetFloat("_Spread", spread);

        // Example: Gradually increase the spread radius
        spread = Mathf.Min(spread + Time.deltaTime * 1.5f, maxSpread); // Define maxSpread as a serialized field.
                                                                       
    }

    public void ChangePlatformProporties(ColorPicker.ColorEnum color,Vector2 collisionPoint)
    {
        ColorPicker.ColorEnum prevColor = currentColor;
        currentColor = color;

        switch (color)
        {
            case ColorPicker.ColorEnum.Blue:
                if (canBeBlue)
                {

                    if (prevColor == ColorPicker.ColorEnum.Yellow || prevColor == ColorPicker.ColorEnum.Gray || prevColor == ColorPicker.ColorEnum.Blue)
                        targetColor = new Color(0, 0, 1);

                    if (prevColor == ColorPicker.ColorEnum.Red)
                    {
                        targetColor = new Color(0, 0, 1, 0.3f);
                    }
                    materialInstance.SetColor("_TargetColor", targetColor);

                    hitPoint = collisionPoint;

                    spread = 0f; // Reset the lerp timer
                    platformController.moveOn = true;
                }
                break;
            case ColorPicker.ColorEnum.Red:
                if (canBeRed)
                {
                    targetColor = new Color(1, 0, 0, 0.3f); // Set target color to Red with transparency
                    materialInstance.SetColor("_TargetColor", targetColor);

                    hitPoint = collisionPoint;


                    spread = 0f; // Reset the lerp timer
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
                    materialInstance.SetColor("_TargetColor", targetColor);

                    hitPoint = collisionPoint;

                    spread = 0f; // Reset the lerp timer
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
                targetColor = new Color(1, 1, 1, 1); // Set target color to Gray
                materialInstance.SetColor("_TargetColor", targetColor);

                spread = 0f; // Reset the lerp timer
                hitPoint = collisionPoint;
                break;
        }
    }
}
