using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelLoad
{
    private GameObject levelLoader;
    private GameObject canvasMenu;

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

    [OneTimeTearDown]
    public void End(){
        SceneManager.UnloadSceneAsync(0);
        //Es necesario cerrar los recursos una vez testeado, solo cerrar la última escena.
    }
}
