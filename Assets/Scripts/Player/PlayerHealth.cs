using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int _currentHealth;
    [SerializeField] private HealthBar _healthBar;

    private bool _isInvincible = false;
    [SerializeField] private float invincibilityDuration = 2f; // Duration of invincibility in seconds

    private Scene _scene;

    void Start()
    {
        _scene = SceneManager.GetActiveScene();
        _currentHealth = maxHealth;
        _healthBar.SetMaxHealth(maxHealth);
    }

    public int GetCurrentHealth() => _currentHealth;

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return; // Ignore damage if invincible

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }

        _healthBar.SetHealth(_currentHealth);
        StartCoroutine(BecomeInvincible());
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        if (_currentHealth > maxHealth)
            _currentHealth = maxHealth;

        _healthBar.SetHealth(_currentHealth);
    }

    private IEnumerator BecomeInvincible()
    {
        _isInvincible = true;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        for (int i = 0; i < invincibilityDuration * 5; i++) // Blink 5 times per second
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        sprite.enabled = true;
        _isInvincible = false;
    }

    //TODO DEATH SCREEN maybey lives like in mario?
    public void Die()
    {
        CheckpointManager.instance.Respawn();
    }
}
