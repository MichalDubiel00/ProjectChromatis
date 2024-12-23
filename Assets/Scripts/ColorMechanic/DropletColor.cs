using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletColor : MonoBehaviour
{
    private SpriteRenderer _SpriteRenderer;
    [SerializeField] private ColorPicker _ColorPicker;
    private Color _color;

    // Start is called before the first frame update
    void Awake()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        if (_ColorPicker != null)
            SetByPicker();

    }
    public void SetColor(Color color)
    {
        _SpriteRenderer.color = color;
    }
    void SetByPicker()
    {
        if (_ColorPicker)
        {
            switch (_ColorPicker.MyColor)
            {
                case ColorPicker.ColorEnum.Red:
                    _color = Color.red;
                    break;
                case ColorPicker.ColorEnum.Blue:
                    _color = Color.blue;
                    break;
                case ColorPicker.ColorEnum.Yellow
                :
                    _color = Color.yellow;
                    break;
            }
            _SpriteRenderer.color = _color;
        }
    }
}