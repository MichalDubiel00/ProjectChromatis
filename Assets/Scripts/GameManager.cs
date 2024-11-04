using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMananger : MonoBehaviour
{

    bool isPause = false;
    // Start is called before the first frame update
    public static GameMananger instance { get; private set; }

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        GameInput.instance.OnPauseAction += GameInput_OnPauseAction;
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TogglePauseGame()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1.0f;
            OnGameUnPaused?.Invoke(this, EventArgs.Empty);

        }
    }
}
