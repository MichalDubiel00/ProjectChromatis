using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] PlayerHealth health;

    //Temp for Playroom presentation
    public bool juiceOn = true;
    public PlayerMovement juicyMovment;
    public SimplePlayerMovment simpleMovment;

    //Collected Colors capacity
    public enum ColorsEnum
    {
        Red, Blue, Yellow
    }
    int colorCount = Enum.GetValues(typeof(ColorsEnum)).Length;

    public Dictionary<string, int> Colors = new Dictionary<string, int>();
    public ColorBar colorBar;

    ColorsEnum _currentColor;
    public ColorsEnum CurrentColor { get => _currentColor; }

    [SerializeField] private int _maxCapacity = 10;
    public int MaxCapacity { get => _maxCapacity; }



    // Start is called before the first frame update
    void Start()
    {
        GameInput.instance.OnToggleAction += GameInput_OnToggleAction; ;
        GameInput.instance.OnDebugAction += GameInput_OnDebugAction;
        GameInput.instance.OnNextColorAction += GameInput_OnNextColorAction;
        GameInput.instance.OnPreviousColorAction += GameInput_OnPreviousColorAction;

        simpleMovment.enabled = false;

        Init();
    }

    private void GameInput_OnPreviousColorAction(object sender, System.EventArgs e)
    {
        //bit to much but works
        _currentColor = (ColorsEnum)(((int)_currentColor - 1 + colorCount) % colorCount);
        ChangeColor(_currentColor.ToString());
        UpdateColorUI(_currentColor.ToString());

        Debug.Log(_currentColor.ToString());
    }

    private void GameInput_OnNextColorAction(object sender, System.EventArgs e)
    {
        _currentColor = (ColorsEnum)(((int)_currentColor + 1) % colorCount);
        ChangeColor(_currentColor.ToString());
        UpdateColorUI(_currentColor.ToString());


        Debug.Log(_currentColor.ToString());

    }

    private void GameInput_OnToggleAction(object sender, System.EventArgs e)
    {
        if (juiceOn)
        {
            juiceOn = false;
            Debug.Log("Juicy Movment On");
            simpleMovment.enabled = false;
            juicyMovment.enabled = true;
        }
        else
        {
            juiceOn = true;
            Debug.Log("Juicy Movment Off");
            simpleMovment.enabled = true;
            juicyMovment.enabled = false;
        }
    }

    private void GameInput_OnDebugAction(object sender, System.EventArgs e)
    {
        Debug.Log("10 Damage Taken current: " + health.GetCurrentHealth() );
        health.TakeDamage(10);
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    void Init()
    {
        Colors.Add("Red", 0);
        Colors.Add("Blue", 0);
        Colors.Add("Yellow", 0);

        
        colorBar.SetMaxColorCapacity(MaxCapacity);
    }
    
    public void UpdateColorUI(string color)
    {
        colorBar.UpdateAmount(Colors[color]);
        Enum.TryParse(color, out _currentColor);
        
    }

    public void ChangeColor(string choosenColor)
    {

      
        colorBar.SetColorGradient(choosenColor);
    }
}
