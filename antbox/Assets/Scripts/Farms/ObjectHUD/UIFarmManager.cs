using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIFarmManager : MonoBehaviour
{
    [SerializeField]
    private GameObject infoCanvas;

    [SerializeField]
    private TextMeshProUGUI capacityText;

    [SerializeField]
    private TextMeshProUGUI typeText;

    [SerializeField]
    private TextMeshProUGUI cycle;

    [SerializeField]
    private TextMeshProUGUI energyCostText;

    [SerializeField]
    private FarmStats farmStats;

    public GameObject InfoCanvas { get => infoCanvas; set => infoCanvas = value; }
    public TextMeshProUGUI CapacityText { get => capacityText; set => capacityText = value; }
    public TextMeshProUGUI TypeText { get => typeText; set => typeText = value; }
    public TextMeshProUGUI Cycle { get => cycle; set => cycle = value; }
    public TextMeshProUGUI EnergyCostText { get => energyCostText; set => energyCostText = value; }
    public FarmStats FarmStats { get => farmStats; set => farmStats = value; }

    void Start(){
        FarmStats=this.gameObject.GetComponentInParent<FarmStats>();
    }



    void Update(){
        if(FarmStats!=null){
            UpdateCanvasWithFarmStats(FarmStats);
        }
    }
    public void ShowInfo(){
        InfoCanvas.gameObject.SetActive(true);
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
        if(farmStats!=null && multipleFarmingMenu!=null){
            multipleFarmingMenu.InitMultipleFarmingMenu(farmStats);
        }
    }

    public void UpdateCanvasWithFarmStats(FarmStats stats){
        CapacityText.text="Remaining Capacity:"+(stats.GetMaxCapacity()-stats.antsOfFarm.Count)+"/"+stats.GetMaxCapacity();
        TypeText.text="Type:"+stats.GetTypeText();
        Cycle.text="Cycle:"+stats.timePerCycleConsumed+"/"+stats.GetTimePerCycle();
        EnergyCostText.text="EnergyCost:"+stats.energyCostOfCycle;
    }


    public void HideInfo()
    {
        InfoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
}
