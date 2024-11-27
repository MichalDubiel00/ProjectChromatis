using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public int notes;//notes change to HashTable
    [SerializeField] PlayerHealth health;
    SoundManager audioManager;
	//Temp for Playroom presentation
	public bool juiceOn = true;
    public PlayerMovement juicyMovment;
    public SimplePlayerMovment simpleMovment;

    //Collected Colors capacity
    int colorCount = Enum.GetValues(typeof(ColorPicker.ColorEnum)).Length-1;

    public Dictionary<ColorPicker.ColorEnum, int> Colors = new Dictionary<ColorPicker.ColorEnum, int>();
    public ColorBar colorBar;
    public SelectedColor selectedColor;

    ColorPicker.ColorEnum _currentColor;
    public ColorPicker.ColorEnum CurrentColor { get => _currentColor; }

    [SerializeField] private int _maxCapacity = 10;
    public int MaxCapacity { get => _maxCapacity; }


	private void Awake()
	{
		audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
	}
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
        _currentColor = (ColorPicker.ColorEnum)(((int)_currentColor - 1 + colorCount) % colorCount);
        ChangeColor(_currentColor);
        UpdateColorUI(_currentColor);

		audioManager.PlaySFX(audioManager.switchColor);
		Debug.Log(_currentColor.ToString());
    }

    private void GameInput_OnNextColorAction(object sender, System.EventArgs e)
    {
        _currentColor = (ColorPicker.ColorEnum)(((int)_currentColor + 1) % colorCount);
        ChangeColor(_currentColor);
        UpdateColorUI(_currentColor);

		audioManager.PlaySFX(audioManager.switchColor);
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
        Colors.Add(ColorPicker.ColorEnum.Red, 0);
        Colors.Add(ColorPicker.ColorEnum.Blue, 0);
        Colors.Add(ColorPicker.ColorEnum.Yellow, 0);

        
        colorBar.SetMaxColorCapacity(MaxCapacity);
    }
    
    public void UpdateColorUI(ColorPicker.ColorEnum color)
    {
        colorBar.UpdateAmount(Colors[color]);
        _currentColor = color;        
    }

    public void ChangeColor(ColorPicker.ColorEnum choosenColor)
    {
        selectedColor.UpdateSelected(choosenColor);
        colorBar.SetColorGradient(choosenColor);
    }
}
