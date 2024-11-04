using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;

    public void SetMaxHealth(int health)
    {
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;

        fill.color = gradient.Evaluate(1f);

    }

    public void SetHealth(int health) 
    {
        healthBarSlider.value = health;

        fill.color = gradient.Evaluate(healthBarSlider.normalizedValue);
    }
}
