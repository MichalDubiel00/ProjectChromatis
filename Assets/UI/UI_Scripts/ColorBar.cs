using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class ColorBar : MonoBehaviour
{
    [SerializeField] Slider _colorBarSlider;
    [SerializeField] Image _fill;
    
    [SerializeField] Gradient _blueGradient;
    [SerializeField] Gradient _redGradient;
    [SerializeField] Gradient _yellowGradient;
 
    Gradient _currentGradient;
    
    public void SetMaxColorCapacity(int maxCapacity)
    {
        _colorBarSlider.maxValue = maxCapacity;

        UpdateFillColor();
    }

    public void UpdateAmount(int colorValue)
    {
        _colorBarSlider.value = colorValue;

        UpdateFillColor();
    }
    
    // Sets the color gradient based on the player's current color
    public void SetColorGradient(string playerColor)
    {
        // Select the appropriate gradient based on the player's color
        switch (playerColor)
        {
            case "Red":
                _currentGradient = _redGradient;
                break;
            case "Blue":
                _currentGradient = _blueGradient;
                break;
            case "Yellow":
                _currentGradient = _yellowGradient;
                break;
        }
        UpdateFillColor();
    }

    private void UpdateFillColor()
    {
        if (_currentGradient != null)
        {
            _fill.color = _currentGradient.Evaluate(_colorBarSlider.normalizedValue);
        }
    }
}

