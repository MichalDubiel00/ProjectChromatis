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
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private Animator animator;
    private String currentAnimation = "";
    [HideInInspector] public ColorPicker.ColorEnum currentColor;

    [SerializeField] Collectible collectible;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        ChangeAnimation("DroppletAnimation");
        rb.constraints = RigidbodyConstraints2D.None;
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
                transform.rotation = Quaternion.Euler(0, 0, angle-90);
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
        ObjectColoring obj = collision.GetComponent<ObjectColoring>();
       
        if (!(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall") || obj != null))
            return;

     
        if(collision.gameObject.CompareTag("Wall"))
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        else
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(SquashEffect(collision));

        if (obj == null && isThrown == true)
        {
            isThrown = false;
            collectible.spawnCollectible(transform.position,currentColor);
        }
        isThrown = false;
 
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
       
        ObjectColoring obj = collision.GetComponent<ObjectColoring>();
        if (!(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall") || obj != null))
            return;
        if (obj != null)
            obj.ChangePlatformProporties(currentColor);
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
            rb.position = new Vector2(rb.position.x, rb.position.y - objectOffset/3); 
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