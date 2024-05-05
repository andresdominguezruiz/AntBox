using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.PerformanceTesting;
using UnityEngine.SceneManagement;

public class PerformanceTest
{

    [OneTimeSetUp]
    public void Init(){
        SceneManager.LoadScene(0);
    }



    [UnityTest,Performance]
    public IEnumerator PlayCard(){
        GameObject canvasMenu=GameObject.Find("Canvas");
        InitialMenu initialMenu=canvasMenu.GetComponent<InitialMenu>();
        initialMenu.Play();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);

        GameObject randomCard=GameObject.Find("BaseCard(Clone)");
        Assert.That(randomCard!=null);
        CardDisplay card=randomCard.GetComponent<CardDisplay>();
        Measure.Method(() => { card.UseCard();})
        .WarmupCount(10)
        .MeasurementCount(10)
        .IterationsPerMeasurement(5)
        .GC()
        .Run();

        GameObject uiGame=GameObject.Find("UIGame");
        Assert.True(uiGame!=null && uiGame.GetComponent<PauseMenu>()!=null);
        uiGame.GetComponent<PauseMenu>().Quit();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(SceneManager.GetActiveScene().name == "InitialMenu");
    }
    
    [UnityTest,Performance]
    public IEnumerator SpawnEnemies()
    {
        GameObject canvasMenu=GameObject.Find("Canvas");
        InitialMenu initialMenu=canvasMenu.GetComponent<InitialMenu>();
        initialMenu.Play();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(SceneManager.GetActiveScene().name == "Game");
        GameObject generator=GameObject.Find("Generator");
        NestManager nestManager=generator.GetComponent<NestManager>();
        Assert.True(nestManager!=null);
        Measure.Method(() => { nestManager.SpawnEnemiesForHorde();})
        .WarmupCount(10)
        .MeasurementCount(10)
        .IterationsPerMeasurement(5)
        .GC()
        .Run();

        GameObject uiGame=GameObject.Find("UIGame");
        Assert.True(uiGame!=null && uiGame.GetComponent<PauseMenu>()!=null);
        uiGame.GetComponent<PauseMenu>().Quit();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(SceneManager.GetActiveScene().name == "InitialMenu");
    }

    [OneTimeTearDown]
    public void End(){
        SceneManager.UnloadSceneAsync(0);
        //Es necesario cerrar los recursos una vez testeado, solo cerrar la última escena.
    }
}
