using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    public enum ColorEnum 
    {
        Red,
        Blue,
        Yellow,
        Gray
    }
    [SerializeField] private ColorEnum _mycolor;

    public ColorEnum MyColor
    { get => _mycolor;
      set => _mycolor = value;
    }  
}
