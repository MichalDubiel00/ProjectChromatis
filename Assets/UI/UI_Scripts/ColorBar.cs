using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class ColorBar : MonoBehaviour
{
    [SerializeField] private Slider _colorBarSlider;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Image _fill;
    public enum ColorNameEnum 
    {
        Red, Yellow, Blue
    }
    [SerializeField] ColorNameEnum _colorName;
    public ColorNameEnum ColorName { get => _colorName; }


    public void SetMaxColorCapacity(int maxCapacity)
    {
        _colorBarSlider.maxValue = maxCapacity;

        _fill.color = _gradient.Evaluate(1f);

    }

    public void UpdateAmount(int colorValue)
    {
        _colorBarSlider.value = colorValue;

        _fill.color = _gradient.Evaluate(_colorBarSlider.normalizedValue);
    }
}
