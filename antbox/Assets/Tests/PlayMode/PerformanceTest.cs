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
    public IEnumerator SpawnEnemies()
    {
        GameObject canvasMenu=GameObject.Find("Canvas");
        InitialMenu initialMenu=canvasMenu.GetComponent<InitialMenu>();
        initialMenu.Play();
        Measure.Method(() => { initialMenu.Play();})
        .WarmupCount(10)
        .MeasurementCount(10)
        .IterationsPerMeasurement(5)
        .GC()
        .Run();
        yield return new WaitForSecondsRealtime(LevelLoader.Instance.transitionTime+1f);
        Assert.True(SceneManager.GetActiveScene().name == "Game");
    }

    [OneTimeTearDown]
    public void End(){
        SceneManager.UnloadSceneAsync(1);
        //Es necesario cerrar los recursos una vez testeado, solo cerrar la última escena.
    }
}
