using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ColorCountUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countRed,countBlue,countYellow;
    private void Awake()
    {
        UpdateAmount(0,ColorPicker.ColorEnum.Gray);
    }
    public void UpdateAmount(int colorValue,ColorPicker.ColorEnum color)
    {
        switch (color)
        {
            case ColorPicker.ColorEnum.Blue:
                countBlue.text = colorValue.ToString();
                break;
            case ColorPicker.ColorEnum.Red:
                countRed.text = colorValue.ToString();
                break;
            case ColorPicker.ColorEnum.Yellow:
                countYellow.text = colorValue.ToString();
                break;
            default:
                countYellow.text = colorValue.ToString();
                countRed.text = colorValue.ToString();
                countBlue.text = colorValue.ToString();
                break;
        }
    }
}

