using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth  = 100;
    private int currentHealth;
    [SerializeField] private HealthBar healthBar;

    // Start is called before the first frame update
    UnityEngine.SceneManagement.Scene _scene = new UnityEngine.SceneManagement.Scene();
    void Start()
    {
        _scene = SceneManager.GetActiveScene(); 
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public static Loader.Scene targerScene;

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
        //update to current Scene
        Loader.Scene scene;
        if(Enum.TryParse(_scene.name, out scene))
            Loader.Load(scene);
        else
            Debug.Log($"Add {_scene.name} to Loader Enum");


    }

}
