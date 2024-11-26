using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Scene Loader
public static class Loader
{
    //Add new Scenes Here must equal Scene Name 1:1
    public enum Scene
    {
        PlayRoomScene,
        Loading,
        MainMenuScene,
        PlayerHealthScene,
        CollectingScene,
        DoorScene,
        MichalPlayRoomScene, 
        MichalWorkingOnplatformsScene
    }

    private static Scene targetScene;


    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
