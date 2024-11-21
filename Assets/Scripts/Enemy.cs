using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Transform player;
    float nextFireTime;
    float fireCoolDown;

    [SerializeField] float detectionRange = 10f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float fireRate = 2f; // in Seconds
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] LayerMask obstacleMask;

    void Start()
    {
        player = FindFirstObjectByType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (HasLineOfSight()) 
            ShootAtPlayer();

    }
    void ShootAtPlayer()
    {
        if (player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange) 
        {
            fireCoolDown -= Time.deltaTime;
            if (fireCoolDown <= 0f)
            {



                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Vector2 direction = (player.position - transform.position).normalized;

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb.velocity = direction * projectileSpeed;
                nextFireTime = Time.deltaTime + 1f / fireRate;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
                fireCoolDown = 1f / fireRate;
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
            Debug.Log($"Obstacle detected: {hit.collider.name}");
            return false; // Obstacle is blocking the view
        }

        return true; // Clear line of sight
    }
}
