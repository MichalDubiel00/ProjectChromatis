using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    private float _musicVolume = 50.0f;
    private float _soundVolume = 50.0f;
    
    public void BackToMenu()
    {
        pauseMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ChangeSoundFXVolume(float volume)
    {
        _soundVolume = volume;
        Debug.Log(_soundVolume+"First Volume");
        SoundManager.Instance.SetSoundFXVolume(_soundVolume);
    }

    public void ChangeMusicVolume(float volume)
    {
        _musicVolume = volume;
        SoundManager.Instance.SetMusicVolume(_musicVolume);
    }
    
}