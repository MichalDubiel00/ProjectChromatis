using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] Transform posA, posB;
    Vector2 targetPos;

    [SerializeField] float platformSpeed = 1f;
    bool moveOn = false;

    [SerializeField] bool canBeBlue = true, canBeRed = true, canBeYellow = true;

    
    ColorPicker.ColorEnum currentColor = ColorPicker.ColorEnum.Gray;
    GameObject parent;
    
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
        targetPos = posB.position;

    }
    

    // Update is called once per frame
    void Update()
    {
        if (moveOn)
            MovePlatform();

    }

    void MovePlatform()
    {
        if (posA != null || posB != null) 
        {
            if (Vector2.Distance(transform.position, posA.position) <= .1f)
                targetPos = posB.position;
            if (Vector2.Distance(transform.position, posB.position) <= .1f)
                targetPos = posA.position;

            transform.position = Vector2.MoveTowards(transform.position, targetPos, platformSpeed * Time.deltaTime);
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
                    Debug.Log("Hello");
                    if (prevColor == ColorPicker.ColorEnum.Yellow || prevColor == ColorPicker.ColorEnum.Gray)
                        _SpriteRenderer.color = new Color(0, 0, 1);
                    if (prevColor == ColorPicker.ColorEnum.Red)
                    {
                        _SpriteRenderer.color = new Color(0, 0, 1,0.3f);
                    }
                    moveOn = true;
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
                    moveOn = false;
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
                    moveOn = false;
                }
                break;
            default:
                _SpriteRenderer.color = Color.gray;
                break;
        }
    }
    //Logic for moving player with the platform

    void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (!moveOn) return;
        PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
        if (player != null && player.LastOnGroundTime > 0 && !player.IsJumping)
            collision.collider.transform.SetParent(transform);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!moveOn) return;
        PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();


        if (player != null && player.LastOnGroundTime > 0 && !player.IsJumping)
            collision.collider.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!moveOn) return;
        Player player = collision.gameObject.GetComponent<Player>();
        if (player == null)
            return;
        collision.collider.transform.SetParent(null);
    }

}
