using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Transform player;
    float nextFireTime;
    float fireCoolDown;
    bool _detected = false;

    Vector2 targetPos;


    [SerializeField] int damage = 5;
    [SerializeField] float shootingBuffer = 5;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float fireRate = 2f; // in Seconds
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] LayerMask obstacleMask;

    [Header("Moveable")]
    [SerializeField] bool patroling = false;
    [SerializeField] bool keepPatrolingOnDetection = false;
    [SerializeField] Transform leftPosition, RightPosition;
    [SerializeField] Vector2 direction = new Vector2(1.0f,1.0f);
    [SerializeField] float speed;

    public Rigidbody2D RB { get; private set; }
    public bool IsFacingRight { get; private set; }

    float shootingBufferTime;

    void Start()
    {
        targetPos = RightPosition.position;
        player = FindFirstObjectByType<Player>().transform;
        shootingBufferTime = shootingBuffer;

        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if ((patroling && !_detected) || keepPatrolingOnDetection)
        {
            MoveEnemy();
        }

        if (distanceToPlayer <= detectionRange)
        {
            _detected = true;
            if (shootingBuffer>=0)
            {
                shootingBuffer -= Time.deltaTime;
            }
        }
        else
        {
            _detected = false;
            shootingBuffer = shootingBufferTime;
        }

        if (HasLineOfSight() && _detected && shootingBuffer <= 0) 
            ShootAtPlayer();
            
    }
    void ShootAtPlayer()
    {
       
        {
            fireCoolDown -= Time.deltaTime;
            if (fireCoolDown <= 0f)
            {


                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Vector2 direction = (player.position - transform.position).normalized;

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                Projectile proj = projectile.GetComponent<Projectile>();
                proj.onHitDamage = damage;
                rb.velocity = direction * projectileSpeed;
                nextFireTime = Time.deltaTime + 1f / fireRate;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
                fireCoolDown = 1f / fireRate;
                Debug.Log("shooting");
            }
        }

    }

    bool HasLineOfSight()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);

        if (hit.collider != null)
        {
            //Debug.Log($"Obstacle detected: {hit.collider.name}");
            return false; // Obstacle is blocking the view
        }

        return true; // Clear line of sight
    }


    //change to a better logick that it doesnt float but moves on ground/platform
    //RigidBody Movment
    //good for drones maybe
    //
    void MoveEnemy()
    {
        if (leftPosition == null || RightPosition == null) return;

        // Check if the enemy reached the left or right patrol points
        if (Vector2.Distance(transform.position, targetPos) <= 0.5f)
        {
            // Switch target position to the other patrol point
            if (targetPos == (Vector2)leftPosition.position)
            {
                Debug.Log("target = right");
                targetPos = RightPosition.position;
                IsFacingRight = true;
            }
            else
            {
                Debug.Log("target = left");
                targetPos = leftPosition.position;
                IsFacingRight = false;
            }
        }

        // Smooth movement towards target
        Vector2 direction = ((Vector2)targetPos - (Vector2)transform.position).normalized;
        RB.velocity = new Vector2(direction.x * speed, RB.velocity.y);

        // Flip the enemy sprite if needed
        if (IsFacingRight && transform.localScale.x < 0)
            Flip();
        else if (!IsFacingRight && transform.localScale.x > 0)
            Flip();
    }

    // Flips the sprite to face the movement direction
    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
