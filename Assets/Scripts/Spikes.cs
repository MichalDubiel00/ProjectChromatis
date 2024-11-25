using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float knockbackForce = 5f; 

    private void OnTriggerEnter2D(Collider2D collider)
    {

        PlayerHealth health = collider.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage); 

            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, knockbackForce); 
            }
        }
    }
}
