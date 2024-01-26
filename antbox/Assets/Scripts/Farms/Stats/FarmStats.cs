using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Type{
    WATER,FOOD
}

public class FarmStats : MonoBehaviour
{

    //LIMITS:
    [SerializeField] private int MAX_RESOURCES=5;
    [SerializeField] private int MIN_RESOURCES=1;
    [SerializeField] private int maxTimePerCycle=40;
    [SerializeField] private int minTimePerCycle=10;
    [SerializeField] private int minLimitOfCapacity=2;
    [SerializeField] private int maxLimitOfCapacity=8;
    [SerializeField] private int maxEnergyCost=5;
    [SerializeField] private int minEnergyCost=1;

    private System.Random random = new System.Random();


    //-----------------------------------------
    public bool broken=false;
    private Type type;
    [SerializeField] private int timePerCycle=20;
    public float timePerCycleConsumed;
    public float timeLastFrame;

    [SerializeField] public int energyCostOfCycle=2;

    [SerializeField] private int maxCapacity=4;
    public int actualCapacity;

    public List<GameObject> antsWorkingInFarm=new List<GameObject>();//hormigas que estan actualmente trabajando
    //utilizar este para estadísticas
    public List<GameObject> antsOfFarm=new List<GameObject>();//hormigas de la granja (ya esten trabajando o llendo
    // hacia la granja)
    //AÑADIR HORMIGAS UNA VEZ SELECCIONADA LA GRANJA, PERO LAS STATS SE ACTUALIZARÁN CUANDO
    //ENTRE EN COLISIÓN LA HORMIGA CON LA GRANJA
    //utilizar este para estado de la granja

    public int GetTimePerCycle(){
        return timePerCycle;
    }

    public void SetMinResources(int resources){
        if(resources>=1 && resources<MAX_RESOURCES) MIN_RESOURCES=resources;
    }
    public void SetMaxResources(int resources){
        if(resources>MIN_RESOURCES) MAX_RESOURCES=resources;
    }

    public void ApplyEffect(FarmEffect farmEffect){
        if(farmEffect.farmEffect.Equals(UpdateEffectOnFarm.FARM_CYCLE)){
            int newTimePerCycle=farmEffect.multiplicatorValue
            *(this.timePerCycle+(int)farmEffect.sumValue);
            if(newTimePerCycle>maxTimePerCycle) this.timePerCycle=maxTimePerCycle; //ESTO SE HACE PARA PONER UN LÍMITE
            else if(newTimePerCycle<minTimePerCycle) this.timePerCycle=minTimePerCycle;
            else this.timePerCycle=newTimePerCycle;
            
        }
        else if(farmEffect.farmEffect.Equals(UpdateEffectOnFarm.CAPACITY)){
            maxCapacity=farmEffect.multiplicatorValue*(maxCapacity+(int)farmEffect.sumValue);
            if(maxCapacity>maxLimitOfCapacity) maxCapacity=maxLimitOfCapacity;
            else if(maxCapacity<minLimitOfCapacity) maxCapacity=minLimitOfCapacity;
        }
        else if(farmEffect.farmEffect.Equals(UpdateEffectOnFarm.FARM_RESOURCES)){
            SetMinResources(farmEffect.multiplicatorValue*(MIN_RESOURCES+(int)farmEffect.sumValue));
            SetMaxResources(farmEffect.multiplicatorValue*(MAX_RESOURCES+(int)farmEffect.sumValue));
        }else if(farmEffect.farmEffect.Equals(UpdateEffectOnFarm.ENERGY_COST)){
            energyCostOfCycle=farmEffect.multiplicatorValue*(energyCostOfCycle+(int)farmEffect.sumValue);
            if(energyCostOfCycle<minEnergyCost) energyCostOfCycle=minEnergyCost;
            else if(energyCostOfCycle>maxEnergyCost) energyCostOfCycle=maxEnergyCost;
        }
    }

    public void ProcessUpdateEffectOfAction(List<FarmEffect> farmEffects){
        foreach(FarmEffect farmEffect in farmEffects){
            ApplyEffect(farmEffect);
        }
    }
    public Type GetTypeOfFarm(){
        return type;
    }


    public void InitWaterFarm(bool isBroken,System.Random random){
        type=Type.WATER;
        InitValues(isBroken,random);
    }

    public void InitFoodFarm(bool isBroken,System.Random random){
        type=Type.FOOD;
        InitValues(isBroken,random);
    }

    private void InitValues(bool isBroken,System.Random random){
        this.random=random;
        actualCapacity=maxCapacity;
        timePerCycleConsumed=0;
        this.broken=isBroken;
    }

    public int GetMaxCapacity(){
        return maxCapacity;
    }

    public string GetTypeText(){
        return type.ToString();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!broken){
            if(Time.time -timeLastFrame>=1.0f){
            float sumSpeed=0f;
            foreach(GameObject ant in antsWorkingInFarm){
                sumSpeed+=ant.GetComponent<AntStats>().GetFarmingSpeed();
            }
            timePerCycleConsumed+=sumSpeed;
            if(antsWorkingInFarm.Count>0){
                    List<AntStats> list=new List<AntStats>();
                    foreach(GameObject ant in antsWorkingInFarm){
                        AntStats stats=ant.GetComponent<AntStats>();
                        stats.ApplyEnergyCost(energyCostOfCycle);
                        if(stats.GetActualEnergy()<energyCostOfCycle){
                            list.Add(stats);
                        }
                    }
                    //Si iteras sobre una colección a la vez que lo modificas, da error, es por eso que separo
                    foreach(AntStats antStats in list){
                        antStats.CancelAntAction();
                    }
                }
            if(timePerCycleConsumed>=timePerCycle){
                ContainerData container=FindObjectOfType<ContainerData>();
                container.AddResources(random.Next(MIN_RESOURCES,MAX_RESOURCES),type);
                timePerCycleConsumed=0f;
            }
            timeLastFrame=Time.time;
        }
        }
        
    }

    public bool CanAntWorkInHere(){
        bool res=false;
        if(antsOfFarm.Count<maxCapacity) res=true;
        return res;
    }
    public void AddAntToFarm(GameObject item){
        antsOfFarm.Add(item);
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        GameObject item=other.gameObject;
        Debug.Log("Name:"+item.name);
        if(item.CompareTag("Ant") && item.GetComponent<AntStats>().GetAction().Equals(ActualAction.FARMING)
         && antsOfFarm.Contains(item)){
            antsWorkingInFarm.Add(item);
         } 
    }


    public void OnTriggerStay2D(Collider2D collision)
    {
        GameObject item=collision.gameObject;
        Debug.Log("Name:"+item.name);
        if(item.CompareTag("Ant") && item.GetComponent<AntStats>().GetAction().Equals(ActualAction.FARMING)
         && antsOfFarm.Contains(item)){
            if(!antsWorkingInFarm.Contains(item)) antsWorkingInFarm.Add(item);
         } 
    }
}
