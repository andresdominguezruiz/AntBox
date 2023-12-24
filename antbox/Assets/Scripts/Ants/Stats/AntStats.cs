using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum ActualAction{
    NOTHING,FARMING,SLEEPING,DIGGING
}


public class AntStats : CharacterStats
{
    //------------------------------------

    private ActualAction action=ActualAction.NOTHING;
    [SerializeField] protected int MIN_ENERGY=50;
    [SerializeField] protected int MAX_ENERGY=200;
    [SerializeField] protected int ENERGY_PER_AGE=5;

    [SerializeField] protected int MIN_FARMING_SPEED=50; //es porcentual la velocidad
    [SerializeField] protected int MAX_FARMING_SPEED=200;
    [SerializeField] protected int FARMING_SPEED_PER_AGE=10;

    [SerializeField] protected int MIN_DIGGING_SPEED=10; //es porcentual la velocidad
    [SerializeField] protected int MAX_DIGGING_SPEED=50;
    [SerializeField] protected int DIGGING_SPEED_PER_AGE=5;


    //----------------------------------------------------
    [SerializeField] private int maxEnergy;

    [SerializeField] private int actualEnergy;
    [SerializeField] private float farmingSpeed;
    [SerializeField] private float diggingSpeed;
    //TODO: Cuando tengamos juego base, mejorar stats para que cada hormiga sea buena en algo

    public float GetFarminSpeed(){
        return farmingSpeed;
    }

    public ActualAction GetAction(){
        return action;
    }
    public void StopDigging(){
        this.gameObject.GetComponent<NavMeshAgent>().enabled=true;
        this.gameObject.GetComponent<ExcavationMovement>().StopDigging();
        this.DoNothing();
    }
    public void StartFarming(){
        action=ActualAction.FARMING;
    }

    public void StartDigging(){
        action=ActualAction.DIGGING;
    }

    public void DoNothing(){
        action=ActualAction.NOTHING;
    }

    public string GetEnergyText(){
        return maxEnergy.ToString()+"/"+actualEnergy.ToString();
    }

    public float GetDiggingSpeed(){
        return diggingSpeed;
    }


    public void SetEnergy(int energy){
        actualEnergy=energy;
    }

    public void ApplyEnergyCost(int cost){
        SetEnergy(actualEnergy-cost);

    }
    public void InitAntStats(System.Random random){
        InitVariables(random);
        InitOtherVariables();
    }

    private void Start(){
        this.timeLastFrame=0f;
    }

    public void InitOtherVariables(){
        int randomEnergy=GetRandom().Next(MIN_ENERGY,MAX_ENERGY);
        int randomSpeed=GetRandom().Next(MIN_FARMING_SPEED,MAX_FARMING_SPEED);
        int randomDigging=GetRandom().Next(MIN_DIGGING_SPEED,MAX_DIGGING_SPEED);
        farmingSpeed=(float)(randomSpeed*1.0/100);
        diggingSpeed=(float)(randomDigging*1.0/100);
        maxEnergy=randomEnergy;
        SetEnergy(randomEnergy);

    }
}
