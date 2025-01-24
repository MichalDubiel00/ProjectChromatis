using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMent : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private GameObject settingsMenu;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => {
            GameMananger.instance.TogglePauseGame();
        });
        menuButton.onClick.AddListener(() => {
            GameMananger.instance.TogglePauseGame();
            Loader.Load(Loader.Scene.Menu);
        });
    }
    private void Start()
    {
        GameMananger.instance.OnGamePaused += GameManager_OnGamePaused;
        GameMananger.instance.OnGameUnPaused += GameManager_OnGameUnPaused;

        Hide();
    }



    private void GameManager_OnGameUnPaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }

    public void OpenOptions()
    {
        settingsMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
