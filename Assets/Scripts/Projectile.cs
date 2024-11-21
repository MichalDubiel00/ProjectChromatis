using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 5f;
    [SerializeField] LayerMask ignoredLayers;
    [HideInInspector] public int onHitDamage = 5;

    Collider2D col;

    private void Start()
    {
        Destroy(gameObject,lifeTime);
        col = GetComponent<Collider2D>();
        col.excludeLayers = ignoredLayers;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit");

        Player player = collision.GetComponent<Player>();
        PlatformController platform = collision.GetComponent<PlatformController>();
        if (player != null)
        {
            Debug.Log("Hit");
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(onHitDamage);

            }
            Destroy(gameObject);
        }       
            Destroy(gameObject);
    }
}
