using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth  = 100;
    private int currentHealth;
    [SerializeField] private HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    

    public int GetCurrentHealth() { return currentHealth; }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0) { Die(); }
        healthBar.SetHealth(currentHealth);
    }

    public void Heal(int amount) 
    {
        currentHealth += amount;
        healthBar.SetHealth(currentHealth);
    }

    public void Die() 
    {
        //TODO:: Handle Death
        Debug.Log("Dead");
        Loader.Load(Loader.Scene.PlayerHealthScene);
    }

}
