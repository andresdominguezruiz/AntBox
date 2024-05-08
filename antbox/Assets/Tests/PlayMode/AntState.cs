using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class AntState
{

    private GameObject antObject=new GameObject();
    private AntStats antStats;
    readonly System.Random random = new System.Random();

    private BarManager CreateOneBar(){
        GameObject sliderGameObject = new GameObject();
        Slider slider = sliderGameObject.AddComponent<Slider>();
        GameObject fillGameObject = new GameObject();
        Image fill = fillGameObject.AddComponent<Image>();
        GameObject oneBar=new GameObject();
        BarManager bar=oneBar.AddComponent<BarManager>();
        bar.Gradient=new Gradient();
        bar.Fill=fill;
        bar.Slider=slider;
        return bar;
    }

    private AllBarsManager CreateBars(){
        GameObject bars=new GameObject();
        AllBarsManager allBarsManager=bars.AddComponent<AllBarsManager>();
        allBarsManager.HealthBar=CreateOneBar();
        allBarsManager.HungerBar=CreateOneBar();
        allBarsManager.ThirstBar=CreateOneBar();
        allBarsManager.EnergyBar=CreateOneBar();
        return allBarsManager;
    }
    [OneTimeSetUp]
    public void Init(){
        GameObject player= new GameObject();
        player.AddComponent<Player>();
        GameObject stats=new GameObject();
        stats.AddComponent<StatisticsOfGame>();
    }

    [SetUp]
    public void SetUp(){
        AntStats ant = this.antObject.AddComponent<AntStats>();
        ant.AllBarsManager=CreateBars();
        ant.InitAntStats(random);
        antStats=ant;


    }
    
    [UnityTest]
    public IEnumerator AntStatsThroughtTime()
    {
        Assert.True(antStats.ActualHP.Equals(antStats.GetMaxHP())
         && antStats.AllBarsManager.HealthBar.Slider.maxValue
         .Equals(antStats.AllBarsManager.HealthBar.Slider.value));

        antStats.TakeDamage(-20);

        Assert.True(!antStats.ActualHP.Equals(antStats.GetMaxHP()));
        yield return null;
    }
}
