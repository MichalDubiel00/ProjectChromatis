using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Collector : MonoBehaviour
{
    //Perhaps it would be better to move the logic to Player class for direct private acces
    public Dictionary<string,int> Colors = new Dictionary<string, int>();
    public List<ColorBar> colorBars;
    
    private int _maxcapacity = 10;//

    void Start()
    {
        //_colorBar.SetMaxColorCapacity(_maxcapacity);
        foreach (ColorBar colorBar in colorBars)
            colorBar.SetMaxColorCapacity(_maxcapacity);
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ColorPicker picker = collision.GetComponent<ColorPicker>();
        if (picker != null) 
        {
            CollectColor(collision.gameObject,picker.Amount,picker.MyColor.ToString());
        }
    }
    void CollectColor(GameObject colorObj,int amount,string color)
    {
        //Destroys Color Drop
        Destroy(colorObj.transform.parent.gameObject);

        if (Colors[color] < _maxcapacity )
         Colors[color] += amount;
        UpdateColorUI(color);

    }
    void UpdateColorUI(string color) 
    {
        foreach (ColorBar colorBar in colorBars)
        {
            if (colorBar.ColorName.ToString().Equals(color))
            { 
                colorBar.UpdateAmount(Colors[color]);
                break;
            }
        }
    }
    void Init()
    {
        Colors.Add("Red", 0);
        Colors.Add("Blue", 0);
        Colors.Add("Yellow", 0);
    }
}
