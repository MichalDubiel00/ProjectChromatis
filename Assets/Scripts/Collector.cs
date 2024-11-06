using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Collector : MonoBehaviour
{
    //Perhaps it would be better to move the logic to Player class for direct private acces
    public Dictionary<string,int> Colors = new Dictionary<string, int>();
    //TODO::change it to something prettier like a clor pallate
    public TextMeshProUGUI colorText;
    
    void Start()
    {
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ColorPicker picker = collision.GetComponent<ColorPicker>();
        if (picker) 
        {
            CollectColor(collision.gameObject,picker.Amount,picker.MyColor.ToString());
        }
    }
    void CollectColor(GameObject colourObj,int amount,string color)
    {
        //Destroys Color Drop
        Destroy(colourObj.transform.parent.gameObject);

        Colors[color] += amount;
        UpdateColorUI();

    }
    void UpdateColorUI() 
    {
        //better change to inviduals colors latter
        if (colorText)
            {
            colorText.text = "";
             foreach (KeyValuePair<string,int> color in Colors)
              colorText.text += $"{color.Key}: {color.Value}\n";
            }
    }
    void Init()
    {
        Colors.Add("Red", 0);
        Colors.Add("Blue", 0);
        Colors.Add("Yellow", 0);
    }
}
