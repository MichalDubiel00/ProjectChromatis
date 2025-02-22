using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDroppletController : MonoBehaviour
{
    public float stretchFactor = 0.1f;
    public float squashDuration = 0.1f;
    [HideInInspector] public bool isThrown = false;

    private bool isSquashing = false;
    private bool hit = false;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private Animator animator;
    private String currentAnimation = "";
    [HideInInspector] public ColorPicker.ColorEnum currentColor;
    private Color _color;
    private SpriteRenderer _SpriteRenderer;


    [SerializeField] ParticleSystem droppingWater;

    [SerializeField] Collectible collectible;
    // Start is called before the first frame update
    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        ChangeAnimation("DroppletAnimation");
        rb.constraints = RigidbodyConstraints2D.None;

        switch (currentColor)
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
        _SpriteRenderer.color = _color;
        var mainModule = droppingWater.main;
        mainModule.startColor = _color;
    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isSquashing)
        {
            //Rotate in Direction of Movement
            Vector2 velocity = rb.velocity;
            float speed = velocity.magnitude;

            if (speed > 0.1f)
            {
                //Rotate the droplet to align with the movement direction
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }
        }
    }

    private void ChangeAnimation(string anim, float crossfadeDuration = 0.2f)
    {
        currentAnimation = anim;
        animator.CrossFade(anim, crossfadeDuration);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Spikes spikes = collision.GetComponent<Spikes>();
        if (spikes != null && hit == false)
        {
            hit = true;
            collectible.spawnCollectible(FindAnyObjectByType<Player>().transform.position, currentColor);
            return;
        }
        ObjectColoring obj = collision.GetComponent<ObjectColoring>();
        if (collision.GetComponent<Player>() == null)
        {
            droppingWater.Stop();
        }

        if (!(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall") || obj != null))
            return;


        if (collision.gameObject.CompareTag("Wall"))
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        else
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(SquashEffect(collision));
        if (obj!= null && obj.CurrentColor == currentColor && hit == false)
        {
            hit = true;
            collectible.spawnCollectible(FindAnyObjectByType<Player>().transform.position, currentColor);
            return;
        }
        if (obj != null && hit == false)
        {
            hit = true;
            if (obj.notPuzzle)
            {

                Vector3 localPos = obj._SpriteRenderer.transform.InverseTransformPoint(transform.position);
                Bounds spriteBounds = obj._SpriteRenderer.sprite.bounds;
                float u = (localPos.x - spriteBounds.min.x) / spriteBounds.size.x;
                float v = (localPos.y - spriteBounds.min.y) / spriteBounds.size.y;
                obj.ChangePlatformProporties(currentColor, new Vector2(u, v));

                collectible.spawnCollectible(FindAnyObjectByType<Player>().transform.position, currentColor);

                return;
            }
            if (!obj.canBeBlue && currentColor == ColorPicker.ColorEnum.Blue)
            {
                collectible.spawnCollectible(FindAnyObjectByType<Player>().transform.position, currentColor);

                return;
            }
            if (!obj.canBeRed && currentColor == ColorPicker.ColorEnum.Red)
            {
                collectible.spawnCollectible(FindAnyObjectByType<Player>().transform.position, currentColor);
                return;
            }
            if (!obj.canBeYellow && currentColor == ColorPicker.ColorEnum.Yellow)
            {
                collectible.spawnCollectible(FindAnyObjectByType<Player>().transform.position, currentColor);
                return;
            }
        }
        if (obj != null)
        {
            hit = true;
            var collisionPoint = collision.gameObject.GetComponent<Collider2D>().transform.position;
            collisionPoint =  (collisionPoint - transform.position);
            Vector3 localPos = obj._SpriteRenderer.transform.InverseTransformPoint(transform.position);
            Bounds spriteBounds = obj._SpriteRenderer.sprite.bounds;
            float u = (localPos.x - spriteBounds.min.x) / spriteBounds.size.x;
            float v = (localPos.y - spriteBounds.min.y) / spriteBounds.size.y;
            obj.ChangePlatformProporties(currentColor, new Vector2(u, v));
        }
        if (obj == null && isThrown == true && hit == false)
        {
            hit = true;
            isThrown = false;
            collectible.spawnCollectible(transform.position, currentColor);
        }
        isThrown = false;

    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        ObjectColoring obj = collision.GetComponent<ObjectColoring>();
        if (!(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall") || obj != null))
            return;

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            StartCoroutine(SquezeEffect(collision));
        }
        isThrown = false;
    }
    private System.Collections.IEnumerator SquashEffect(Collider2D collision)
    {
        isSquashing = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //Calculate the offset of object and ground to position object correctly
        //TODO will probably dont work for Walls or Ceilings Redo at Best
        if (collision.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2))
        {
            var objectOffset = rb.position.y - rb2.position.y;
            rb.position = new Vector2(rb.position.x, rb.position.y - objectOffset / 3);
        }

        ChangeAnimation("DropletSplash");
        yield return new WaitForSeconds(squashDuration);
        Destroy(gameObject);
    }
    private System.Collections.IEnumerator SquezeEffect(Collider2D collision)
    {
        ChangeAnimation("DroppletAnimation");
        yield return new WaitForSeconds(squashDuration);
    }

}