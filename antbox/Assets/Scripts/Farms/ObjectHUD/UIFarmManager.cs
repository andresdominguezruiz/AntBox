using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIFarmManager : MonoBehaviour
{
    public GameObject infoCanvas;
    public TextMeshProUGUI capacityText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI cycle;
    public TextMeshProUGUI energyCostText;
    public FarmStats farmStats;

    void Start(){
        farmStats=this.gameObject.GetComponentInParent<FarmStats>();
    }



    void Update(){
        if(farmStats!=null) UpdateCanvasWithFarmStats(farmStats);
    }
    public void ShowInfo(){
        infoCanvas.gameObject.SetActive(true);
    }
    public void UpdateAndShowInfo()
    {
        FarmStats farmStats=this.gameObject.GetComponentInParent<FarmStats>();
        UpdateCanvasWithFarmStats(farmStats);
        ShowInfo();
    }

    public void StartMultipleFarminMenu(){
        FarmStats farmStats=this.gameObject.GetComponentInParent<FarmStats>();
        MultipleFarmingMenu multipleFarmingMenu=FindObjectOfType<MultipleFarmingMenu>(true);
        if(farmStats!=null && multipleFarmingMenu!=null) multipleFarmingMenu.InitMultipleFarmingMenu(farmStats);
    }

    public void UpdateCanvasWithFarmStats(FarmStats stats){
        capacityText.text="Remaining Capacity:"+(stats.GetMaxCapacity()-stats.antsOfFarm.Count)+" of "+stats.GetMaxCapacity();
        typeText.text="Type:"+stats.GetTypeText();
        cycle.text="Cycle:"+stats.GetTimePerCycle()+"/"+stats.timePerCycleConsumed;
        energyCostText.text="EnergyCost:"+stats.energyCostOfCycle;
    }


    public void HideInfo()
    {
        infoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
}
