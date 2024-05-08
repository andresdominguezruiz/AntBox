using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public int actualCapacity=0;

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
        if(resources>=1 && resources<MAX_RESOURCES){
            MIN_RESOURCES=resources;
        }
    }
    public void SetMaxResources(int resources){
        if(resources>MIN_RESOURCES){
            MAX_RESOURCES=resources;
        }
    }

    public void ApplyEffect(FarmEffect farmEffect){
        if(farmEffect.farmEffect.Equals(UpdateEffectOnFarm.FARM_CYCLE)){
            int newTimePerCycle=farmEffect.MultiplicatorValue
            *(this.timePerCycle+(int)farmEffect.SumValue);
            if(newTimePerCycle>maxTimePerCycle){
                this.timePerCycle=maxTimePerCycle; //ESTO SE HACE PARA PONER UN LÍMITE
            }
            else if(newTimePerCycle<minTimePerCycle){
                this.timePerCycle=minTimePerCycle;
            }
            else{
                this.timePerCycle=newTimePerCycle;
            }
            
        }
        else if(farmEffect.farmEffect.Equals(UpdateEffectOnFarm.CAPACITY)){
            maxCapacity=farmEffect.MultiplicatorValue*(maxCapacity+(int)farmEffect.SumValue);
            if(maxCapacity>maxLimitOfCapacity){
                maxCapacity=maxLimitOfCapacity;
            }
            else if(maxCapacity<minLimitOfCapacity){
                maxCapacity=minLimitOfCapacity;
            }
        }
        else if(farmEffect.farmEffect.Equals(UpdateEffectOnFarm.FARM_RESOURCES)){
            SetMinResources(farmEffect.MultiplicatorValue*(MIN_RESOURCES+(int)farmEffect.SumValue));
            SetMaxResources(farmEffect.MultiplicatorValue*(MAX_RESOURCES+(int)farmEffect.SumValue));
        }else if(farmEffect.farmEffect.Equals(UpdateEffectOnFarm.ENERGY_COST)){
            energyCostOfCycle=farmEffect.MultiplicatorValue*(energyCostOfCycle+(int)farmEffect.SumValue);
            if(energyCostOfCycle<minEnergyCost){
                energyCostOfCycle=minEnergyCost;
            }
            else if(energyCostOfCycle>maxEnergyCost){
                energyCostOfCycle=maxEnergyCost;
            }
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
        actualCapacity=0;
        timePerCycleConsumed=0;
        this.broken=isBroken;
    }

    public int GetMaxCapacity(){
        return maxCapacity;
    }

    public string GetTypeText(){
        return type.ToString();
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
            CheckTimePerCycle();
            timeLastFrame=Time.time;
        }
        }else{
            DestroyFarm();
        }
        
    }
    public void CheckTimePerCycle(){
        if(timePerCycleConsumed>=timePerCycle){
                ContainerData container=FindObjectOfType<ContainerData>();
                container.AddResources(random.Next(MIN_RESOURCES,MAX_RESOURCES),type);
                timePerCycleConsumed=0f;
            }else if(timePerCycleConsumed<=-1*timePerCycle){
                broken=true;
            }
    }

    public void DestroyFarm(){
        SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
        item.IsSelected=false;
        List<GameObject> antsInFarm=new List<GameObject>(antsOfFarm); //Necesito clonar
        //porque no puedo recorrer una lista la cuál voy a ir vaciando.
        foreach(GameObject ant in antsInFarm){
            AntStats stats=ant.GetComponent<AntStats>();
            if(stats!=null){
                stats.StopFarming();
            }
        }
        FarmGenerator farmGenerator=FindObjectOfType<FarmGenerator>(false);
        if(farmGenerator!=null && this.type.Equals(Type.FOOD)){
            farmGenerator.foodFarms.Remove(this.gameObject);
        }else if(farmGenerator!=null && this.type.Equals(Type.WATER)){
            farmGenerator.waterFarms.Remove(this.gameObject);
        }
        item.RemoveSelectableItem();
        Destroy(this.gameObject);
    }

    public bool CanAntWorkInHere(){
        bool res=false;
        if(antsOfFarm.Count<maxCapacity){
            res=true;
        }
        return res;
    }
    public void AddAntToFarm(GameObject item){
        antsOfFarm.Add(item);
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        GameObject item=other.gameObject;
        if(item.CompareTag("Ant") && item.GetComponent<AntStats>().GetAction().Equals(ActualAction.FARMING)
         && antsOfFarm.Contains(item)){
            antsWorkingInFarm.Add(item);
         } 
    }


    public void OnTriggerStay2D(Collider2D collision)
    {
        GameObject item=collision.gameObject;
        if(item.CompareTag("Ant") && item.GetComponent<AntStats>().GetAction().Equals(ActualAction.FARMING)
         && antsOfFarm.Contains(item)){
            if(!antsWorkingInFarm.Contains(item)){
                antsWorkingInFarm.Add(item);
            }
         } 
    }
}
