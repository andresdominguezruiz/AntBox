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



    void Update(){
        FarmStats farmStats=this.gameObject.GetComponentInParent<FarmStats>();
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

    public void UpdateCanvasWithFarmStats(FarmStats stats){
        capacityText.text="Capacity:"+(stats.GetMaxCapacity()-stats.antsOfFarm.Count)+"/"+stats.GetMaxCapacity();
        typeText.text="Type:"+stats.GetTypeText();
        cycle.text="Cycle:"+stats.GetTimePerCycle()+"/"+stats.timePerCycleConsumed;
        energyCostText.text="EnergyCost:"+stats.energyCostOfCycle;
    }


    public void HideInfo()
    {
        infoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
}
