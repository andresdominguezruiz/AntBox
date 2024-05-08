using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class LevelLoad
{
    private GameObject levelLoader;
    private GameObject canvasMenu;

    private readonly float newValue=0.25f;

    [OneTimeSetUp]
    public void Init(){
        SceneManager.LoadScene(0);
    }

    [SetUp]
    public void SetUp(){
        levelLoader=GameObject.Find("LevelLoader");
    }



    [UnityTest]
    public IEnumerator LevelLoadExist()
    {
        levelLoader=GameObject.Find("LevelLoader");
        Assert.True(levelLoader!=null
         && levelLoader.GetComponent<LevelLoader>()!=null);
        yield return null;
    }

    [UnityTest]
    public IEnumerator StartAndExitGame()
    {
        canvasMenu=GameObject.Find("Canvas");
        InitialMenu initial=canvasMenu.GetComponent<InitialMenu>();
        Assert.True(initial!=null);

        initial.Play();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(SceneManager.GetActiveScene().name == "Game");

        GameObject uiGame=GameObject.Find("UIGame");
        Assert.True(uiGame!=null && uiGame.GetComponent<PauseMenu>()!=null);
        uiGame.GetComponent<PauseMenu>().Quit();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(SceneManager.GetActiveScene().name == "InitialMenu");
        yield return null;
    }

    [UnityTest]
    public IEnumerator StartAndEndGame(){
        canvasMenu=GameObject.Find("Canvas");
        InitialMenu initial=canvasMenu.GetComponent<InitialMenu>();
        initial.Play();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);

        GameObject queen=GameObject.Find("Queen");
        Assert.That(queen!=null && queen.GetComponent<QueenStats>()!=null);
        queen.GetComponent<QueenStats>().Die();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+2f);
        Assert.True(SceneManager.GetActiveScene().name == "EndgameMenu");

        GameObject ui=GameObject.Find("UI");
        Assert.True(ui!=null && ui.GetComponent<EndgameMenu>()!=null);
        ui.GetComponent<EndgameMenu>().GoToInitialMenu();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(SceneManager.GetActiveScene().name == "InitialMenu");
        yield return null;
    }

    [UnityTest]
    public IEnumerator ChangeVolumeInInitialMenuAndChangeScene(){
        canvasMenu=GameObject.Find("Canvas");
        InitialMenu initial=canvasMenu.GetComponent<InitialMenu>();
        GameObject settingsButton=GameObject.Find("SettingsButton");
        settingsButton.GetComponent<Button>().onClick.Invoke(); //ESTO SIRVE PARA EJECUTAR UN BOTON

        GameObject settingsMenu=GameObject.Find("Settings");
        Assert.True(settingsMenu!=null);
        SettingsMenu settings=settingsMenu.GetComponent<SettingsMenu>();
        settings.Volume.value=newValue;
        settings.UpdateVolume();
        Assert.True(LevelLoader.Instance.ActualVolume.Equals(settings.Volume.value));

        GameObject goBackButton=GameObject.Find("BackButton");
        goBackButton.GetComponent<Button>().onClick.Invoke();
        initial.Play();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(LevelLoader.Instance.ActualVolume.Equals(settings.Volume.value));
        GameObject pauseButton=GameObject.Find("PauseBotton");
        pauseButton.GetComponent<Button>().onClick.Invoke();
        GameObject setting=GameObject.Find("SettingsButton");
        setting.GetComponent<Button>().onClick.Invoke();
        settingsMenu=GameObject.Find("Settings");
        Assert.True(settingsMenu.GetComponent<SettingsMenu>().Volume.value.Equals(newValue));
        goBackButton=GameObject.Find("BackButton");
        goBackButton.GetComponent<Button>().onClick.Invoke();

        GameObject uiGame=GameObject.Find("UIGame");
        Assert.True(uiGame!=null && uiGame.GetComponent<PauseMenu>()!=null);
        uiGame.GetComponent<PauseMenu>().Quit();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(SceneManager.GetActiveScene().name == "InitialMenu");
        yield return null;
    }



    [OneTimeTearDown]
    public void End(){
        SceneManager.UnloadSceneAsync(0);
        //Es necesario cerrar los recursos una vez testeado, solo cerrar la última escena.
    }
}
